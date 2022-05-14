
using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using MT5SignalReceiver.Models;
using MT5SignalReceiver.WCF;
using Brush = System.Windows.Media.Brush;
using Timer = System.Timers.Timer;
using MT5SignalReceiver.Properties;

namespace MT5SignalReceiver.ViewModels
{


    public class MainViewModel : ViewModelBase<MainViewModel>
    {
        private MediaPlayer _NotificationSoundPlayer = new MediaPlayer();
        private Uri _SignalSoundUri;
        private Uri _MessageSoundUri;

        private Timer _StatusLoadingTimer = new Timer();
        private Timer _StatusElapsedTimeToShowTimer = new Timer();
        private Timer _NewSignalTimer = new Timer();
        private int _ElapsedTimeToShow;
        private int _ElapsedTimeNewSignal;

        private int _StatusDotCount = 1;


        private Brush _NormalMessageColor = new SolidColorBrush(Colors.White);
        private Brush _SuccesMessageColor = new SolidColorBrush(Colors.Green);
        private Brush _UnSuccesMessageColor = new SolidColorBrush(Colors.Red);

        private string _ScreenShotFilePath;
        private Thread _DrawChartThread;
        private Timer _DrawChartTimer;
        private bool _ChartAssetInstalized;
        private double _ChartRefreshRate = 5000.0;


        private string _StartReceivingSignalText = "Start receiving signal";
        private string _StopReceivingSignalText = "Stop receiving signal";
        private string _ReceivingSignalStartedMessage = "Receiving signal started.";
        private string _ReceivingSignalStopedMessage = "Receiving signal stoped.";

        private string _StatusLoadingText = "Loading web page";
        private string _StatusReadyMessage = "Ready.";

        private ImageSource _StartedReceivingSignalIconSource = new MemoryBitmapSource(Resources.stop).Source;
        private ImageSource _StopedReceivingSignalIconSource = new MemoryBitmapSource(Resources.start).Source;
        public MainViewModel()
        {

            _StatusLoadingTimer.Interval = 500;
            _StatusLoadingTimer.Elapsed += (s, e) =>
            {
                if (!Status.StartsWith(_StatusLoadingText)) return;

                if (_StatusDotCount > 2)
                    _StatusDotCount = 0;
                else
                    _StatusDotCount++;

                var dot = "";
                for (int i = 0; i < _StatusDotCount; i++)
                    dot += ". ";

                Status = string.Format("{0}{1}", _StatusLoadingText, dot);
            };

            _StatusElapsedTimeToShowTimer.Interval = 1000;
            _StatusElapsedTimeToShowTimer.Elapsed += (s, e) =>
              {



                  _ElapsedTimeToShow -= 1;
                  if (_ElapsedTimeToShow == 0)
                  {
                      StatusElapsedTimeToShow = "";
                      _StatusElapsedTimeToShowTimer.Stop();
                      _MainWindow.Dispatcher.Invoke(() => CheckStatus());
                  }
                  else
                      StatusElapsedTimeToShow = string.Format("({0})", _ElapsedTimeToShow);
              };

            _NewSignalTimer.Interval = 2000;
            _NewSignalTimer.Elapsed += (s, e) =>
            {
                _ElapsedTimeNewSignal += 2;
                if (_ElapsedTimeNewSignal == 60)
                {
                    _NewSignalTimer.Stop();
                    _MainWindow.Dispatcher.Invoke((Action)(() =>
                    {
                        _MainWindow.TaskbarItemInfo.ProgressValue = 1;
                        _MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
                    }));
                }
                else
                    _MainWindow.Dispatcher.Invoke(() => _MainWindow.TaskbarItemInfo.ProgressValue = _ElapsedTimeNewSignal / 60.0);
            };

            var soundsdir = Path.Combine(AppContext.BaseDirectory, @"Sounds\");

            _SignalSoundUri = new Uri(Path.Combine(soundsdir, "Notification.mp3"));
            _MessageSoundUri = new Uri(Path.Combine(soundsdir, "Message.mp3"));

            SignalReceiver.NewSignalRecived += Signals_NewSignalRecived;

            SignalReceiver.SignalList.CollectionChanged += SignalList_CollectionChanged;

            SignalReceiver.Strategys.CollectionChanged += Strategys_CollectionChanged;

            StartText = _StopReceivingSignalText;
            StartIconSource = _StartedReceivingSignalIconSource;

            TraderServiceHost.Open();

        }



        private void Strategys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SignalReceiver.Strategys.CollectionChanged -= Strategys_CollectionChanged;
            if (SelectedChartAsset == null && SignalReceiver.Strategys.Count > 0)
            {
                if (SignalReceiver.Strategys.First().ChartAssets.Count > 0)
                    SelectFirstChartAsset();
                else
                    SignalReceiver.Strategys.First().ChartAssets.CollectionChanged += ChartAssets_CollectionChanged;
            }
        }

