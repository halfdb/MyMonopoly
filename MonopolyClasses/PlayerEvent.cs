using System;
using System.ComponentModel;
using System.Globalization;

namespace Monopoly.Classes
{
    public partial class Player
    {
        public delegate void PlayerEventHandler<T>(Player player, T e) where T : PlayerEventArgs;

        public event PlayerEventHandler<PlayerEventArgs> GoingEvent = delegate { };
        public event PlayerEventHandler<PlayerEventArgs> PositionChangedEvent = delegate { };
        public event PlayerEventHandler<PlayerEventArgs> RevertDirectionEvent = delegate { };
        public event PlayerEventHandler<PlayerEventArgs> BankruptEvent = delegate { };
        
        public event PlayerEventHandler<MoneyEventArgs> AllMoneyEvent
        {
            add
            {
                AddCashEvent += value;
                SpendCashEvent += value;
                AddSavingEvent += value;
                SpendSavingEvent += value;
                DepositEvent += value;
                WithdrawEvent += value;
                EarnInterestEvent += value;
            }

            remove
            {
                AddCashEvent -= value;
                SpendCashEvent -= value;
                AddSavingEvent -= value;
                SpendSavingEvent -= value;
                DepositEvent -= value;
                WithdrawEvent -= value;
                EarnInterestEvent -= value;
            }
        }
        public event PlayerEventHandler<MoneyEventArgs> AddCashEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> SpendCashEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> AddSavingEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> SpendSavingEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> DepositEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> WithdrawEvent = delegate { };
        public event PlayerEventHandler<MoneyEventArgs> EarnInterestEvent = delegate { };

        public event PlayerEventHandler<EstateEventArgs> AllEstateEvent
        {
            add
            {
                AddEstateEvent += value;
                RemoveEstateEvent += value;
                BuyEstateEvent += value;
                SellEstateEvent += value;
                PayRentEvent += value;
            }

            remove
            {
                AddEstateEvent -= value;
                RemoveEstateEvent -= value;
                BuyEstateEvent -= value;
                SellEstateEvent -= value;
                PayRentEvent -= value;
            }
        }
        public event PlayerEventHandler<EstateEventArgs> AddEstateEvent = delegate { };
        public event PlayerEventHandler<EstateEventArgs> RemoveEstateEvent = delegate { };
        public event PlayerEventHandler<EstateEventArgs> BuyEstateEvent = delegate { };
        public event PlayerEventHandler<EstateEventArgs> SellEstateEvent = delegate { };
        public event PlayerEventHandler<EstateEventArgs> PayRentEvent = delegate { };


        public class PlayerEventArgs : EventArgs
        {
            public string Info { get; private set; }
            
            public PlayerEventArgs(string info)
            {
                Info = info;
            }
        }

        public class MoneyEventArgs : PlayerEventArgs
        {
            public int Amount { get; private set; }
            public MoneyEventArgs(string info, int amount)
                : base(info)
            {
                Amount = amount;
            }
        }

        public class EstateEventArgs : PlayerEventArgs
        {
            public Estate Target { get; private set; }
            public EstateEventArgs(string info, Estate target)
                : base(info)
            {
                Target = target;
            }
        }
        
    }
}
