using System;
using System.Collections.Generic;
using System.Xml;

namespace Monopoly.Classes
{


    public partial class Player
    {
        protected readonly double INTEREST_RATE = 0.1;

        public int Id { get; private set; }
        public string Name { get; set; }
        public int Cash { get; protected set; }
        public int Saving { get; protected set; }
        public Direction Direction { get; protected set; }
        private bool _IsBankrupted;
        public bool IsBankrupted
        {
            get { return _IsBankrupted; }
            set
            {
                _IsBankrupted = value;
                if (value)
                {
                    BankruptEvent(this, new PlayerEventArgs("You are bankrupted. :("));
                }
            }
        }

        private Tuple<int, int> _Coordinate;
        public Tuple<int, int> Coordinate
        {
            get { return _Coordinate; }
            set
            {
                _Coordinate = value;
                PositionChangedEvent(this, new PlayerEventArgs("The player changed his/her position."));
            }
        }

        public Place Position
        {
            get
            {
                return Game.Map.Places[_Coordinate.Item1, _Coordinate.Item2];
            }
            set
            {
                _Coordinate = Game.Map.PlaceIdToCoordinate[value.Id];
            }
        }

        public Player(int id,XmlElement detail)
            : this(id)
        {
            XmlNode name = detail.SelectSingleNode("name");
            if (name != null)
            {
                Name = name.InnerText;
            }

            XmlNode coordinate = detail.SelectSingleNode("coordinate");
            if (coordinate != null)
            {
                int row = int.Parse(coordinate.SelectSingleNode("row").InnerText);
                int col = int.Parse(coordinate.SelectSingleNode("col").InnerText);
                _Coordinate = new Tuple<int, int>(row, col);
            }

            IsBankrupted = bool.Parse(detail.GetAttribute("bankrupt"));

            string direction = detail.GetAttribute("direction");
            if (direction == "cw")
            {
                Direction = Direction.CW;
            }
            else
            {
                Direction = Direction.CCW;
            }

            XmlNode properties = detail.SelectSingleNode("properties");
            if (properties != null)
            {
                int cash = int.Parse(properties.SelectSingleNode("cash").InnerText);
                int savings = int.Parse(properties.SelectSingleNode("savings").InnerText);
            }

        }

        public Player(int id, string name = null, Tuple<int, int> coordinate = null, int cash = 30000, int saving = 0)
        {
            Id = id;
            if (name != null)
            {
                Name = name;
            }
            else
            {
                Name = "Player";
            }
            Cash = cash;
            Saving = saving;
            IsBankrupted = false;
            Direction = Direction.CW;
            if (coordinate == null)
            {
                _Coordinate = Game.Map.PlaceIdToCoordinate[1];
            }
            else
            {
                _Coordinate = coordinate;
            }
        }

        public bool AddCash(int amount)
        {
            Cash += amount;
            AddCashEvent(this, new MoneyEventArgs("You have earned some cash.", amount));
            return true;
        }

        public bool SpendCash(int amount)
        {
            if (Cash >= amount)
            {
                Cash -= amount;
                SpendCashEvent(this, new MoneyEventArgs("You have lost some cash.", amount));
                return true;
            }
            else
            {
                Inform("You don't have enough cash. :(");
                return false;
            }
        }

        public bool AddSaving(int amount)
        {
            Saving += amount;
            AddSavingEvent(this, new MoneyEventArgs("You have earned some savings.", amount));
            return true;
        }

        public bool SpendSaving(int amount)
        {
            if (Saving >= amount)
            {
                SpendCashEvent(this, new MoneyEventArgs("You have lost some savings.", amount));
                Saving -= amount;
                return true;
            }
            else
            {
                Inform("You don't have enough saving. :(");
                return false;
            }
        }

        public virtual bool Deposit(int amount)
        {
            DepositEvent(this, new MoneyEventArgs("You are depositing your money.", amount));
            if (SpendCash(amount))
            {
                return AddSaving(amount);
            }
            else return false;
        }

        public virtual bool Withdraw(int amount)
        {
            WithdrawEvent(this, new MoneyEventArgs("You are withdrawing your money.", amount));
            if (SpendSaving(amount))
            {
                return AddCash(amount);
            }
            else
            {
                return false;
            }
        }

        public virtual void EarnInterest()
        {
            int amount = (int)(Saving * INTEREST_RATE);
            EarnInterestEvent(this, new MoneyEventArgs("You earned some interest because of your saving.", amount));
            AddSaving(amount);
        }

        public virtual void AddEstate(Estate e)
        {
            AddEstateEvent(this, new EstateEventArgs("Now you've got a new estate!", e));
        }

        public virtual void BuyEstate(Estate e)
        {
            if (HasEstate(e))
            {
                return;
            }
            if (!SpendCash(e.Price))
            {
                return;
            }
            else
            {
                e.Owner = this;
                BuyEstateEvent(this, new EstateEventArgs("Congratulations! You have bought this estate!", e));
            }
        }

        public virtual void RemoveEstate(Estate e)
        {
            if (HasEstate(e))
            {
                e.Owner = null;
                RemoveEstateEvent(this, new EstateEventArgs("You have lost the estate.", e));
            }
        }

        public virtual void SellEstate(Estate e)
        {
            if (HasEstate(e))
            {
                RemoveEstate(e);
                AddCash(e.Price);
                SellEstateEvent(this, new EstateEventArgs("You have sold the estate.", e));
            }

        }

        internal virtual void PayRent()
        {
            int rent = ((Estate)Position).Rent;
            PayRentEvent(this, new EstateEventArgs("You have to pay the rent.", (Estate)Position));
            if (!SpendCash(rent))
            {
                rent -= Cash;
                SpendCash(Cash);
                if (!SpendSaving(rent))
                {
                    IsBankrupted = true;
                }
            }
        }

        public virtual bool HasEstate(Estate e)
        {
            return this == e.Owner;
        }

        public virtual void RevertDirection()
        {
            if (Direction == Direction.CW)
            {
                Direction = Direction.CCW;
            }
            else
            {
                Direction = Direction.CW;
            }
            RevertDirectionEvent(this, new PlayerEventArgs("You reverted your direction."));
        }

        public virtual void Go(int steps = -1)
        {
            if (steps == -1)
            {
                steps = new Random().Next(6) + 1;
            }
            bool bankTriggered = false;
            GoingEvent(this, new PlayerEventArgs("You are going to move " + steps + " step(s)."));
            while (steps > 1)
            {
                Position = Position.Next(Direction);
                if (Position is Bank && !bankTriggered)
                {
                    Position.Trigger(this);
                    bankTriggered = true;
                }
                steps--;
            }
            if (!(Position is Bank && bankTriggered))
            {
                Position.Trigger(this);
            }

        }


        //basic IO methods
        public virtual void Inform(string word)
        {
            Game._Io.Write(word, this);
        }

        public virtual string Input(string cue)
        {
            return Game._Io.Read(cue, this);
        }

        public virtual int InputInt(string cue)
        {
            return Game._Io.ReadInt(cue, this);
        }

        public virtual int Choose(string[] choices)
        {
            return Game._Io.Choose(choices, this);
        }

        public virtual bool Confirm(string cue)
        {
            return Game._Io.Confirm(cue, this);
        }


    }
}
