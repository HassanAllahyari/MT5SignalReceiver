using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace MT5SignalReceiver.WCF
{
    public class TraderServiceHost
    {
        private static bool _HostOpened;
        private static ServiceHost _Host;
        public static void Open()
        {
            if (_HostOpened) return;

            if (_Host == null)
                _Host = new ServiceHost(typeof(TraderService));

            try
            {
                _Host.Open();
                _HostOpened = true;
            }
            catch
            {

            }
        }

    }
}
