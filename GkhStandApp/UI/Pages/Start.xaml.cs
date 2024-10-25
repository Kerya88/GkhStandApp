using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GkhStandApp.UI.Pages
{
    public partial class Start : Page
    {
        readonly Quiz _quizPage;
        readonly ManOrgInfo _manOrgInfoPage;

        readonly DispatcherTimer _inactivityTimer;
        readonly DispatcherTimer _slideshowTimer;

        readonly string[] _imagePaths = ["pack://application:,,,/Resources/Images/Slideshow/1.jpg", "pack://application:,,,/Resources/Images/Slideshow/2.jpg",
            "pack://application:,,,/Resources/Images/Slideshow/3.jpg", "pack://application:,,,/Resources/Images/Slideshow/4.jpg",
            "pack://application:,,,/Resources/Images/Slideshow/5.jpg"];
        int _currentImageIndex;

        public Start()
        {
            InitializeComponent();
            _quizPage = new Quiz();
            _manOrgInfoPage = new ManOrgInfo();

            // Инициализация таймера
            _inactivityTimer = new DispatcherTimer();
            _inactivityTimer.Interval = TimeSpan.FromMinutes(5);
            _inactivityTimer.Tick += InactivityTimer_Tick;

            _slideshowTimer = new DispatcherTimer();
            _slideshowTimer.Interval = TimeSpan.FromSeconds(5);
            _slideshowTimer.Tick += SlideshowTimer_Tick;

            // Отслеживание пользовательских действий
            //this.PreviewMouseMove += ResetInactivityTimer;
            this.PreviewMouseDown += ResetInactivityTimer;

            // Запуск таймера при запуске приложения
            _inactivityTimer.Start();
        }

        private void OpenQuizButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Переход на страницу с опросом
            NavigationService.Navigate(_quizPage);
        }

        private void OpenManOrgInfoButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Переход на страницу с опросом
            NavigationService.Navigate(_manOrgInfoPage);
        }

        // Метод запуска слайдшоу после 5 минут бездействия
        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Останавливаем таймер
            StartSlideshow(); // Запуск слайдшоу
        }

        private void SlideshowTimer_Tick(object sender, EventArgs e)
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Length;
            SlideshowImage.Source = new BitmapImage(new Uri(_imagePaths[_currentImageIndex]));
        }

        // Метод для сброса таймера при активности пользователя
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _slideshowTimer.Stop();

            SlideshowImage.Visibility = System.Windows.Visibility.Collapsed;

            OpenQuizButton.Visibility = System.Windows.Visibility.Visible;
            OpenManOrgInfoButton.Visibility = System.Windows.Visibility.Visible;

            _inactivityTimer.Start();
        }

        // Метод для запуска слайдшоу
        private void StartSlideshow()
        {
            OpenQuizButton.Visibility = System.Windows.Visibility.Collapsed;
            OpenManOrgInfoButton.Visibility = System.Windows.Visibility.Collapsed;

            SlideshowImage.Visibility = System.Windows.Visibility.Visible;

            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Length;
            SlideshowImage.Source = new BitmapImage(new Uri(_imagePaths[_currentImageIndex]));
            _slideshowTimer.Start();
        }
    }
}
