using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GalaSoft.MvvmLight;

namespace MyTrader
{

    public enum AssetMarket
    {
        Unknown = 0,
        Currency = 1,
        Stock = 2,
    }

    public class Asset : ObservableObject
    {

        public Asset()
        {

        }




        private bool _Active;
        public bool Active
        {
            get
            {
                return _Active;
            }
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    RaisePropertyChanged("Active");
                }
            }
        }

        private AssetMarket _Market;
        public AssetMarket Market
        {
            get
            {
                return _Market;
            }
            set
            {
                if (_Market != value)
                {
                    _Market = value;
                    RaisePropertyChanged("Market");
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
                    RaisePropertyChanged("Name");
                }
            }
        }

        private double _PayoutFrom;
        public double PayoutFrom
        {
            get
            {
                return _PayoutFrom;
            }
            set
            {
                if (_PayoutFrom != value)
                {
                    _PayoutFrom = value;
                    RaisePropertyChanged("PayoutFrom");
                    RaisePropertyChanged("PayoutText");
                }
            }
        }
        private double _PayoutTo;
        public double PayoutTo
        {
            get
            {
                return _PayoutTo;
            }
            set
            {
                if (_PayoutTo != value)
                {
                    _PayoutTo = value;
                    RaisePropertyChanged("PayoutTo");
                    RaisePropertyChanged("PayoutText");
                }
            }
        }

        public string PayoutText
        {
            get
            {
                return PayoutFrom != PayoutTo ? string.Format("{0}-{1}%", PayoutFrom, PayoutTo) :
                new DoubleToPercentageConverter().Convert(PayoutTo, null, null, null).ToString(); ;
            }

        }

       


    }
}
