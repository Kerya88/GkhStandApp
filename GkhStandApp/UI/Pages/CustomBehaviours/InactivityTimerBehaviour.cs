using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace GkhStandApp.UI.Pages.CustomBehaviours
{
    public class InactivityTimerBehaviour : DependencyObject
    {
        DispatcherTimer _inactivityTimer;
        static readonly TimeSpan _defaultInactivityTime = TimeSpan.FromMinutes(5);

        public static void Attach(Page page)
        {
            var behavior = new InactivityTimerBehaviour();
            behavior.Initialize(page);
        }

        private void Initialize(Page page)
        {
            _inactivityTimer = new DispatcherTimer { Interval = _defaultInactivityTime };
            _inactivityTimer.Tick += (s, e) => NavigateToStartPage(page, _inactivityTimer);

            page.PreviewMouseDown += ResetInactivityTimer;

            _inactivityTimer.Start();
        }

        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();
        }

        private static void NavigateToStartPage(Page page, DispatcherTimer inactivityTimer)
        {
            inactivityTimer.Stop();
            NavigationService.GetNavigationService(page)?.Navigate(new Start());
        }
    }
}
