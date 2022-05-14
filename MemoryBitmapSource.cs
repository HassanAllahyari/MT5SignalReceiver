using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MT5SignalReceiver
{

    public class MemoryBitmapSource : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        private IntPtr hBitmap;
        public MemoryBitmapSource(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            hBitmap = bitmap.GetHbitmap();
            Source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            bitmap.Dispose();
        }
        public MemoryBitmapSource(string imageurl)
        {


            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imageurl);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            Source = bitmapImage;


        }
        public MemoryBitmapSource(Bitmap image)
        {
            hBitmap = image.GetHbitmap();
            Source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            image.Dispose();
        }
        public BitmapSource Source { get; private set; }

        public void Dispose()
        {
            DeleteObject(hBitmap);
        }
    }
}
