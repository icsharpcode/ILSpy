using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ICSharpCode.ILSpy
{
    /// <summary>
    /// Interaction logic for MainWindowSplashScreen.xaml
    /// </summary>
    public partial class MainWindowSplashScreen : Window
    {
        public MainWindowSplashScreen()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200.0);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        int index = 0;


        void timer_Tick(object sender, EventArgs e)
        {
            BitmapImage current = null;
            index = (index + 1) % 6;
            switch (index)
            {
                case 1:
                    current = GetImage("loading_12");
                    break;
                case 2:
                    current = GetImage("loading_13");
                    break;
                case 3:
                    current = GetImage("loading_14");
                    break;
                case 4:
                    current = GetImage("loading_13");
                    break;
                case 5:
                    current = GetImage("loading_12");
                    break;
                default:
                    current = GetImage("loading_11");
                    break;
            }
            ilspyheader.Source = current;
        }

        Dictionary<string, BitmapImage> _imageCache = new Dictionary<string, BitmapImage>();
        private BitmapImage GetImage(string imageName)
        {
            BitmapImage result;
            if (!_imageCache.TryGetValue(imageName, out result))
            {
                result = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageName + ".gif"));
                result.Freeze();
                _imageCache.Add(imageName, result);
            }
            return result;
        }
    }
}
