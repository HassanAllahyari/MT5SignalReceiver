
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using MT5SignalReceiver.Helpers;
using MT5SignalReceiver.Models;

namespace MT5SignalReceiver
{
    public static class SignalReceiver
    {

        private static ObservableCollection<Strategy> _Strategys = new ObservableCollection<Strategy>();

        public static ObservableCollection<Strategy> Strategys
        {
            get { return _Strategys; }
        }

        public static event NewSignalRecivedEventHandler NewSignalRecived;
        public static ObservableCollection<SignalItem> SignalList = new ObservableCollection<SignalItem>();

        public static ChartAsset StrategyAddedOnChart(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath,long chartForeground)
        {
            var strategy = AddStrategyIfNotExist(strategyName);

            Brush foreground = ImageHelper.LongToBrush(chartForeground);

            return strategy.AddNewChartAssetIfNotExist(asset, assetDescripion, timeFrame, screenShotFilePath, foreground);
        }

        public static void AddNewSignal(string strategyName, DateTime date, string asset, string assetDescripion, string timeFrame, TradeAction tradeAction, string screenShotFilePath, long chartForeground)
        {

            asset = asset.ToUpper();


            var chartasset = StrategyAddedOnChart(strategyName, asset, assetDescripion, timeFrame, screenShotFilePath,chartForeground);

            if (!chartasset.AllowToReciveSignal) return;

            chartasset.NewSignalAdded();

            var item = AddSignal(strategyName, date, chartasset, tradeAction);

            if (NewSignalRecived != null)
                NewSignalRecived.Invoke(new NewSignalRecivedEventArgs(item));
        }
        private static SignalItem AddSignal(string strategyName, DateTime date, ChartAsset chartAsset, TradeAction tradeAction)
        {
            var item = new SignalItem(chartAsset)
            {
                StrategyName = strategyName,
                Date = date,
                TradeAction = tradeAction,
            };

            SignalList.Add(item);
            return item;
        }
        public static SignalItem AddNewSignalManually(string strategyName, ChartAsset chartAsset)
        {
            chartAsset.NewSignalAdded();
            return AddSignal(strategyName, DateTime.Now, chartAsset, TradeAction.Call);
        }
        public static Strategy AddStrategyIfNotExist(string strategyName)
        {
            var strategy = Strategys.FirstOrDefault(n => n.Name == strategyName);

            if (strategy == null)
            {
                strategy = new Strategy() { Name = strategyName };
                Strategys.Add(strategy);
            }
            return strategy;
        }


    }
}
