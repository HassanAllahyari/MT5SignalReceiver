using System;
using System.Windows.Media;

namespace MT5SignalReceiver.Helpers
{
    public class ImageHelper
    {
        public static Color LongToColor(long colornum)
        {
            var colorhex = string.Format("#{0:X}", colornum == 0 ? "FF000000" : Convert.ToString(colornum, 16));


            var color =(Color) ColorConverter.ConvertFromString(colorhex);

            return color;

        }
        public static Brush LongToBrush(long colornum)
        {
            var color = LongToColor(colornum);

            return new SolidColorBrush(color);

        }

    }
}
