using GkhStandApp.UI.Pages;
using System.Windows;

namespace GkhStandApp.UI.Windows
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            MainFrame.Navigate(new Start());

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }
    }
}
