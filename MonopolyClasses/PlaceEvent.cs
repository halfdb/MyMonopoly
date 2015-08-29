using System;

namespace Monopoly.Classes
{
    public abstract partial class Place
    {
        public class PlayerArrivedEventArgs : EventArgs
        {
            public Player Player { get; private set; }

            internal PlayerArrivedEventArgs(Player player)
            {
                Player = player;
            }
        }

        public delegate void PlayerArrivedEventHandler<T>(T sender, PlayerArrivedEventArgs e) where T : Place;
    }

    public partial class Estate : Place
    {
        public event PlayerArrivedEventHandler<Estate> PlayerArrivedEvent = delegate { };

        public delegate void UpgradeEventHandler(Estate sender);
        public event UpgradeEventHandler UpgradeEvent = delegate { };
    }

    public partial class Bank : Place
    {
        public event PlayerArrivedEventHandler<Bank> PlayerArrivedEvent = delegate { };
    }

    public partial class Casino : Place
    {
        public event PlayerArrivedEventHandler<Casino> PlayerArrivedEvent = delegate { };

        public delegate void CasinoEventHandler(Player player,CasinoEventArgs e);
        public event CasinoEventHandler ResultEvent = delegate { };

        public class CasinoEventArgs : EventArgs
        {
            public bool Result;

            internal CasinoEventArgs(bool result)
            {
                Result = result;
            }
        }
    }
}
