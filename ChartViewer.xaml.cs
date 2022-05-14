using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MT5SignalReceiver.Models;
using Point = System.Windows.Point;

namespace MT5SignalReceiver
{
    /// <summary>
    /// Interaction logic for ScreenShot.xaml
    /// </summary>
    public partial class ChartViewer : UserControl
    {
        public ChartViewer()
        {
            InitializeComponent();

        }




        public ChartAsset ChartAsset
        {
            get { return (ChartAsset)GetValue(ChartAssetProperty); }
            set { SetValue(ChartAssetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartAsset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartAssetProperty =
            DependencyProperty.Register("ChartAsset", typeof(ChartAsset), typeof(ChartViewer), new PropertyMetadata(null));




        public bool IsChartLoading
        {
            get { return (bool)GetValue(IsChartLoadingProperty); }
            set { SetValue(IsChartLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChartLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsChartLoadingProperty =
            DependencyProperty.Register("IsChartLoading", typeof(bool), typeof(ChartViewer), new PropertyMetadata(false));



        public ImageSource ScreenShotSource
        {
            get { return (ImageSource)GetValue(ScreenShotSourceProperty); }
            set { SetValue(ScreenShotSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScreenShotSourceProperty =
            DependencyProperty.Register("ScreenShotSource", typeof(ImageSource), typeof(ChartViewer), new PropertyMetadata(null));

        public void ScrollToRightEnd()
        {
            ChartScrollViewer.ScrollToRightEnd();
        }
        public void MouseUpPreesed()
        {
            _MouseDownPreesed = false;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {

            if (IsChartLoading)
                this.Cursor = null;
            else if (ChartAsset != null)
            {
                this.Cursor = Cursors.None;
                if (BrdVertical.Visibility != Visibility.Visible)
                {
                    BrdVertical.Visibility = Visibility.Visible;
                    BrdHorizontal.Visibility = Visibility.Visible;
                }
            }

            var chartViewerCurrentPos = e.GetPosition(this);

            BrdVertical.SetValue(Canvas.TopProperty, chartViewerCurrentPos.Y);
            BrdHorizontal.SetValue(Canvas.LeftProperty, chartViewerCurrentPos.X);

            if (ChartAsset != null && BrdVertical.BorderBrush != ChartAsset.ChartForeground)
            {
                BrdVertical.BorderBrush = ChartAsset.ChartForeground;
                BrdHorizontal.BorderBrush = ChartAsset.ChartForeground;
            }

            if (_MouseDownPreesed)
            {
                var currentPos = e.GetPosition(ImgScreenShot);

                if ((int)_ChartViewerPerviewsMousePos.X != (int)chartViewerCurrentPos.X)
                {
                    if ((int)currentPos.X > (int)_MouseDownPos.X)
                        ChartScrollViewer.LineLeft();
                    else if ((int)currentPos.X < (int)_MouseDownPos.X)
                        ChartScrollViewer.LineRight();
                    _ChartViewerPerviewsMousePos = chartViewerCurrentPos;
                }
            }
        }
        private bool _MouseDownPreesed;
        private Point _MouseDownPos;
        private Point _ChartViewerPerviewsMousePos;
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _MouseDownPreesed = true;
                _MouseDownPos = e.GetPosition(ImgScreenShot);
                _ChartViewerPerviewsMousePos = e.GetPosition(this);
            }
        }



        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ChartScrollViewer.LineRight();
                ChartScrollViewer.LineRight();
            }
            else if (e.Delta < 0)
            {
                ChartScrollViewer.LineLeft();
                ChartScrollViewer.LineLeft();
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            BrdVertical.Width = element.ActualWidth;
            BrdHorizontal.Height = element.ActualHeight;
        }


        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            BrdVertical.Visibility = Visibility.Collapsed;
            BrdHorizontal.Visibility = Visibility.Collapsed;
        }

    }
}
