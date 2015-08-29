using System;
using System.Xml;
using Monopoly.Classes;

namespace Monopoly.ConsoleGame
{
    class ConsoleGame : Classes.Interfaces.IIo
    {

        public ConsoleGame()
        {
            Console.Title = "Monopoly";

            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\hpkel\Documents\Visual Studio 2015\Projects\Monopoly\DefaultMap1.xml");
            XmlElement map = doc.SelectSingleNode("map") as XmlElement;
            Game.AllGameEvents += AllGameEventHandler;
            Game.Initialize(this, 2, map);

            foreach (Player item in Game.Players)
            {
                item.AllEstateEvent += AllPlayerEstateEventHandler;
                item.AllMoneyEvent += AllPlayerMoneyEventHandler;
                item.RevertDirectionEvent += OtherPlayerEventHandler;
                item.GoingEvent += OtherPlayerEventHandler;
                item.BankruptEvent += OtherPlayerEventHandler;
            }

            Game.Start();
        }

        public void AllGameEventHandler(Game.GameEventArgs e)
        {
            Console.WriteLine(e.Info);
        }

        public void AllPlayerMoneyEventHandler(Player player, Player.MoneyEventArgs e)
        {
            Console.WriteLine(e.Info + "\nAmount: " + e.Amount+"\nPlayer: "+player.Name);
        }

        public void AllPlayerEstateEventHandler(Player p, Player.EstateEventArgs e)
        {
            Console.WriteLine(e.Info+" Estate: "+e.Target.Name);
        }

        public void OtherPlayerEventHandler(Player p,Player.PlayerEventArgs e)
        {
            Console.WriteLine(e.Info);
        }

        public int Choose(string[] choices, Player target)
        {
            foreach (string item in choices)
            {
                Console.WriteLine(item);
            }
            int ret;
            while (true)
            {
                try
                {
                    ret = int.Parse(Console.ReadLine());
                    if (ret <= 0 || ret >= choices.Length)
                    {
                        Console.WriteLine("Illegal input. Please enter again.");
                        continue;
                    }
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please retry...");
                }
            }
            return ret;
        }

        public bool Confirm(string cue, Player target)
        {
            Console.WriteLine(cue + "\nY for Yes or Any Other for No:");
            string ans = Console.ReadLine();
            switch (ans[0])
            {
                case 'Y':
                case 'y':
                    return true;
                case 'N':
                case 'n':
                    return false;
                default:
                    return false;
            }
        }

        public int GetSteps()
        {
            Console.WriteLine("\n"+Game.CurrentPlayer.Name+", it's your turn.");
            return -1;
        }

        public string Read(string cue, Player target)
        {
            return Console.ReadLine();
        }

        public int ReadInt(string cue, Player target)
        {
            Console.WriteLine(cue);
            string s = Console.ReadLine();
            Console.WriteLine(s);
            return int.Parse(s);
        }

        public void Write(object item, Player target)
        {
            Console.WriteLine(item);
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            
            new ConsoleGame();
        }

    }


}
