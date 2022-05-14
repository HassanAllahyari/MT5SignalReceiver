using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5SignalReceiver
{
    public delegate void WebBrowserStatusChangedEventHandler(object sender, WebBrowserStatusChangedEventEventArgs e);
    public class WebBrowserStatusChangedEventEventArgs
    {
        public WebBrowserStatus Status { get; private set; }
        public WebBrowserStatusChangedEventEventArgs(WebBrowserStatus status)
        {
            Status = status;
        }
    }
}