        private void ChartAssets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var chartassets = SignalReceiver.Strategys.First().ChartAssets;
            chartassets.CollectionChanged -= ChartAssets_CollectionChanged;
            SelectFirstChartAsset();
        }
        private void SelectFirstChartAsset()
        {
            var chartassets = SignalReceiver.Strategys.First().ChartAssets;

            if (SelectedChartAsset == null && chartassets.Count > 0)
                SelectedStrategy = SignalReceiver.Strategys.First();

        }


        private void SignalList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ClearItemsCommand.RaiseCanExecuteChanged();
        }

        private void Signals_NewSignalRecived(NewSignalRecivedEventArgs e)
        {
            _NotificationSoundPlayer.Open(_SignalSoundUri);
            _NotificationSoundPlayer.Play();
            _MainWindow.Dispatcher.Invoke(() =>
            {
                _MainWindow.TaskbarItemInfo.ProgressValue = 0;

                _MainWindow.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;

            });

            if (_NewSignalTimer.Enabled)
                _NewSignalTimer.Stop();

            _ElapsedTimeNewSignal = 0;
            _NewSignalTimer.Start();

            ChangeStatus(string.Format("New signal received [{0},{1},{2} {3}]", e.SignalItem.StrategyName,
                e.SignalItem.ChartAsset.Name, e.SignalItem.ChartAsset.TimeFrame, e.SignalItem.TradeAction), true, true);

        }


        private void ChangeStatus(string message, bool succes, bool mute = false)
        {
            if (!mute)
            {
                _NotificationSoundPlayer.Open(_MessageSoundUri);
                _NotificationSoundPlayer.Play();
            }
            Status = message;
            StatusColor = succes ? _SuccesMessageColor : _UnSuccesMessageColor;

            StatusElapsedTimeToShow = "(10)";
            _ElapsedTimeToShow = 10;
            _StatusElapsedTimeToShowTimer.Start();

        }
        private void CheckStatus()
        {
            if (_StatusElapsedTimeToShowTimer.Enabled) return;
            StatusElapsedTimeToShow = "";
            StatusColor = _NormalMessageColor;
            if (_BrokerWebBrowser.Status == WebBrowserStatus.None || _BrokerWebBrowser.Status == WebBrowserStatus.Ready)
            {
                _StatusLoadingTimer.Stop();
                Status = _StatusReadyMessage;
            }
            else
            {
                _StatusLoadingTimer.Start();
                Status = _StatusLoadingText;

            }
        }


        private MainWindow _MainWindow;
        public MainWindow MainWindow
        {
            get { return _MainWindow; }
            internal set
            {
                _MainWindow = value;
                _MainWindow.TaskbarItemInfo = new TaskbarItemInfo();
                _MainWindow.Closing += _MainWindow_Closing;
                _MainWindow.Loaded += _MainWindow_Loaded;
            }
        }
        private void _MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _NewSignalTimer.Stop();
            _StatusElapsedTimeToShowTimer.Stop();
            _StatusLoadingTimer.Stop();
            if (_DrawChartTimer != null)
                _DrawChartTimer.Stop();

        }
        private void _MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                HwndSource.FromHwnd(new WindowInteropHelper(_MainWindow).Handle)?.AddHook(this.WndProc);
            }
            catch { }
        }
        private const int WM_LBUTTONUP = 0x0202;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_LBUTTONUP)
                _MainWindow.ChartViewer.MouseUpPreesed();

            return IntPtr.Zero;
        }

        private WebBrowser _BrokerWebBrowser;
        public WebBrowser BrokerWebBrowser
        {
            get { return _BrokerWebBrowser; }
            internal set
            {
                _BrokerWebBrowser = value;
                _BrokerWebBrowser.WebBrowserStatusChanged += _BrokerWebBrowser_WebBrowserStatusChanged;

            }
        }

        private void _BrokerWebBrowser_WebBrowserStatusChanged(object sender, WebBrowserStatusChangedEventEventArgs e)
        {
            CheckStatus();
        }



        private DelegateCommand _StartCommand;
        public DelegateCommand StartCommand
        {
            get
            {
                return _StartCommand
                    ?? (_StartCommand = new DelegateCommand(() =>
                    {
                        if (StartText == _StartReceivingSignalText)
                        {


                            foreach (var item in ChartAssets)
                            {
                                item.AllowToReciveSignalChanged -= ChartAsset_AllowToReciveSignalChanged;
                                item.AllowToReciveSignal = true;
                                item.AllowToReciveSignalChanged += ChartAsset_AllowToReciveSignalChanged;
                            }
                            StartReceivingSignal();
                        }
                        else
                        {

                            foreach (var item in ChartAssets)
                            {
                                item.AllowToReciveSignalChanged -= ChartAsset_AllowToReciveSignalChanged;
                                item.AllowToReciveSignal = false;
                                item.AllowToReciveSignalChanged += ChartAsset_AllowToReciveSignalChanged;
                            }

                            StopReceivingSignal();

                        }
                    }));
            }
        }

        private void StartReceivingSignal()
        {
            StartText = _StopReceivingSignalText;
            StartIconSource = _StartedReceivingSignalIconSource;
            ChangeStatus(_ReceivingSignalStartedMessage, true);
        }
        private void StopReceivingSignal()
        {
            StartText = _StartReceivingSignalText;
            StartIconSource = _StopedReceivingSignalIconSource;
            ChangeStatus(_ReceivingSignalStopedMessage, false);
        }
        private void ChartAsset_AllowToReciveSignalChanged(object sender, EventArgs e)
        {
            var item = sender as ChartAsset;
            if (item.AllowToReciveSignal)
            {
                if (StartText == _StartReceivingSignalText)
                    StartReceivingSignal();
            }
            else if (ChartAssets.FirstOrDefault(n => n.AllowToReciveSignal) == null)
                if (StartText == _StopReceivingSignalText)
                    StopReceivingSignal();

        }


        private DelegateCommand<SignalItem> _RemoveItemCommand;
        public DelegateCommand<SignalItem> RemoveItemCommand
        {
            get
            {
                return _RemoveItemCommand
                    ?? (_RemoveItemCommand = new DelegateCommand<SignalItem>(
                     itemcase =>
                     {
                         RemoveAssetItem(itemcase);

                     }));
            }
        }
        private DelegateCommand _ClearItemsCommand;
        public DelegateCommand ClearItemsCommand
        {
            get
            {
                return _ClearItemsCommand
                    ?? (_ClearItemsCommand = new DelegateCommand(() =>
                    {

                        while (SignalReceiver.SignalList.Count > 0)
                        {
                            var item = SignalReceiver.SignalList.FirstOrDefault();
                            if (item != null)
                                RemoveAssetItem(item);
                        }


                    }, () => SignalReceiver.SignalList.Count > 0));
            }
        }



        private void RemoveAssetItem(SignalItem item)
        {

            item.ChartAsset.SignalRemoved();
            SignalReceiver.SignalList.Remove(item);


        }




        private DelegateCommand<SignalItem> _ViewChartCommand;
        public DelegateCommand<SignalItem> ViewChartCommand
        {
            get
            {
                return _ViewChartCommand
                    ?? (_ViewChartCommand = new DelegateCommand<SignalItem>(
                    item =>
                    {
                        SelectedSignal = item;

                        SelectedStrategy = Strategys.FirstOrDefault(n => n.Name == item.StrategyName);
                        if (SelectedStrategy != null)
                            SelectedChartAsset = item.ChartAsset;

                    },
                    item =>
                    {
                        if (item.StrategyName == SelectedStrategy.Name && item.ChartAsset == SelectedChartAsset)
                            return false;
                        return true;
                    }));

            }
        }



        private DelegateCommand<SignalItem> _SelectedSignalCommand;
        public DelegateCommand<SignalItem> SelectedSignalCommand
        {
            get
            {
                return _SelectedSignalCommand
                    ?? (_SelectedSignalCommand = new DelegateCommand<SignalItem>(
                    item =>
                    {
                        SelectedSignal = item;

                    }, (item) => SelectedSignal != item));
            }
        }



        private double _RowSignalHeightValue = 4.0;
        private DelegateCommand _ExpendSignalSectionCommand;
        public DelegateCommand ExpendSignalSectionCommand
        {
            get
            {
                return _ExpendSignalSectionCommand
                    ?? (_ExpendSignalSectionCommand = new DelegateCommand(
                    () =>
                    {
                        if (_MainWindow.RowSignal.Height.Value > 0)
                        {
                            _RowSignalHeightValue = _MainWindow.RowSignal.Height.Value;
                            _MainWindow.RowSignal.Height = new GridLength(0.0, GridUnitType.Star);
                        }
                        else
                            _MainWindow.RowSignal.Height = new GridLength(_RowSignalHeightValue, GridUnitType.Star);

                    }));
            }
        }
        private DelegateCommand _NextAssetChartCommand;
        public DelegateCommand NextAssetChartCommand
        {
            get
            {
                return _NextAssetChartCommand
                    ?? (_NextAssetChartCommand = new DelegateCommand(
                    () =>
                    {

                        var inx = ChartAssets.IndexOf(SelectedChartAsset);

                        SelectedChartAsset = inx == ChartAssets.Count - 1 ? ChartAssets[0] : ChartAssets[++inx];
                    }, () => SelectedChartAsset != null || ChartAssets.Count > 1));


            }
        }
        private DelegateCommand _BackAssetChartCommand;
        public DelegateCommand BackAssetChartCommand
        {
            get
            {
                return _BackAssetChartCommand
                    ?? (_BackAssetChartCommand = new DelegateCommand(
                    () =>
                    {
                        var inx = ChartAssets.IndexOf(SelectedChartAsset);
                        SelectedChartAsset = inx == 0 ? ChartAssets[ChartAssets.Count - 1] : ChartAssets[--inx];
                    }, () => SelectedChartAsset != null || ChartAssets.Count > 1));
            }
        }
        private DelegateCommand _AboutCommand;
        public DelegateCommand AboutCommand
        {
            get
            {
                return _AboutCommand
                    ?? (_AboutCommand = new DelegateCommand(
                    () =>
                    {
                        var aboutwin = new AboutWindow();
                        aboutwin.Show();
                    }));
            }
        }


        public ObservableCollection<SignalItem> Signals
        {
            get
            {
                return SignalReceiver.SignalList;
            }
        }


        private string _StartText;
        public string StartText
        {
            get
            {
                return _StartText;
            }
            set
            {
                if (_StartText != value)
                {
                    _StartText = value;
                    NotifyPropertyChanged(n => n.StartText);
                }
            }
        }

        private ImageSource _StartIconSource;
        public ImageSource StartIconSource
        {
            get
            {
                return _StartIconSource;
            }
            set
            {
                if (_StartIconSource != value)
                {
                    _StartIconSource = value;
                    NotifyPropertyChanged(n => n.StartIconSource);
                }
            }
        }
        private bool _IsChartLoading;
        public bool IsChartLoading
        {
            get
            {
                return _IsChartLoading;
            }
            private set
            {
                if (_IsChartLoading != value)
                {
                    _IsChartLoading = value;
                    NotifyPropertyChanged(n => n.IsChartLoading);
                }
            }
        }
        private SignalItem _SelectedSignal;
        public SignalItem SelectedSignal
        {
            get
            {
                return _SelectedSignal;
            }
            set
            {
                if (_SelectedSignal != value)
                {
                    _SelectedSignal = value;
                    NotifyPropertyChanged(n => n.SelectedSignal);
                    SelectedSignalCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    NotifyPropertyChanged(n => n.Status);
                }
            }
        }


        private Brush _StatusColor = new SolidColorBrush(Colors.White);
        public Brush StatusColor
        {
            get
            {
                return _StatusColor;
            }
            set
            {
                if (_StatusColor != value)
                {
                    _StatusColor = value;
                    NotifyPropertyChanged(n => n.StatusColor);
                }
            }
        }

        private string _StatusMessageCloseTimer;
        public string StatusElapsedTimeToShow
        {
            get
            {
                return _StatusMessageCloseTimer;
            }
            set
            {
                if (_StatusMessageCloseTimer != value)
                {
                    _StatusMessageCloseTimer = value;
                    NotifyPropertyChanged(n => n.StatusElapsedTimeToShow);
                }
            }
        }


        private BitmapSource _ScreenShotSource;
        public BitmapSource ScreenShotSource
        {
            get
            {
                return _ScreenShotSource;
            }
            set
            {
                if (_ScreenShotSource != value)
                {
                    _ScreenShotSource = value;
                    NotifyPropertyChanged(n => n.ScreenShotSource);

                }
            }
        }


        public ObservableCollection<Strategy> Strategys
        {
            get { return SignalReceiver.Strategys; }
        }
        private ObservableCollection<ChartAsset> _ChartAssets = new ObservableCollection<ChartAsset>();
        public List<ChartAsset> ChartAssets
        {
            get { return _ChartAssets.OrderBy(n => n.Name).ToList(); }
        }

        public Strategy _SelectedStrategy;
        public Strategy SelectedStrategy
        {
            get
            {
                return _SelectedStrategy;
            }
            set
            {
                if (value != null)
                {

                    if (_SelectedStrategy != null)
                        _SelectedStrategy.ChartAssets.CollectionChanged -= ChartAssets_CollectionChanged1;

                    _SelectedStrategy = value;
                    NotifyPropertyChanged(n => n.SelectedStrategy);
                    _SelectedStrategy.ChartAssets.CollectionChanged += ChartAssets_CollectionChanged1;

                    _ChartAssets.Clear();
                    foreach (var item in _SelectedStrategy.ChartAssets)
                    {
                        item.AllowToReciveSignalChanged -= ChartAsset_AllowToReciveSignalChanged;
                        item.AllowToReciveSignalChanged += ChartAsset_AllowToReciveSignalChanged;
                        _ChartAssets.Add(item);

                    }
                    NotifyPropertyChanged(n => n.ChartAssets);
                    SelectedChartAsset = ChartAssets.FirstOrDefault();
                }



            }
        }

        private void ChartAssets_CollectionChanged1(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            for (int i = _ChartAssets.Count; i < _SelectedStrategy.ChartAssets.Count; i++)
                _ChartAssets.Add(_SelectedStrategy.ChartAssets[i]);

            NotifyPropertyChanged(n => n.ChartAssets);
            NextAssetChartCommand.RaiseCanExecuteChanged();
            BackAssetChartCommand.RaiseCanExecuteChanged();
        }

        public ChartAsset _SelectedChartAsset;
        public ChartAsset SelectedChartAsset
        {
            get
            {
                return _SelectedChartAsset;
            }
            set
            {
                //if (_SelectedChartAsset != value)
                //{
                if (_SelectedChartAsset != null)
                    _SelectedChartAsset.IsViewing = false;

                _SelectedChartAsset = value;

                NotifyPropertyChanged(n => n.SelectedChartAsset);
                if (_SelectedChartAsset != null)
                {
                    _SelectedChartAsset.IsViewing = true;
                    SetScreenShotFilePath(_SelectedChartAsset.ScreenShotFilePath);
                    ViewChartCommand.RaiseCanExecuteChanged();
                }
                NextAssetChartCommand.RaiseCanExecuteChanged();
                BackAssetChartCommand.RaiseCanExecuteChanged();
                //}
            }
        }
        public void SetScreenShotFilePath(string path)
        {
            _ScreenShotFilePath = path;
            _ChartAssetInstalized = false;

            if (_DrawChartThread == null)
            {
                _DrawChartThread = new Thread(new ThreadStart(DrawChart));
                _DrawChartThread.IsBackground = true;
            }



            if (_DrawChartTimer == null)
            {
                _DrawChartTimer = new Timer();
                _DrawChartTimer.Elapsed += _DrawChartTimer_Elapsed;
                _DrawChartTimer.Interval = _ChartRefreshRate;
            }

            DeleteScreenShotFile();
            if (_DrawChartTimer.Enabled)
                _DrawChartTimer.Stop();
            _DrawChartTimer.Start();
            IsChartLoading = true;
        }






        private void _DrawChartTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_DrawChartThread.IsAlive)
            {
                _DrawChartThread = new Thread(new ThreadStart(DrawChart));
                _DrawChartThread.IsBackground = true;
                _DrawChartThread.Start();
            }
        }

        System.Windows.Forms.PictureBox _LoadImagePicBox;
        private MemoryBitmapSource _ScreenShotMemoryBSource;
        private void DrawChart()
        {

            while (string.IsNullOrEmpty(_ScreenShotFilePath) ||
                (!string.IsNullOrEmpty(_ScreenShotFilePath) && !File.Exists(_ScreenShotFilePath)))
                Thread.Sleep(500);

            _LoadImagePicBox = new System.Windows.Forms.PictureBox();
            _LoadImagePicBox.ImageLocation = _ScreenShotFilePath;

            do
            {
                try
                {
                    _LoadImagePicBox.Load();
                }
                catch
                {
                }
            } while (_LoadImagePicBox.Image == null);

            _MainWindow.Dispatcher.Invoke((Action)(() =>
            {

                if (_ScreenShotMemoryBSource != null)
                    _ScreenShotMemoryBSource.Dispose();

                _ScreenShotMemoryBSource = new MemoryBitmapSource(_LoadImagePicBox.Image);
                _LoadImagePicBox.Image.Dispose();
                _LoadImagePicBox.Image = null;
                _LoadImagePicBox.Dispose();

                ScreenShotSource = _ScreenShotMemoryBSource.Source;

                if (!_ChartAssetInstalized)
                {
                    _MainWindow.ChartViewer.ScrollToRightEnd();
                    _ChartAssetInstalized = true;
                }


                IsChartLoading = false;

            }), null);
            DeleteScreenShotFile();
        }

        private void DeleteScreenShotFile()
        {
            if (!string.IsNullOrEmpty(_ScreenShotFilePath))
            {
                var deleted = false;
                do
                {
                    try
                    {
                        File.Delete(_ScreenShotFilePath);
                        deleted = true;
                    }
                    catch
                    {
                    };
                } while (!deleted);
            }
        }




    }
}
