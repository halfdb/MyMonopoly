using System;
using Monopoly.Classes.Interfaces;
using System.Xml;

namespace Monopoly.Classes
{

    public partial class Estate : Place
    {
        //field and props
        protected const int BASE_RENT = 200;
        protected const int BASE_PRICE = 600;

        private int _OwnerId;
        public Player Owner
        {
            get
            {
                if (_OwnerId == -1)
                {
                    return null;
                }
                return Game.Players[_OwnerId];
            }
            set
            {
                if (value != null)
                {
                    if (_OwnerId != -1)
                    {
                        Owner.RemoveEstate(this);
                    }
                    value.AddEstate(this);
                    _OwnerId = value.Id;
                }
                else
                {
                    _OwnerId = -1;
                }
            }
        }
        public virtual int Price
        {
            get
            {
                return BASE_PRICE * Level;
            }
        }
        public virtual int Rent
        {
            get
            {
                int cnt = 0;
                Place prev = this, next = this;
                for (int i = 1; i <= 5; i++)
                {
                    prev = this.Next(Direction.CCW);
                    next = this.Next(Direction.CW);
                    if (prev is Estate && (prev as Estate).Owner == Owner)
                    {
                        cnt++;
                    }
                    if (next is Estate && (next as Estate).Owner == Owner)
                    {
                        cnt++;
                    }
                }
                return (cnt + Level) * BASE_RENT;
            }
        }
        private int _Level;
        public virtual int Level
        {
            get
            {
                return _Level;
            }

            set
            {
                if (value <= 6)
                {
                    _Level = value;
                }
            }
        }
        public virtual string Name { get; private set; }

        //static ctor
        static Estate()
        {
            Map.NewInstanceFromXml += NewInstanceFromXml;
        }

        //ctor
        public Estate(int id)
            : base(id)
        {
            Level = 1;
            Owner = null;
        }

        //methods
        public override void Trigger(Player player)
        {
            PlayerArrivedEvent(this, new PlayerArrivedEventArgs(player));
            if (Owner == null)
            {
                if (player.Cash >= Price)
                {
                    if (player.Confirm("Would you like to buy the estate? Price: " + Price))
                    {
                        player.BuyEstate(this);
                    }
                }
                else
                {
                    player.Inform("Sorry, but you cannot afford this estate. Price: " + Price);
                }
            }
            else if (Owner == player)
            {
                if (player.Cash >= Price && Level != 6)
                {
                    if (player.Confirm("Would you like to upgrade the estate? Price: " + Price))
                    {
                        Upgrade(player);
                    }
                }
                else if (Level == 6)
                {
                    player.Inform("The estate is fully upgraded!");
                }
                else
                {
                    player.Inform("Sorry, but you cannot afford upgrading this estate. Price: " + Price);
                }
            }
            else
            {
                player.PayRent();
            }
        }

        protected virtual void Upgrade(Player player)
        {
            if (player != Owner)
            {
                return;
            }
            if (Level < 6)
            {
                if (player.SpendCash(Price))
                {
                    UpgradeEvent(this);
                    ++Level;
                    player.Inform("Congratulations! You have upgraded this estate!");
                }
            }
        }

        internal static void NewInstanceFromXml(XmlNode detail, ref Place result, int id)
        {
            if (detail.Name != "Estate")
            {
                return;
            }
            result = new Estate(id);
            Estate obj = result as Estate;
            obj.Name = detail.SelectSingleNode("name").InnerText;
            if ((detail.ParentNode.ParentNode as XmlElement).GetAttribute("continue") == "true")
            {
                obj.Owner = Game.Players[int.Parse(detail.SelectSingleNode("owner_id").InnerText)];
                obj.Level = int.Parse(detail.SelectSingleNode("level").InnerText);
            }
        }

    }

    public partial class Bank : Place
    {
        static Bank()
        {
            Map.NewInstanceFromXml += NewInstanceFromXml;
        }

        public Bank(int id)
            : base(id)
        {

        }

        internal static void NewInstanceFromXml(XmlNode detail, ref Place result, int id)
        {
            if (detail.Name != "Bank")
            {
                return;
            }
            result = new Bank(id);
        }

        public override void Trigger(Player player)
        {
            PlayerArrivedEvent(this, new PlayerArrivedEventArgs(player));
            string[] choices = { "Welcom to the bank! How can I help you?", "1.Deposit", "2.Withdraw", "3.Leave" };
            int choice = player.Choose(choices);
            while (choice != 3)
            {
                switch (choice)
                {
                    case 1:
                        if (player.Deposit(player.InputInt("How much?")))
                        {
                            player.Inform("Done!");
                        }
                        else
                        {
                            player.Inform("Failed!");
                        }
                        break;
                    case 2:
                        if (player.Withdraw(player.InputInt("How much?")))
                        {
                            player.Inform("Done!");
                        }
                        else
                        {
                            player.Inform("Failed!");
                        }
                        break;
                    default:
                        throw new Exception();
                }
                choice = player.Choose(choices);
            }
        }
    }

    public partial class Casino : Place
    {
        private const double MAGIC_NUMBER = 0.4;

        static Casino()
        {
            Map.NewInstanceFromXml += NewInstanceFromXml;
        }

        public Casino(int id)
            : base(id)
        {

        }

        internal static void NewInstanceFromXml(XmlNode detail, ref Place result, int id)
        {
            if (detail.Name != "Casino")
            {
                return;
            }
            result = new Casino(id);
        }

        public override void Trigger(Player player)
        {
            PlayerArrivedEvent(this, new PlayerArrivedEventArgs(player));
            if (player.Confirm("I want to play a game..."))
            {
                int invest = player.InputInt("How much do you want to bet?");
                player.SpendCash(invest);
                int revenue = Calculate(invest);
                player.AddCash(revenue);
                if (invest >= revenue)
                {
                    player.Inform("Poor guy...");
                    ResultEvent(player,new CasinoEventArgs(false));
                }
                else
                {
                    player.Inform("Lucky you...");
                    ResultEvent(player,new CasinoEventArgs(true));
                }
            }
        }

        protected virtual int Calculate(int invest)
        {
            return (int)((new Random().NextDouble() + MAGIC_NUMBER) * invest);
        }
    }

}
