
using System;
using System.Collections.Generic;

namespace MainCyberSecurityChatBot{
    class CybersecurityChatbot {
        private static Random random = new Random();

       
        private static string userName = "";
        private static string userInterest = "";
        private static string lastTopic = "";


        private static Dictionary<string, List<string>> tips = new Dictionary<string, List<string>>(){
            {
                "phishing", new List<string>() {
                    "Be cautious of emails asking for personal information.",
                    "Always verify the sender before clicking links.",
                    "Do not open suspicious attachments.",
                    "Look out for urgent or threatening language in emails."
                }
            },
            {
                "password", new List<string>() {
                    "Use at least 12 characters with symbols and numbers.",
                    "Never reuse passwords across accounts.",
                    "Enable two-factor authentication.",
                    "Avoid using personal details in passwords."
                }
            },
            {
                "browsing", new List<string>() {
                    "Only use HTTPS websites for sensitive data.",
                    "Avoid downloading unknown files.",
                    "Keep your browser updated.",
                    "Do not enter personal info on untrusted sites."
                }
            }
        };

      
        private static string GetRandom(List<string> list) {
            return list[random.Next(list.Count)];
        }


        private static string DetectSentiment(string input) {
            if (input.Contains("worried") || input.Contains("scared"))
                return "worried";

            if (input.Contains("frustrated") || input.Contains("annoyed"))
                return "frustrated";

            if (input.Contains("curious"))
                return "curious";

            return "";
        }


        public static string GetResponse(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you.";

            input = input.ToLower();

            string sentiment = DetectSentiment(input);

       
            if (input.Contains("my name is")) {
                userName = input.Replace("my name is", "").Trim();
                return $"Nice to meet you, {userName}! How can I help you with cybersecurity today?";
            }


            if (input.Contains("i'm interested in")) {
                userInterest = input.Replace("i'm interested in", "").Trim();
                return $"Great! I'll remember that you're interested in {userInterest}.";
            }

          
            if (input.Contains("phishing")) {
                lastTopic = "phishing";
                return BuildResponse("Phishing is a cyberattack where scammers trick you into giving sensitive information.", sentiment);
            }

         
            if (input.Contains("password")) {
                lastTopic = "password";
                return BuildResponse("Strong passwords protect your accounts from attackers.", sentiment);
            }

         
            if (input.Contains("safe browsing")) {
                lastTopic = "browsing";
                return BuildResponse("Safe browsing helps protect you from malicious websites.", sentiment);
            }

         
            if (input.Contains("hack")) {
                lastTopic = "hacking";
                return BuildResponse("Hacking involves exploiting system vulnerabilities.", sentiment);
            }

       
            if (input.Contains("give me a tip") || input.Contains("tip")) {
                if (tips.ContainsKey(lastTopic))
                    return GetRandom(tips[lastTopic]);

                return "Tell me a topic first (like phishing or password safety) so I can give you a tip.";
            }

        
            if (input.Contains("help") || input.Contains("what can i ask")) {
                return @"You can ask me about:
- Phishing
- Password safety
- Safe browsing
- Hacking

Then ask 'give me a tip' for advice!";
            }

            return "I'm not sure I understand. Can you try rephrasing or ask about cybersecurity topics like phishing or passwords?";
        }


        private static string BuildResponse(string baseResponse, string sentiment) {
            string emotionalLayer = "";

            if (sentiment == "worried") {
                emotionalLayer = "It's completely understandable to feel that way. ";
            }
            else if (sentiment == "frustrated") {
                emotionalLayer = "Don't worry, cybersecurity can be confusing at first. ";
            }
            else if (sentiment == "curious") {
                emotionalLayer = "That's a great question! ";
            }

            string tip = "";

            if (tips.ContainsKey(lastTopic)) {
                tip = "\n\nTip: " + GetRandom(tips[lastTopic]);
            }

            return emotionalLayer + baseResponse + tip;
        }
    }
}