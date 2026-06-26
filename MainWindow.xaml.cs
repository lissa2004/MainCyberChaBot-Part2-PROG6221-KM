using MainCyberSecurityChatBot;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Media;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
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
                        AddBotMessage(
                            $"Got it! I'll remind you on " +
                            $"{ReminderDatePicker.SelectedDate.Value.ToShortDateString()}."
                        );
                    }

                    cmd.Parameters.AddWithValue("@title", TaskTitleBox.Text);
                    cmd.Parameters.AddWithValue("@desc", TaskDescriptionBox.Text);
                    cmd.Parameters.AddWithValue("@date", reminderDate.HasValue ? reminderDate : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", "Pending");

                    cmd.ExecuteNonQuery();

                    AddBotMessage(
                    $"Task added with the description " +
                    $"\"{TaskDescriptionBox.Text}\"."
                    );

                    TaskTitleBox.Clear();
                    TaskDescriptionBox.Clear();
                    ReminderDatePicker.SelectedDate = null;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
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
            }
        }
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e ) {
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

        private void SendButton_Click(object sender, RoutedEventArgs e) {

            // Get user message from textbox
            string userMessage = UserInput.Text;

            if (userMessage.ToLower().Contains("add task")) {
                TaskAssistantPanel.Visibility = Visibility.Visible;

                AddBotMessage(
                    "Task assistant opened. " +
                    "Please enter the task details."
                );

                return;
            }

            // Check if textbox empty
            if (string.IsNullOrWhiteSpace(userMessage)) {
                AddBotMessage("Please type something.");

                return;
            }

            // Display user message bubble
            AddUserMessage(userMessage);

            if (userMessage.ToLower().Contains("add task") ||
            userMessage.ToLower().Contains("reminder")) {
                AddBotMessage("Opening Task Assistant...");

                TaskAssistantPanel.Visibility = Visibility.Visible;
            }


            // Clear textbox after sending
            UserInput.Clear();

            if (waitingForName) {
                userName = userMessage;
                waitingForName = false;

                // Display welcome message
                TypeText(
                    $"Nice to meet you, {userName}!\n\n" +
                    "Ask me anything about online safety!\n\n" +
                    "You can ask about:\n" +
                    "- Password safety\n" +
                    "- Phishing\n" +
                    "- Safe browsing\n" +
                    "- Hacking"
                );

                return;
            }


            // Exit chatbot if user types goodbye
            if (userMessage.ToLower() == "goodbye") {
                TypeText("Stay safe online, GOODBYE!");
            Close();

                return;
            }

        string response = CybersecurityChatbot.GetResponse(userMessage);
        TypeText(response);
    }

        // Detect Enter key press/trigger event
        private void UserInput_KeyDown(object sender,System.Windows.Input.KeyEventArgs e) {
            if (e.Key == System.Windows.Input.Key.Enter) {

                SendButton_Click(sender, e);
            }
        }

        // Method for displaying user chat bubble
        private void AddUserMessage(string message) {
            Border bubble = new Border {
                Background = Brushes.LightPink,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Right,
                Child = new TextBlock {
                    Text = message,
                    FontSize = 16
                }
            };

            // Add bubble to chat panel
            ChatPanel.Children.Add(bubble);

            // Scroll to newest message
            ChatScrollViewer.ScrollToEnd();
        }


        // Method for bot messages chat bubble
        private void AddBotMessage(string message) {

            Border bubble = new Border {
                Background = Brushes.Purple,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8),
                Margin = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = new TextBlock {
                    Text = message,
                    FontSize = 16,
                    Width = 300,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            // Add chatbot bubble
            ChatPanel.Children.Add(bubble);

            // Auto scroll
            ChatScrollViewer.ScrollToEnd();

        }
    }
}
