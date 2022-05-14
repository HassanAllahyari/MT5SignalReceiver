
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MT5SignalReceiver.Models;

namespace MT5SignalReceiver
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TradeService" in both code and config file together.
    public class TraderService : ITraderService
    {
 
        public void SendSignal(string strategyName, DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground)
        {
            SignalReceiver.AddNewSignal(strategyName, date, asset, assetDescripion, timeFrame, isCall ? TradeAction.Call : TradeAction.Put, screenShotFilePath, chartForeground);
        }

        public void StrategyAddedOnChart(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground)
        {
            SignalReceiver.StrategyAddedOnChart(strategyName, asset, assetDescripion, timeFrame, screenShotFilePath, chartForeground);

        }
    }
}
