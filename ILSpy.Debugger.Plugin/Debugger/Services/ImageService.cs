using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ICSharpCode.ILSpy.Debugger.Services
{
    internal static class ImageService
    {
        private static BitmapImage LoadBitmap(string name)
        {
            BitmapImage result;
            try
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/ILSpy.Debugger.Plugin;component/Images/" + name + ".png"));
                if (bitmapImage == null)
                {
                    result = null;
                }
                else
                {
                    bitmapImage.Freeze();
                    result = bitmapImage;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public static ImageSource GetImage(string imageName)
        {
            return ImageService.LoadBitmap(imageName);
        }

        
        public static readonly BitmapImage Breakpoint = ImageService.LoadBitmap("Breakpoint");

        public static readonly BitmapImage CurrentLine = ImageService.LoadBitmap("CurrentLine");
    }
}
