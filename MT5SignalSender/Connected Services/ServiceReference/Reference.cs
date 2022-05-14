﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MT5SignalSender.ServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference.ITraderService")]
    public interface ITraderService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITraderService/SendSignal", ReplyAction="http://tempuri.org/ITraderService/SendSignalResponse")]
        void SendSignal(string strategyName, System.DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITraderService/SendSignal", ReplyAction="http://tempuri.org/ITraderService/SendSignalResponse")]
        System.Threading.Tasks.Task SendSignalAsync(string strategyName, System.DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITraderService/StrategyAddedOnChart", ReplyAction="http://tempuri.org/ITraderService/StrategyAddedOnChartResponse")]
        void StrategyAddedOnChart(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITraderService/StrategyAddedOnChart", ReplyAction="http://tempuri.org/ITraderService/StrategyAddedOnChartResponse")]
        System.Threading.Tasks.Task StrategyAddedOnChartAsync(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITraderServiceChannel : MT5SignalSender.ServiceReference.ITraderService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TraderServiceClient : System.ServiceModel.ClientBase<MT5SignalSender.ServiceReference.ITraderService>, MT5SignalSender.ServiceReference.ITraderService {
        
        public TraderServiceClient() {
        }
        
        public TraderServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TraderServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TraderServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TraderServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void SendSignal(string strategyName, System.DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground) {
            base.Channel.SendSignal(strategyName, date, asset, assetDescripion, timeFrame, isCall, screenShotFilePath, chartForeground);
        }
        
        public System.Threading.Tasks.Task SendSignalAsync(string strategyName, System.DateTime date, string asset, string assetDescripion, string timeFrame, bool isCall, string screenShotFilePath, long chartForeground) {
            return base.Channel.SendSignalAsync(strategyName, date, asset, assetDescripion, timeFrame, isCall, screenShotFilePath, chartForeground);
        }
        
        public void StrategyAddedOnChart(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground) {
            base.Channel.StrategyAddedOnChart(strategyName, asset, assetDescripion, timeFrame, screenShotFilePath, chartForeground);
        }
        
        public System.Threading.Tasks.Task StrategyAddedOnChartAsync(string strategyName, string asset, string assetDescripion, string timeFrame, string screenShotFilePath, long chartForeground) {
            return base.Channel.StrategyAddedOnChartAsync(strategyName, asset, assetDescripion, timeFrame, screenShotFilePath, chartForeground);
        }
    }
}
