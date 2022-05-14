
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MT5SignalReceiver
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITradeService" in both code and config file together.
    [ServiceContract]
    public interface ITraderService
    {

        [OperationContract]
        void SendSignal(string strategyName, DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground);

        [OperationContract]
        void StrategyAddedOnChart(string strategyName,  string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground);

    }
}
