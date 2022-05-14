
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MT5SignalReceiver.Models;
using System.Windows.Input;

namespace MT5SignalReceiver
{
    public enum WebBrowserStatus
    {
        None,
        Loading,
        Ready
    }
    /// <summary>
    /// Interaction logic for BrokerWebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : System.Windows.Controls.UserControl
    {
        public WebBrowser()
        {
            InitializeComponent();

            CWebBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            CWebBrowser.AddressChanged += CWebBrowser_AddressChanged;
        }

        private void CWebBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TxtBUrl.Text = CWebBrowser.Address;
            CWebBrowser.BackCommand.CanExecute(null);
            CWebBrowser.ForwardCommand.CanExecute(null);
        }

        public event WebBrowserStatusChangedEventHandler WebBrowserStatusChanged;


        public WebBrowserStatus Status
        {
            get { return (WebBrowserStatus)GetValue(StatusProperty); }
            private set
            {
                SetValue(StatusProperty, value);
                if (WebBrowserStatusChanged != null)
                    WebBrowserStatusChanged(this, new WebBrowserStatusChangedEventEventArgs(Status));
            }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(WebBrowserStatus), typeof(WebBrowser), new PropertyMetadata(WebBrowserStatus.None));

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {

            Dispatcher.Invoke(() => Status = e.IsLoading ? WebBrowserStatus.Loading : WebBrowserStatus.Ready);
        }

        private void TxtBUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string url = TxtBUrl.Text;
                Uri uriResult;
                bool urlisvalid = ValidHttpURL(url, out uriResult);

                if (!urlisvalid)
                    url = string.Format("https://www.google.com/search?q={0}", url);

                CWebBrowser.Load(url);
            }
        }
        public static bool ValidHttpURL(string url, out Uri resultURI)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out resultURI)
                && (resultURI.Scheme == Uri.UriSchemeHttp || resultURI.Scheme == Uri.UriSchemeHttps);

        }
        private void TxtBUrl_KeyDown(object sender, KeyEventArgs e)
        {

            TxtBlUrlWatermarker.Visibility = Visibility.Collapsed;
        }

        private void TxtBlUrlWatermarker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtBUrl.Focus();
        }

        private void TxtBUrl_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TxtBlUrlWatermarker.Visibility = string.IsNullOrEmpty(TxtBUrl.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}