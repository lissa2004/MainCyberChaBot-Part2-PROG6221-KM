using System;
using System.Media; // Provides SoundPlayer for playing .wav audio files

// This class handles all audio-related functionality
class AudioGreetings{
   //Audio plays when chat start method, plays a .wav audio file when the chatbot starts
    public static void PlayGreeting(){
        try{
            SoundPlayer player = new SoundPlayer("audio/greeting.wav");
            player.PlaySync();
        }
        catch{
            // Error handling if file is missing or path is incorrect
            Console.WriteLine("⚠ Audio file not found.");
        }
    }
}