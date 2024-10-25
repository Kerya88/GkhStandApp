using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GkhStandApp.UI.Pages
{
    public partial class Slideshow : Page
    {
        readonly DispatcherTimer _slideshowTimer;
        readonly string[] _imagePaths = ["pack://application:,,,/Resources/Slideshow/1.jpg", "pack://application:,,,/Resources/Slideshow/2.jpg",
            "pack://application:,,,/Resources/Slideshow/3.jpg", "pack://application:,,,/Resources/Slideshow/4.jpg",
            "pack://application:,,,/Resources/Slideshow/5.jpg"];
        int _currentImageIndex;

        public Slideshow()
        {
            InitializeComponent();

            this.PreviewMouseMove += ResetInactivityTimer;
            this.PreviewMouseDown += ResetInactivityTimer;

            // Путь к изображениям слайдшоу
            _currentImageIndex = 0;

            // Настройка таймера для смены изображений
            _slideshowTimer = new DispatcherTimer();
            _slideshowTimer.Interval = TimeSpan.FromSeconds(5);
            _slideshowTimer.Tick += SlideshowTimer_Tick;
            _slideshowTimer.Start();
        }

        // Смена изображения
        private void SlideshowTimer_Tick(object sender, EventArgs e)
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Length;
            SlideshowImage.Source = new BitmapImage(new Uri(_imagePaths[_currentImageIndex]));
        }

        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _slideshowTimer.Stop();

            NavigationService.Navigate(new Start());
        }
    }
}
