
using MT5SignalSender.ServiceReference;
using System;
using System.ServiceModel;
using System.Threading.Tasks;


namespace MT5SignalSender
{
    public static class SignalSender
    {
        public static void Send(string strategyName, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground)
        {
            Task.Run(() =>
            {
                try
                {
                    var _Service = CreateService();
                    _Service.Open();

                    _Service.SendSignal(strategyName, DateTime.Now, asset, assetDescripion, timeFrame, isCall, screenShotFilePath, chartForeground);
                    _Service.Close();

                }
                catch (Exception e)
                {
                }
            });
        }

        public static void StrategyAddedOnChart(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground)
        {
            Task.Run(() =>
            {
                try
                {
                    var _Service = CreateService();
                    _Service.Open();

                    _Service.StrategyAddedOnChart(strategyName, asset, assetDescripion, timeFrame, screenShotFilePath, chartForeground);
                    _Service.Close();
                }
                catch (Exception ex)
                {
                }
            });
        }

        private static TraderServiceClient CreateService()
        {
            var endpoint = new EndpointAddress("http://localhost:4321/MT5SignalReceiver/Service/");
            BasicHttpBinding binding = new BasicHttpBinding();

            binding.Name = "BasicHttpBinding_ITraderService";

            return new TraderServiceClient(binding, endpoint);
        }

    }
}
