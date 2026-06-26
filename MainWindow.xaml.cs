using MainCyberSecurityChatBot;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Media;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace MainCyberSecurityChatBot {

    //Main chatbot window class
    public partial class MainWindow : Window {
        string connectionString =
         "server=localhost;database=maincybersecuritychatbot;uid=root;pwd=kamogelomathikge@2004;";


        public MainWindow() {

            InitializeComponent();

            TaskAssistantPanel.Visibility = Visibility.Collapsed;

            SoundPlayer player = new SoundPlayer(@"audio\greeting.wav");

            player.Play();

            Loaded += MainWindow_Loaded;

            TestConnection();
        }

        private void TestConnection() {
            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                try {
                    conn.Open();
                    MessageBox.Show("Database Connected!");
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e) {
            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                try {
                    conn.Open();

                    string query = @"INSERT INTO tasks 
                            (title, description, reminder_date, status)
                            VALUES 
                            (@title, @desc, @date, @status)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    DateTime? reminderDate = null;

                    if (ReminderDatePicker.SelectedDate.HasValue) {
                        reminderDate = ReminderDatePicker.SelectedDate.Value;
                    }

                    if (ReminderCheckBox.IsChecked == true
                        && ReminderDatePicker.SelectedDate.HasValue) {
                        TypeText( $"Got it! I'll remind you on " + $"{ReminderDatePicker.SelectedDate.Value.ToShortDateString()}.");
                        AddToLog($"Reminder set for '{TaskTitleBox.Text}' on {ReminderDatePicker.SelectedDate.Value.ToShortDateString()}");
                    }

                    cmd.Parameters.AddWithValue("@title", TaskTitleBox.Text);
                    cmd.Parameters.AddWithValue("@desc", TaskDescriptionBox.Text);
                    cmd.Parameters.AddWithValue("@date", reminderDate.HasValue ? reminderDate : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", "Pending");

                    cmd.ExecuteNonQuery();

                    TypeText( $"Task added with the description " + $"\"{TaskDescriptionBox.Text}\".");
                    AddToLog($"Task added: '{TaskTitleBox.Text}' - '{TaskDescriptionBox.Text}'");


                    TaskTitleBox.Clear();

                    TaskDescriptionBox.Clear();

                    ReminderDatePicker.SelectedDate = null;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private QuizManager quizManager = new QuizManager();


        private void StartQuizButton_Click(object sender, RoutedEventArgs e) {
            quizManager.StartQuiz();

            StartQuizButton.Visibility = Visibility.Collapsed;

            LoadCurrentQuestion();

            AddToLog("Quiz started");
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e) {
            Button clickedButton = (Button)sender;

            // Get the first character (A, B, C or D)
            string selectedAnswer = clickedButton.Content.ToString().Substring(0, 1);

            bool correct = quizManager.SubmitAnswer(selectedAnswer);

            if (correct) {
                FeedbackLabel.Text = "✅ Correct!\n\n" +
                                     quizManager.GetExplanation();
            }
            else {
                FeedbackLabel.Text = "❌ Incorrect!\n\n" +
                                     quizManager.GetExplanation();
            }

            ScoreLabel.Text =
                $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";

            NextQuestionButton.Visibility = Visibility.Visible;

            // Prevent answering twice
            AnswerAButton.IsEnabled = false;
            AnswerBButton.IsEnabled = false;
            AnswerCButton.IsEnabled = false;
            AnswerDButton.IsEnabled = false;
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e) {
            if (quizManager.NextQuestion()) {
                LoadCurrentQuestion();

                AnswerAButton.IsEnabled = true;
                AnswerBButton.IsEnabled = true;
                AnswerCButton.IsEnabled = true;
                AnswerDButton.IsEnabled = true;
            }
            else {
                FinishQuiz();
            }
        }

        private void CloseQuizButton_Click(object sender, RoutedEventArgs e) {
            QuizPanel.Visibility = Visibility.Collapsed;

            TaskAssistantPanel.Visibility = Visibility.Visible;

            StartQuizButton.Visibility = Visibility.Visible;
        }

        private void LoadCurrentQuestion() {
            QuizQuestion question = quizManager.GetCurrentQuestion();

            QuestionNumberLabel.Text =
                $"Question {quizManager.CurrentQuestionNumber} of {quizManager.TotalQuestions}";

            QuestionLabel.Text = question.Question;

            AnswerAButton.Content = question.Options[0];

            AnswerBButton.Content = question.Options[1];

            if (question.Options.Count > 2) {
                AnswerCButton.Content = question.Options[2];
                AnswerCButton.Visibility = Visibility.Visible;

                AnswerDButton.Content = question.Options[3];
                AnswerDButton.Visibility = Visibility.Visible;
            }
            else {
                AnswerCButton.Visibility = Visibility.Collapsed;
                AnswerDButton.Visibility = Visibility.Collapsed;
            }

            ScoreLabel.Text =
                $"Score: {quizManager.Score}/{quizManager.TotalQuestions}";

            FeedbackLabel.Text = "";

            NextQuestionButton.Visibility = Visibility.Collapsed;

            AnswerAButton.IsEnabled = true;
            AnswerBButton.IsEnabled = true;
            AnswerCButton.IsEnabled = true;
            AnswerDButton.IsEnabled = true;

            FeedbackLabel.Text = "";

            NextQuestionButton.Visibility = Visibility.Collapsed;
        }

        private void FinishQuiz()
        {
            string message;

            if (quizManager.Score >= 10)
            {
                message = "🏆 Great job! You're a cybersecurity pro!";
            }
            else if (quizManager.Score >= 7)
            {
                message = "👍 Good effort! Keep learning to stay safe online!";
            }
            else
            {
                message = "📚 Keep practicing your cybersecurity knowledge!";
            }

            MessageBox.Show(
                $"Quiz Complete!\n\n" +
                $"Final Score: {quizManager.Score}/{quizManager.TotalQuestions}\n\n" +
                message,
                "Quiz Results");

            QuizPanel.Visibility = Visibility.Collapsed;

            TaskAssistantPanel.Visibility = Visibility.Visible;

            StartQuizButton.Visibility = Visibility.Visible;

            AddToLog($"Quiz completed - Score: {quizManager.Score}/{quizManager.TotalQuestions}");
        }
        private void ViewTasksButton_Click(object sender, RoutedEventArgs e) {
            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                try {
                    conn.Open();

                    string query = "SELECT * FROM tasks";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    TaskListBox.Items.Clear();

                    while (reader.Read()) {
                        TaskListBox.Items.Add(
                            $"{reader["id"]} | {reader["title"]} | {reader["status"]}"
                        );
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e) {
            int id = GetSelectedTaskId();

            if (id == -1) {
                MessageBox.Show("Select a task first!");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                conn.Open();

                string query = "UPDATE tasks SET status='Completed' WHERE id=@id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Task completed!");
                AddToLog($"Task marked as completed: ID {id}");
            }
        }
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e) {
            int id = GetSelectedTaskId();

            if (id == -1) {
                MessageBox.Show("Select a task first!");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString)) {
                conn.Open();

                string query = "DELETE FROM tasks WHERE id=@id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Task deleted!");
                AddToLog($"Task deleted: ID {id}");
            }
        }

        private int GetSelectedTaskId()
        {
            if (TaskListBox.SelectedItem == null)
                return -1;

            string item = TaskListBox.SelectedItem.ToString();
            return int.Parse(item.Split('|')[0].Trim());
        }

        // Method that runs when application loads
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            // Try to play greeting audio
            try {
                SoundPlayer player = new SoundPlayer(@"audio\greeting.wav");

                // Play greeting sound
                player.Play();
            }
            catch {
                MessageBox.Show("Greeting audio file not found.");
            }
            await System.Threading.Tasks.Task.Delay(300);

            // Display introduction message
            TypeText("Hello, Welcome to Cyber Security Awareness Bot, \n I'm here to help you stay safe online.\n Please enter your name in the chat below."
            );
        }

        // Method for chatbot typing animation
        private async void TypeText(string message) {

            // Create chatbot message bubble
            Border bubble = new Border {

                Background = Brushes.Cyan,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 350
            };

            // Create text container
            TextBlock textBlock = new TextBlock {
                FontSize = 16,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = textBlock;
            ChatPanel.Children.Add(bubble);
            string currentText = "";
            foreach (char c in message) {
                currentText += c;
                textBlock.Text = currentText;
                await System.Threading.Tasks.Task.Delay(40);
                ChatScrollViewer.ScrollToEnd();
            }
        }

        // Checks whether chatbot is waiting for user name
        private bool waitingForName = true;
        private string userName = "";

        private string DetectIntent(string input)
        {
            input = input.ToLower();

            // Quiz detection
            if (input.Contains("quiz") || input.Contains("play quiz") || input.Contains("start quiz"))
                return "quiz";

            // Task detection - MORE ROBUST
            if (input.Contains("add task") ||
                input.Contains("new task") ||
                input.Contains("create task") ||
                input.Contains("remind me") ||
                input.Contains("set a reminder") ||
                input.Contains("add a reminder") ||
                input.Contains("task") ||
                input.Contains("reminder") ||
                input.Contains("remember"))
                return "task";

            // Activity log detection - NEW
            if (input.Contains("activity log") ||
                input.Contains("what have you done") ||
                input.Contains("show log") ||
                input.Contains("recent actions"))
                return "log";

            // Info detection
            if (input.Contains("password") ||
                input.Contains("phishing") ||
                input.Contains("hacking") ||
                input.Contains("2fa") ||
                input.Contains("security") ||
                input.Contains("safe"))
                return "info";

            // Exit detection
            if (input.Contains("bye") ||
                input.Contains("goodbye") ||
                input.Contains("exit") ||
                input.Contains("quit"))
                return "exit";

            return "info";
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                AddBotMessage("Please type something.");
                return;
            }

            AddUserMessage(input);

            // First ask for user's name
            if (waitingForName)
            {
                userName = input;
                waitingForName = false;
                TypeText($"Nice to meet you, {userName}! Ask me anything about cybersecurity.");
                UserInput.Clear();
                return;
            }

            string intent = DetectIntent(input);

            switch (intent)
            {
                case "quiz":
                    AddBotMessage("Starting the Cybersecurity Quiz...");

                    // Hide ALL panels first

                    QuizPanel.Visibility = Visibility.Visible;

                    TaskAssistantPanel.Visibility = Visibility.Collapsed;

                    quizManager.StartQuiz();

                    LoadCurrentQuestion();

                    AddToLog($"NLP: User started quiz with command: '{input}'");

                    break;

                case "task":
                    // Extract task description from the input
                    string taskDescription = ExtractTaskDescription(input);

                    if (!string.IsNullOrWhiteSpace(taskDescription) && taskDescription != input)
                    {
                        // Pre-fill the task description
                        TaskDescriptionBox.Text = taskDescription;

                        TypeText($"I'll help you add a task: \"{taskDescription}\". Please fill in the details below.");

                        AddToLog($"NLP: Task extracted from: '{input}' -> '{taskDescription}'");
                    }
                    else {
                        TypeText("Opening Task Assistant...");
                        AddToLog($"NLP: Task assistant opened");
                    }

                    TaskAssistantPanel.Visibility = Visibility.Visible;

                    QuizPanel.Visibility = Visibility.Collapsed;
                    break;

                case "log":


                    QuizPanel.Visibility = Visibility.Collapsed;
                    TaskAssistantPanel.Visibility = Visibility.Collapsed;

                    // Show activity log
                    AddToLog($"NLP: User viewed activity log");
                    ShowActivityLog();
                    break;

                case "exit":

                    QuizPanel.Visibility = Visibility.Collapsed;
                    TaskAssistantPanel.Visibility = Visibility.Collapsed;

                    TypeText("Stay safe online. Goodbye!");
                    Close();
                    return;

                case "info":

                    QuizPanel.Visibility = Visibility.Collapsed;
                    TaskAssistantPanel.Visibility = Visibility.Collapsed;

                    string response = CybersecurityChatbot.GetResponse(input);
                    TypeText(response);
                    AddToLog($"NLP: User asked about: '{input}'");
                    break;

                default:
                    QuizPanel.Visibility = Visibility.Collapsed;
                    TaskAssistantPanel.Visibility = Visibility.Collapsed;

                    TypeText("I didn't quite understand that. Try asking about passwords, phishing, tasks or quizzes.");
                    break;
            }



            UserInput.Clear();
        }

        private List<string> activityLog = new List<string>();

        private void AddToLog(string action)
        {
            activityLog.Add($"{DateTime.Now}: {action}");
            // Keep only last 20 entries
            if (activityLog.Count > 20)
                activityLog.RemoveAt(0);
        }

        private void ShowActivityLog()
        {
            if (activityLog.Count == 0)
            {
                TypeText("No recent activity to show.");
                return;
            }

            string log = "📋 Here's a summary of recent actions:\n\n";
            int count = 1;
            int start = Math.Max(0, activityLog.Count - 10); // Show last 10

            for (int i = start; i < activityLog.Count; i++)
            {
                log += $"{count}. {activityLog[i]}\n";
                count++;
            }

            TypeText(log);
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButton_Click(sender, e);
            }
        }

        private void AddUserMessage(string message)
        {
            Border bubble = new Border
            {
                Background = Brushes.LightPink,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Right,

                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16
                }
            };

            ChatPanel.Children.Add(bubble);

            ChatScrollViewer.ScrollToEnd();
        }

        private string ExtractTaskDescription(string input)
        {
            // Remove common prefixes
            string[] prefixes = { "add task", "new task", "create task", "remind me", "set a reminder", "add a reminder" };

            input = input.ToLower();

            foreach (string prefix in prefixes)
            {
                if (input.Contains(prefix))
                {
                    int index = input.IndexOf(prefix) + prefix.Length;
                    if (index < input.Length)
                    {
                        string description = input.Substring(index).Trim();
                        // Remove leading "to" or "for"
                        if (description.StartsWith("to "))
                            description = description.Substring(3);
                        if (description.StartsWith("for "))
                            description = description.Substring(4);
                        return description;
                    }
                }
            }
            return input;
        }

        private void AddBotMessage(string message)
        {
            Border bubble = new Border
            {
                Background = Brushes.LightBlue,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,

                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16,
                    Width = 300,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            ChatPanel.Children.Add(bubble);

            ChatScrollViewer.ScrollToEnd();
        }
        
    }
}
