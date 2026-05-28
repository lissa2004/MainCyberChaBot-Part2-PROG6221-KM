using System;
using System.Windows;
using System.Windows.Controls;
using System.Media;
using System.Windows.Media;


namespace MainCyberSecurityChatBot {
    public partial class MainWindow : Window {
        public MainWindow() {

            InitializeComponent();

               SoundPlayer player = new SoundPlayer(@"audio\greeting.wav");

            player.Play();

            AddBotMessage("Hello! Welcome to Cyber World Bot.");
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) {
            string userMessage = UserInput.Text;

            if (string.IsNullOrWhiteSpace(userMessage)) {
                AddBotMessage("Please type something.");
                return;
            }

            AddUserMessage(userMessage);
            UserInput.Clear();

            // chatbot response
            string response =
                CybersecurityChatbot.GetResponse(userMessage);

            AddBotMessage(response);
        }

        private void AddUserMessage(string message) {
            Border bubble = new Border {
                Background = Brushes.Gold,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
                Child = new TextBlock {
                    Text = message,
                    FontSize = 16
                }
            };

            ChatPanel.Children.Add(bubble);
        }

        private void AddBotMessage(string message) {

            Border bubble = new Border {
                Background = Brushes.Cyan,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = new TextBlock {
                    Text = message,
                    FontSize = 16,
                    Width = 300,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            ChatPanel.Children.Add(bubble);
        }
    }
}
