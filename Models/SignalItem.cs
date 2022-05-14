using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace MT5SignalReceiver.Models
{

    public enum TradeAction
    {
        Call = 1,
        Put = -1,
    }

    public class SignalItem : ModelBase<SignalItem>
    {
        private Timer _Timer;
        public SignalItem(ChartAsset chartAsset)
        {
            ElapsedTime = new TimeSpan();

            _Timer = new Timer(1000);
            _Timer.Elapsed += _Timer_Elapsed;
            _Timer.Start();

            _ChartAsset = chartAsset;
            _ChartAsset.IsViewingChanged += _ChartAsset_IsViewingChanged;
        }

        private void _ChartAsset_IsViewingChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged(n => n.Background);
        }




        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ElapsedTime += TimeSpan.FromSeconds(1);
        }


        public Brush Background
        {
            get
            {
                return ChartAsset.IsViewing ? Brushes.Green : Brushes.Transparent;
            }
        }
        private string _StrategyName;
        public string StrategyName
        {
            get
            {
                return _StrategyName;
            }
            set
            {
                if (_StrategyName != value)
                {
                    _StrategyName = value;
                    NotifyPropertyChanged(n => n.StrategyName);
                }
            }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    NotifyPropertyChanged(n => n.Date);
                }
            }
        }
        private TimeSpan _ElapsedTime;
        public TimeSpan ElapsedTime
        {
            get
            {
                return _ElapsedTime;
            }
            set
            {
                if (_ElapsedTime != value)
                {
                    _ElapsedTime = value;
                    NotifyPropertyChanged(n => n.ElapsedTime);

                }
            }
        }

        private ChartAsset _ChartAsset;
        public ChartAsset ChartAsset
        {
            get
            {
                return _ChartAsset;
            }

        }
       
       
        private TradeAction _TradeAction;
        public TradeAction TradeAction
        {
            get
            {
                return _TradeAction;
            }
            set
            {
                if (_TradeAction != value)
                {
                    _TradeAction = value;
                    NotifyPropertyChanged(n => n.TradeAction);
                }
            }
        }
    }
}
