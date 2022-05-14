using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MT5SignalReceiver.Models;

namespace MT5SignalReceiver
{
    public delegate void NewSignalRecivedEventHandler(NewSignalRecivedEventArgs e);
    public class NewSignalRecivedEventArgs
    {
        public SignalItem SignalItem { get; private set; }

        public NewSignalRecivedEventArgs(SignalItem item)
        {
            this.SignalItem = item;
        }
    }
}
