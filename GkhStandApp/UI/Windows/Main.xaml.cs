using GkhStandApp.UI.Pages;
using System.Windows;

namespace GkhStandApp.UI.Windows
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            MainFrame.Navigate(new Start());
        }
    }
}
