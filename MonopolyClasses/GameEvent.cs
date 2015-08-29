using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Classes
{
    public sealed partial class Game
    {
        public delegate void GameEventHandler(GameEventArgs e);
        public static event GameEventHandler AllGameEvents
        {
            add
            {
                PlayerChangedEvent += value;
                StageChangedEvent += value;
                WinEvent += value;
            }
            remove
            {
                PlayerChangedEvent -= value;
                StageChangedEvent -= value;
                WinEvent -= value;
            }
        }
        public static event GameEventHandler PlayerChangedEvent = delegate { };
        public static event GameEventHandler StageChangedEvent = delegate { };
        public static event GameEventHandler WinEvent = delegate { };

        public class GameEventArgs : EventArgs
        {
            public string Info { get; set; }

            public GameEventArgs(string info)
            {
                Info = info;
            }
        }
    }
}
