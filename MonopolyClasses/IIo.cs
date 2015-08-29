using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Classes.Interfaces
{
    public interface IIo
    {
        //basic IO
        void Write(object item,Player target);
        string Read(string cue,Player target);
        int ReadInt(string cue,Player target);
        int Choose(string[] choices,Player target);
        bool Confirm(string cue,Player target);

        int GetSteps();
    }
}
