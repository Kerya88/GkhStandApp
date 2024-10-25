using GkhStandApp.Services;
using GkhStandApp.UI.Pages.CustomBehaviours;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GkhStandApp.UI.Pages
{
    public partial class ManOrgInfo : Page
    {
        static readonly CryptoService _cryptoService = new CryptoService();
        readonly QuizService _quizService = new QuizService(_cryptoService);

        Dictionary<string, string[]> _foundedROs;
        string selectedKey;

        public ManOrgInfo()
        {
            InitializeComponent();
            ShowIntro();

            ROTextBox.TextChanged += ROTextBox_TextChanged;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                CustomKeyboardControl.AttachTextBox(textBox); // Подключаем текстовое поле к клавиатуре
                CustomKeyboardControl.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Прячем клавиатуру при потере фокуса, если она не активна
            if (!CustomKeyboardControl.IsKeyboardFocusWithin)
            {
                CustomKeyboardControl.Visibility = Visibility.Collapsed;
            }
        }

        private void ROTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pattern = @"^[А-ЯЁа-яё\-\s]+,{1}\s*[А-ЯЁа-яё\-\s]+,{1}\s*\d+[А-ЯЁа-яё]?$"; // Пример: допустимы только буквы и пробелы
            var isValid = Regex.IsMatch(ROTextBox.Text, pattern);

            // Можно заблокировать кнопку Next, если текст не соответствует шаблону
            SearchButton.IsEnabled = isValid;

            if (!isValid && !string.IsNullOrEmpty(ROTextBox.Text))
            {
                ROTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ROTextBox.BorderBrush = Brushes.Gray;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InactivityTimerBehaviour.Attach(this);
        }

        private void ShowIntro()
        {
            InfoText.Text = "Пожалуйста, укажите адрес вашего дома в формате\r\nНаселенный пункт, Улица, Номер дома.\r\nНапример: Кинель, Спортивная, 12 ";
            ROTextBox.Visibility = Visibility.Visible;
            OptionsListBox.Visibility = Visibility.Collapsed;
            ShowManOrgButton.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Visible;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                selectedKey = radioButton.Content.ToString();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchButton.IsEnabled = false;

            _foundedROs = _quizService.GetMatchesROs(ROTextBox.Text);

            if (_foundedROs.Count > 0)
            {
                OptionsListBox.Visibility = Visibility.Visible;
                OptionsListBox.ItemsSource = _foundedROs.Keys;
                ShowManOrgButton.Visibility = Visibility.Visible;
                InfoText.Visibility = Visibility.Collapsed;
                InfoText.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                OptionsListBox.Visibility = Visibility.Collapsed;
                ShowManOrgButton.Visibility = Visibility.Collapsed;
                InfoText.Visibility = Visibility.Visible;
                InfoText.Foreground = new SolidColorBrush(Colors.Red);
                InfoText.Text = "Ваш адрес не найден в системе Электронного ЖКХ. Попробуйте еще раз";
            }

            SearchButton.IsEnabled = true;
        }

        private void ShowManOrgButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedKey))
            {
                var uk = _foundedROs[selectedKey][1];
                InfoText.Visibility = Visibility.Visible;
                InfoText.Foreground = new SolidColorBrush(Colors.Black);

                if (!string.IsNullOrEmpty(uk))
                {
                    InfoText.Text = $"Ваша управляющая компания - {uk}";
                }
                else
                {
                    InfoText.Text = "ГЖИ не располагает сведениями о Вашей УК";
                }

                ROTextBox.Visibility = Visibility.Collapsed;
                OptionsListBox.Visibility = Visibility.Collapsed;
                ShowManOrgButton.Visibility = Visibility.Collapsed;
                SearchButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                InfoText.Text = "Выберете дом из списка";
                InfoText.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Start());
        }
    }
}
