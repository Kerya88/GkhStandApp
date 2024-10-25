using GkhStandApp.Enums;
using GkhStandApp.Services;
using GkhStandApp.UI.CustomControls;
using GkhStandApp.UI.Pages.CustomBehaviours;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GkhStandApp.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для Quiz.xaml
    /// </summary>
    public partial class Quiz : Page
    {
        static readonly CryptoService _cryptoService = new CryptoService();
        readonly QuizService _quizService = new QuizService(_cryptoService);

        Entities.Quiz _currentQuiz;
        int _currentQuestionIndex;
        Dictionary<string, string[]> _foundedROs;
        string selectedKey;

        public Quiz()
        {
            InitializeComponent();
            LoadQuiz();

            FioTextBox.TextChanged += FioTextBox_TextChanged;
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

        private void FioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pattern = @"^[А-ЯЁ]{1}[а-яё]{1,}\s+[А-ЯЁ]{1}[а-яё]{1,}\s+[А-ЯЁ]{1}[а-яё]{1,}$"; // Пример: допустимы только буквы и пробелы
            var isValid = Regex.IsMatch(FioTextBox.Text, pattern);

            // Можно заблокировать кнопку Next, если текст не соответствует шаблону
            NextButton.IsEnabled = isValid;

            if (!isValid && !string.IsNullOrEmpty(FioTextBox.Text))
            {
                FioTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                FioTextBox.BorderBrush = Brushes.Gray;
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

        private void LoadQuiz()
        {
            try
            {
                _currentQuiz = _quizService.GetNowQuizzes()[0];
                _currentQuestionIndex = -1;
                ShowIntro();
            }
            catch (Exception e)
            {
                ShowOutro(e.Message);
            }
        }

        private void ShowIntro()
        {
            QuizName.Text = _currentQuiz.Name;
            InfoText.Text = _currentQuiz.IntroText;
            QuestionText.Visibility = Visibility.Collapsed;
            AnswerTextBox.Visibility = Visibility.Collapsed;
            FioTextBox.Visibility = Visibility.Collapsed;
            ROTextBox.Visibility = Visibility.Collapsed;
            OptionsListBox.Visibility = Visibility.Collapsed;
            StartButton.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Collapsed;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            InfoText.Visibility = Visibility.Collapsed;
            QuestionText.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Visible;
            DisplayNextQuestion();
        }

        private void DisplayNextQuestion()
        {
            if (++_currentQuestionIndex < _currentQuiz.Questions.Count)
            {
                var question = _currentQuiz.Questions[_currentQuestionIndex];

                QuestionText.Text = question.Name;

                switch (question.QuestionType)
                {
                    case QuestionType.NotSet:
                        {
                            AnswerTextBox.Visibility = Visibility.Visible;
                            break;
                        }
                    case QuestionType.IsAnswerId:
                        {
                            OptionsListBox.Visibility = Visibility.Visible;
                            OptionsListBox.ItemsSource = question.Answers.Select(x => x.Name);
                            break;
                        }
                    case QuestionType.IsROId:
                        {
                            ROTextBox.Visibility = Visibility.Visible;
                            NextButton.Visibility = Visibility.Collapsed;
                            SearchButton.Visibility = Visibility.Visible;
                            break;
                        }
                    case QuestionType.IsFIO:
                        {
                            FioTextBox.Visibility = Visibility.Visible;
                            break;
                        }
                }
            }
            else
            {
                _currentQuiz.Passed = true;
                ShowOutro(_currentQuiz.OutroText);

                _quizService.SendPassedQuiz(_currentQuiz);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                selectedKey = radioButton.Content.ToString();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var question = _currentQuiz.Questions[_currentQuestionIndex];

            switch (question.QuestionType)
            {
                case QuestionType.NotSet:
                    {
                        if (!string.IsNullOrEmpty(AnswerTextBox.Text))
                        {
                            question.Answer = AnswerTextBox.Text;
                            AnswerTextBox.Text = string.Empty;
                        }
                        else
                        {
                            return;
                        }
                        break;
                    }
                case QuestionType.IsAnswerId:
                    {
                        if (!string.IsNullOrEmpty(selectedKey))
                        {
                            question.Answer = question.Answers.First(x => x.Name == selectedKey).Id;
                            selectedKey = string.Empty;
                        }
                        else
                        {
                            return;
                        }
                        break;
                    }
                case QuestionType.IsROId:
                    {
                        if (!string.IsNullOrEmpty(selectedKey))
                        {
                            var uk = _foundedROs[selectedKey][1];

                            if (!string.IsNullOrEmpty(uk))
                            {
                                question.Answer = _foundedROs[selectedKey][0];
                                _currentQuiz.UserId += _foundedROs[selectedKey][0];

                                InfoText.Text = $"Ваша управляющая компания - {uk}";
                                InfoText.Visibility = Visibility.Visible;
                                selectedKey = string.Empty;
                                _foundedROs.Clear();
                            }
                            else
                            {
                                ShowOutro("ГЖИ не располагает сведениями о Вашей УК, дальнейшее прохождение опроса невозможно");
                                InfoText.Visibility = Visibility.Visible;
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;
                    }
                case QuestionType.IsFIO:
                    {
                        question.Answer = FioTextBox.Text;
                        _currentQuiz.UserId = FioTextBox.Text.ToLower().Replace(" ", "");

                        FioTextBox.Text = string.Empty;
                        NextButton.IsEnabled = true;
                        break;
                    }
            }

            OptionsListBox.Visibility = Visibility.Collapsed;
            AnswerTextBox.Visibility = Visibility.Collapsed;
            FioTextBox.Visibility = Visibility.Collapsed;
            ROTextBox.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Collapsed;

            DisplayNextQuestion();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchButton.IsEnabled = false;

            _foundedROs = _quizService.GetMatchesROs(ROTextBox.Text);

            if (_foundedROs.Count > 0)
            {
                OptionsListBox.Visibility = Visibility.Visible;
                OptionsListBox.ItemsSource = _foundedROs.Keys;
                NextButton.Visibility = Visibility.Visible;
                InfoText.Visibility = Visibility.Collapsed;
                InfoText.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                OptionsListBox.Visibility = Visibility.Collapsed;
                InfoText.Visibility = Visibility.Visible;
                InfoText.Foreground = new SolidColorBrush(Colors.Red);
                InfoText.Text = "Ваш адрес не найден в системе Электронного ЖКХ. Попробуйте еще раз";
            }

            SearchButton.IsEnabled = true;
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Start());
        }

        private void ShowOutro(string message)
        {
            InfoText.Text = message;
            QuestionText.Visibility = Visibility.Collapsed;
            OptionsListBox.Visibility = Visibility.Collapsed;
            AnswerTextBox.Visibility = Visibility.Collapsed;
            FioTextBox.Visibility = Visibility.Collapsed;
            ROTextBox.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Collapsed;
        }
    }
}
