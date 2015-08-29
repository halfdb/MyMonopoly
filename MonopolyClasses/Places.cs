using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Monopoly.Classes
{
    
    public abstract partial class Place
    {

        public int Id { get; private set; }

        protected Place(int id)
        {
            Id = id;
        }

        //public static PlaceType StringToPlaceType(string placeType)
        //{
        //    PlaceType ret;
        //    switch (placeType)
        //    {
        //        case "None":
        //            ret = PlaceType.None;
        //            break;
        //        case "Empty":
        //            ret = PlaceType.Empty;
        //            break;
        //        case "Estate":
        //            ret = PlaceType.Estate;
        //            break;
        //        case "Bank":
        //            ret = PlaceType.Bank;
        //            break;
        //        case "News":
        //            ret = PlaceType.News;
        //            break;
        //        case "Lottery":
        //            ret = PlaceType.Lottery;
        //            break;
        //        default:
        //            throw new Exception();
        //            break;
        //    }
        //    return ret;
        //}


        /*
        public static Place NewInstance(int id, PlaceType type)
        {
            Place ret;

            switch (type)
            {
                case PlaceType.None:
                    if (id == 0)
                    {
                        ret = new None(0);
                    }
                    else goto default;
                    break;
                case PlaceType.Empty:
                    ret = new Empty(id);
                    break;
                case PlaceType.Estate:
                    ret = new Estate(id);
                    break;
                case PlaceType.Bank:
                    ret = new Bank(id);
                    break;
                case PlaceType.News:
                    ret = new News(id);
                    break;
                case PlaceType.Lottery:
                    ret = new Lottery(id);
                    break;
                default:
                    throw new Exception();
            }
            return ret;
        }
        */

        public abstract void Trigger(Player player);

        public Place Next(Direction direction)
        {
            return Game.Map.FindPlace(Id + (int)direction);
        }


    }

    public sealed class None : Place
    {
        public None(int id)
            : base(id)
        {
            if (id != 0)
            {
                throw new Exception();
            }
            
        }

        public override void Trigger(Player player)
        {
            throw new Exception("You shall not pass!");
        }
    }

    public class Empty : Place
    {
        static Empty()
        {
            Map.NewInstanceFromXml += NewInstanceFromXml;
        }

        public Empty(int id)
            : base(id)
        {
            

        }

        public override void Trigger(Player player)
        {
            player.Inform("Here is empty, have a rest.");
        }

        internal static void NewInstanceFromXml(XmlNode detail, ref Place result,int id)
        {
            if (detail.Name != "Empty")
            {
                return;
            }
            result = new Empty(id);
        }
    }



    public class News : Place
    {
        static News()
        {
            Map.NewInstanceFromXml += NewInstanceFromXml;
        }

        public News(int id)
            : base(id)
        {
            
        }

        public override void Trigger(Player player)
        {
            foreach (Player item in Game.Players)
            {
                switch(new Random().Next(4))
                {
                    case 0:
                        item.AddCash(item.Cash /20);
                        break;
                    case 1:
                        item.SpendCash(item.Cash / 20);
                        break;
                    case 2:
                        item.AddSaving(item.Saving / 20);
                        break;
                    case 3:
                        item.SpendSaving(item.Saving / 20);
                        break;

                }
            }
        }

        internal static void NewInstanceFromXml(XmlNode detail, ref Place result,int id)
        {
            if (detail.Name != "News")
            {
                return;
            }
            result = new News(id);
        }
    }

}
