using System;
using SimpleMvvmToolkit;
using Brush = System.Windows.Media.Brush;

namespace MT5SignalReceiver.Models
{

    public class ChartAsset : ModelBase<ChartAsset>
    {

        public ChartAsset(string name, string descripion, string timeFrame, string screenShotFilePath,Brush chartForeground)
        {
            _Name = name;
            _Descripion = descripion;
            _TimeFrame = timeFrame;
            _ScreenShotFilePath = screenShotFilePath;
            _ChartForeground = chartForeground;
            _AllowToReciveSignal = true;
        }

        public void NewSignalAdded()
        {
            SignalCount++;
        }
        public void SignalRemoved()
        {
            SignalCount--;
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}\n{2}", _Name, _TimeFrame, _Descripion);
        }




        private string _ScreenShotFilePath;
        public string ScreenShotFilePath
        {
            get
            {
                return _ScreenShotFilePath;
            }
            set
            {
                if (_ScreenShotFilePath != value)
                {
                    _ScreenShotFilePath = value;
                    NotifyPropertyChanged(n => n.ScreenShotFilePath);
                }
            }
        }
        public event EventHandler IsViewingChanged;
        private bool _IsViewing;
        public bool IsViewing
        {
            get
            {
                return _IsViewing;
            }
            set
            {
                if (_IsViewing != value)
                {
                    _IsViewing = value;
                    NotifyPropertyChanged(n => n._IsViewing);
                    if (IsViewingChanged != null)
                        IsViewingChanged(this, EventArgs.Empty);
                }
            }
        }
        private int _SignalCount;
        public int SignalCount
        {
            get
            {
                return _SignalCount;
            }
            private set
            {
                if (_SignalCount != value)
                {
                    _SignalCount = value;
                    NotifyPropertyChanged(n => n.SignalCount);
                }
            }
        }

        private string _TimeFrame;
        public string TimeFrame
        {
            get
            {
                return _TimeFrame;
            }
            set
            {
                if (_TimeFrame != value)
                {
                    _TimeFrame = value;
                    NotifyPropertyChanged(n => n._TimeFrame);
                }
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged(n => n.Name);
                }
            }
        }
        private string _Descripion;
        public string Descripion
        {
            get
            {
                return _Descripion;
            }
            set
            {
                if (_Descripion != value)
                {
                    _Descripion = value;
                    NotifyPropertyChanged(n => n.Descripion);
                }
            }
        }

        private Brush _ChartForeground;
        public Brush ChartForeground
        {
            get
            {
                return _ChartForeground;
            }
            set
            {
                if (_ChartForeground != value)
                {
                    _ChartForeground = value;
                    NotifyPropertyChanged(n => n.ChartForeground);
                }
            }
        }
        public event EventHandler AllowToReciveSignalChanged;
        private bool _AllowToReciveSignal;
        public bool AllowToReciveSignal
        {
            get
            {
                return _AllowToReciveSignal;
            }
            set
            {
                if (_AllowToReciveSignal != value)
                {
                    _AllowToReciveSignal = value;
                    NotifyPropertyChanged(n => n.AllowToReciveSignal);
                    if (AllowToReciveSignalChanged != null)
                        AllowToReciveSignalChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
