using System;
using System.Collections.Generic;
using System.Xml;

namespace Monopoly.Classes
{
    public sealed class Map
    {

        public delegate void NewInstanceFromXmlDel(XmlNode detail, ref Place result, int id);
        public static NewInstanceFromXmlDel NewInstanceFromXml;

        public Dictionary<int, Tuple<int, int>> PlaceIdToCoordinate;
        public Place[,] Places { get; private set; }
        public int Row { get; private set; }
        public int Col { get; private set; }
        private int _MaxId { get; set; }

        static Map()
        {
            NewInstanceFromXml += Estate.NewInstanceFromXml;
            NewInstanceFromXml += News.NewInstanceFromXml;
            NewInstanceFromXml += Empty.NewInstanceFromXml;
            NewInstanceFromXml += Bank.NewInstanceFromXml;
            NewInstanceFromXml += Casino.NewInstanceFromXml;
        }

        public Map(XmlElement map)
        {
            Row = int.Parse(map.GetAttribute("row"));
            Col = int.Parse(map.GetAttribute("col"));
            Places = new Place[Row, Col];
            PlaceIdToCoordinate = new Dictionary<int, Tuple<int, int>>();
            _MaxId = 0;

            for (int r = 0; r < Row; r++)
            {
                for (int c = 0; c < Col; c++)
                {
                    Places[r, c] = new None(0);
                }
            }

            foreach (XmlNode item in map.ChildNodes)
            {
                int r = int.Parse((item as XmlElement).GetAttribute("row"));
                int c = int.Parse((item as XmlElement).GetAttribute("col"));
                int id = int.Parse((item as XmlElement).GetAttribute("id"));
                NewInstanceFromXml(item, ref Places[r, c], id);
                PlaceIdToCoordinate.Add(id, new Tuple<int, int>(r, c));
                if (id > _MaxId)
                {
                    _MaxId = id;
                }
            }

        }

        public Place FindPlace(int id)
        {
            //Console.WriteLine("finding place with id " + id);
            if (id == 0)
            {
                return FindPlace(_MaxId);
            }
            if (id == _MaxId + 1)
            {
                return FindPlace(1);
            }
            Tuple<int, int> target = PlaceIdToCoordinate[id];
            return Places[target.Item1, target.Item2];
            throw new PlaceNotFoundException(id);
        }

        [Serializable]
        public class PlaceNotFoundException : Exception
        {
            public int Id { get; private set; }
            public PlaceNotFoundException(int id)
                : base("Place with id " + id + " not found.")
            {
                Id = id;
            }
        }

    }
}
