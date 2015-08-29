using Monopoly.Classes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Monopoly.Classes
{
    public enum GameStage
    {
        NotInitialized,
        Start,
        InGame,
        Finish,
        Exception
    }

    public sealed partial class Game
    {
        internal static IIo _Io { get; private set; }
        public static Map Map { get; private set; }
        private static GameStage _Stage;
        public static GameStage Stage
        {
            get
            {
                return _Stage;
            }
            private set
            {
                _Stage = value;
                StageChangedEvent(new GameEventArgs("The stage has been changed..."));
            }
        }
        public static Player[] Players { get; private set; }
        private static int _CurrentPlayerId;
        public static Player CurrentPlayer
        {
            get
            {
                return Players[_CurrentPlayerId];
            }
            set
            {
                _CurrentPlayerId = value.Id;
                PlayerChangedEvent(new GameEventArgs("Now it's another player's turn..."));
            }
        }
        private static int _Day;
        public static int Day
        {
            get
            {
                return _Day;
            }
            set
            {
                _Day = value;
                if (_Day % 7 == 0)
                {
                    foreach (Player item in Players)
                    {
                        item.EarnInterest();
                    }
                }
            }
        }


        static Game()
        {
            _Stage = GameStage.NotInitialized;
        }

        [Serializable]
        public class BadStageException : Exception
        {
            public GameStage Required { get; private set; }
            public GameStage Current { get; private set; }

            public BadStageException(GameStage req, GameStage cur)
            {
                Required = req;
                Current = cur;
            }
        }

        private static void VerifyStage(GameStage required)
        {
            if (Stage != required)
            {
                throw new BadStageException(required, Stage);
            }
        }

        public static void Initialize(IIo io, XmlElement mapInfo, XmlElement playerInfo)
        {
            VerifyStage(GameStage.NotInitialized);

            _Io = io;

            Map = new Map(mapInfo);

            //Players = new Player[playerCount];
            //for (int i = 0; i < playerCount; i++)
            //{
            //    Players[i] = new Player(i);
            //}

            _CurrentPlayerId = 0;

            Day = 1;

            Stage = GameStage.Start;
        }

        public static void Start()
        {
            VerifyStage(GameStage.Start);

            Stage = GameStage.InGame;

            while (Day <= 200)
            {

                CurrentPlayer.Go(_Io.GetSteps());
                int prevId = CurrentPlayer.Id;
                SetNextPlayer();
                if (prevId == CurrentPlayer.Id)
                {
                    WinEvent(new GameEventArgs("A player has winned the game!!"));
                    break;
                }
            }

            Stage = GameStage.Finish;
        }

        private static void SetNextPlayer()
        {
            int target = CurrentPlayer.Id + 1;
            if (target == Players.Length)
            {
                target = 0;
                Day++;
            }
            while (Players[target].IsBankrupted)
            {
                target++;
                if (target == Players.Length)
                {
                    target = 0;
                    Day++;
                }
            }
            CurrentPlayer = Players[target];
        }
    }
}
