using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Monopoly.Classes
{
    class Ai : Player
    {

        protected Queue<bool> ConfirmResponses { get; set; }
        public override bool Confirm(string cue)
        {
            return ConfirmResponses.Dequeue();
        }
        protected Queue<int> ChooseResponses { get; set; }
        public override int Choose(string[] choices)
        {
            return ChooseResponses.Dequeue();
        }
        protected Queue<int> InputIntResponses { get; set; }
        public override int InputInt(string cue)
        {
            return InputIntResponses.Dequeue();
        }

        public Ai(int id)
            : base(id)
        {
            foreach (Place item in Game.Map.Places)
            {
                if (item is Estate)
                {
                    (item as Estate).PlayerArrivedEvent += PlayerArrivedEventHandler;
                }
                else if (item is Bank)
                {
                    (item as Bank).PlayerArrivedEvent += PlayerArrivedEventHandler;
                }
                else if (item is Casino)
                {
                    (item as Casino).PlayerArrivedEvent += PlayerArrivedEventHandler;
                }
            }
        }

        public Ai(int id, XmlElement detail)
            : base(id, detail)
        {

        }

        private void PlayerArrivedEventHandler(Casino sender, Place.PlayerArrivedEventArgs e)
        {
            ConfirmResponses.Enqueue(false);
        }

        private void PlayerArrivedEventHandler(Bank sender, Place.PlayerArrivedEventArgs e)
        {
            int amount = (Saving - 2 * Cash) / 3;
            if (amount != 0)
            {
                InputIntResponses.Enqueue(Math.Abs(amount));
                if (amount > 0) //saving > cash*2, withdraw
                {
                    ChooseResponses.Enqueue(2);//2.Withdraw
                }
                else
                {
                    ChooseResponses.Enqueue(1);//1.Deposit
                }
            }
            ChooseResponses.Enqueue(3);//3.Leave
        }

        private void PlayerArrivedEventHandler(Estate sender, Place.PlayerArrivedEventArgs e)
        {
            if (sender.Owner == this)
            {
                if (Cash >= sender.Price * 2 && sender.Level < 6)
                {
                    ConfirmResponses.Enqueue(true);
                }
                else if (Cash < sender.Price * 2)
                {
                    ConfirmResponses.Enqueue(false);
                }
            }
            else if (sender.Owner == null)
            {
                if (Cash >= sender.Price * 1.8)
                {
                    ConfirmResponses.Enqueue(true);
                }
                else
                {
                    ConfirmResponses.Enqueue(false);
                }
            }
        }
    }
}
