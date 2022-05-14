
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using SimpleMvvmToolkit;

namespace MT5SignalReceiver.Models
{



    public class Strategy : ModelBase<Strategy>
    {

        public Strategy()
        {

        }

        public override string ToString()
        {
            return Name;
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

        private ObservableCollection<ChartAsset> _ChartAssets = new ObservableCollection<ChartAsset>();
        public ObservableCollection<ChartAsset> ChartAssets
        {
            get { return _ChartAssets; }
        }
        private Dictionary<string, int> _ChartAssetJoiner = new Dictionary<string, int>();

        public ChartAsset GetChartAsset(string name, string timeFrame)
        {
            ChartAsset item = null;
            try
            {
                item = ChartAssets[_ChartAssetJoiner[GetAssetKey(name, timeFrame)]];
            }
            catch { }

            return item;
        }

        public ChartAsset AddNewChartAssetIfNotExist(string asset, string assetDescripion, string timeFrame, string screenShotFilePath, Brush chartForeground)
        {
            var item = GetChartAsset(asset, timeFrame);
            if (item == null)
            {
                item = new ChartAsset(asset, assetDescripion, timeFrame, screenShotFilePath, chartForeground);
                ChartAssets.Add(item);
                _ChartAssetJoiner.Add(GetAssetKey(asset, timeFrame), ChartAssets.Count - 1);

            }
            else
                item.ChartForeground = chartForeground;

            return item;
        }

        private string GetAssetKey(string asset, string timeFrame)
        {
            return string.Format("{0}{1}", asset, timeFrame);
        }
    }
}
