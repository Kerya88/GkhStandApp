using System.Windows;
using System.Windows.Controls;

namespace GkhStandApp.UI.CustomControls
{
    /// <summary>
    /// Логика взаимодействия для CustomKeyboard.xaml
    /// </summary>
    public partial class CustomKeyboard : UserControl
    {
        TextBox _currentTextBox;

        public CustomKeyboard()
        {
            InitializeComponent();
            ShiftButton.IsChecked = true;
        }

        // Метод для связывания с текущим текстовым полем
        public void AttachTextBox(TextBox textBox)
        {
            _currentTextBox = textBox;
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTextBox != null && sender is Button button)
            {
                if (CapsButton.IsChecked.Value || ShiftButton.IsChecked.Value)
                {
                    _currentTextBox.Text += button.Content.ToString();
                    _currentTextBox.CaretIndex = _currentTextBox.Text.Length; // Перемещаем курсор в конец

                    if (ShiftButton.IsChecked.Value)
                    {
                        ShiftButton.IsChecked = false;
                        UpdateAllButtonCase(false);
                    }
                }
                else
                {
                    _currentTextBox.Text += button.Content.ToString().ToLower();
                    _currentTextBox.CaretIndex = _currentTextBox.Text.Length; // Перемещаем курсор в конец
                }
            }
        }

        private void Shift_Click(object sender, RoutedEventArgs e)
        {
            var upper = ShiftButton.IsChecked.Value;

            UpdateAllButtonCase(upper);
        }

        private void Caps_Click(object sender, RoutedEventArgs e)
        {
            var upper = CapsButton.IsChecked.Value;

            UpdateAllButtonCase(upper);
        }

        private void UpdateAllButtonCase(bool upper)
        {
            UpdateButtonCase(upper, BtnYo);
            UpdateButtonCase(upper, BtnC);
            UpdateButtonCase(upper, BtnU);
            UpdateButtonCase(upper, BtnK);
            UpdateButtonCase(upper, BtnE);
            UpdateButtonCase(upper, BtnN);
            UpdateButtonCase(upper, BtnG);
            UpdateButtonCase(upper, BtnSh);
            UpdateButtonCase(upper, BtnShi);
            UpdateButtonCase(upper, BtnTh);
            UpdateButtonCase(upper, BtnH);
            UpdateButtonCase(upper, BtnHard);
            UpdateButtonCase(upper, BtnF);
            UpdateButtonCase(upper, BtnUi);
            UpdateButtonCase(upper, BtnV);
            UpdateButtonCase(upper, BtnA);
            UpdateButtonCase(upper, BtnP);
            UpdateButtonCase(upper, BtnR);
            UpdateButtonCase(upper, BtnO);
            UpdateButtonCase(upper, BtnL);
            UpdateButtonCase(upper, BtnD);
            UpdateButtonCase(upper, BtnJ);
            UpdateButtonCase(upper, BtnYe);
            UpdateButtonCase(upper, BtnYa);
            UpdateButtonCase(upper, BtnCh);
            UpdateButtonCase(upper, BtnS);
            UpdateButtonCase(upper, BtnM);
            UpdateButtonCase(upper, BtnI);
            UpdateButtonCase(upper, BtnT);
            UpdateButtonCase(upper, BtnSoft);
            UpdateButtonCase(upper, BtnB);
            UpdateButtonCase(upper, BtnYu);
        }

        private void UpdateButtonCase(bool upper, Button button)
        {
            if (upper)
            {
                button.Content = button.Content.ToString().ToUpper();
            }
            else
            {
                button.Content = button.Content.ToString().ToLower();
            }
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTextBox != null && _currentTextBox.Text.Length > 0)
            {
                _currentTextBox.Text = _currentTextBox.Text[..^1];
                _currentTextBox.CaretIndex = _currentTextBox.Text.Length;
            }
        }

        private void Space_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTextBox != null)
            {
                ShiftButton.IsChecked = false;
                CapsButton.IsChecked = false;
                UpdateAllButtonCase(false);

                _currentTextBox.Text += " ";
            }
        }
    }
}
