using System;
using System.Threading;

// Handles all UI, formatting, and display logic
class UserInterface{
    // Header Display
    public static void DisplayHeader(){
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.ForegroundColor = ConsoleColor.Cyan;

        string header = @"
***********************************************
        CYBER WORLD BOT.

Want to Learn More About The Cyber World
***********************************************
";

        TypeText(header, 2);

        Console.WriteLine("\nStay safe in the digital world!\n");

        Console.ResetColor();
    }

    // Get user name Method
    public static string GetUserName(){
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nEnter your name: ");
        Console.ResetColor();

        string name = Console.ReadLine();

        // Input validation
        if (string.IsNullOrWhiteSpace(name)){
            return "Guest";
        }  
        return name;
    }

    // Typing effects method
    public static void TypeText(string text, int speed = 10){
        foreach (char c in text){
            Console.Write(c);
            Thread.Sleep(speed);
        }
        Console.WriteLine();
    }

    // Bot Typing Box method
     public static void BotTyping(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;

        // typing animation loop
        Console.Write("Bot is typing");
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(300);
            Console.Write(".");
        }
        Console.WriteLine("\n");
        DrawBox(message);

        Console.ResetColor();
    }


    // User message method
    public static void UserMessage(string name, string message){
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n{name}:");
        DrawBox(message);
        Console.ResetColor();
    }

    // Draw box method
    private static void DrawBox(string message){
        string[] lines = message.Split('\n');
        int maxLength = 0;

        foreach (string line in lines){
            if (!string.IsNullOrWhiteSpace(line)){   
                string clean = line.Trim();
                if (clean.Length > maxLength)
                    maxLength = clean.Length;
            }
        }

        string border = new string('*', maxLength + 4);
        Console.WriteLine(border);

        foreach (string line in lines){
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string clean = line.Trim();
            Console.WriteLine($"* {clean.PadRight(maxLength)} *");
        }

        Console.WriteLine(border);
    }

    // Error handling method
    public static void Error(string message){
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine( message);
        Console.ResetColor();
    }

    // funny response method 
    public static void FunnyResponse()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Hey... even hackers type something! Try again.");
        Console.ResetColor();
    }
}
