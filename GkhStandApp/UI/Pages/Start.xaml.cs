using System.Windows.Controls;
using System.Windows.Navigation;

namespace GkhStandApp.UI.Pages
{
    public partial class Start : Page
    {
        Quiz _quizPage;

        public Start()
        {
            InitializeComponent();
            _quizPage = new Quiz();
        }

        private void OpenQuizButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Переход на страницу с опросом
            NavigationService.Navigate(_quizPage);
        }
    }
}
