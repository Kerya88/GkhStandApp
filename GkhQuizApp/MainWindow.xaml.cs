using GJIService;
using GkhQuizApp.Entities;
using GkhQuizApp.Enums;
using GkhQuizApp.Services;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GkhQuizApp
{
    public partial class MainWindow : Window
    {
        static readonly CryptoService _cryptoService = new CryptoService();
        static readonly QuizService _quizService = new QuizService(_cryptoService);

        static Quiz _currentQuiz;
        static int _currentQuestionIndex;
        static Dictionary<string, string[]> _foundedROs;
        string selectedKey;

        public MainWindow()
        {
            InitializeComponent();
            LoadQuiz();

            FioTextBox.TextChanged += FioTextBox_TextChanged;
            ROTextBox.TextChanged += ROTextBox_TextChanged;

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void FioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pattern = @"^[А-ЯЁ]{1}[а-яё]{1,}\s[А-ЯЁ]{1}[а-яё]{1,}\s[А-ЯЁ]{1}[а-яё]{1,}$"; // Пример: допустимы только буквы и пробелы
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
            NextButton.IsEnabled = isValid;

            if (!isValid && !string.IsNullOrEmpty(ROTextBox.Text))
            {
                ROTextBox.BorderBrush = Brushes.Red;
            }
            else
            {
                ROTextBox.BorderBrush = Brushes.Gray;
            }
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
            RebootButton.Visibility = Visibility.Collapsed;
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
                        question.Answer = AnswerTextBox.Text;
                        AnswerTextBox.Text = string.Empty;
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
                        FioTextBox.Text = string.Empty;
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
            }
            else
            {
                InfoText.Visibility = Visibility.Visible;
                InfoText.Text = "Ваш адрес не найден в системе Электронного ЖКХ. Попробуйте еще раз";
            }

            SearchButton.IsEnabled = true;
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            QuizName.Text = string.Empty;
            InfoText.Text = string.Empty;
            QuestionText.Text = string.Empty;
            AnswerTextBox.Text = string.Empty;
            FioTextBox.Text = string.Empty;
            ROTextBox.Text = string.Empty;

            QuestionText.Visibility = Visibility.Collapsed;
            AnswerTextBox.Visibility = Visibility.Collapsed;
            FioTextBox.Visibility = Visibility.Collapsed;
            ROTextBox.Visibility = Visibility.Collapsed;
            OptionsListBox.Visibility = Visibility.Collapsed;

            StartButton.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
            SearchButton.Visibility = Visibility.Collapsed;
            RebootButton.Visibility = Visibility.Collapsed;

            LoadQuiz();
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
            RebootButton.Visibility = Visibility.Visible;
        }
    }
}