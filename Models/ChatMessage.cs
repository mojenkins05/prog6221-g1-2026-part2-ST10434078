using System;

namespace POEProg6221.GUI.Models
{
    /// Represents a single chat message in the conversation.
   
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsBot { get; set; }
        public string SentimentDetected { get; set; }

        public ChatMessage(string sender, string message, bool isBot, string sentiment = "")
        {
            Sender = sender;
            Message = message;
            Timestamp = DateTime.Now;
            IsBot = isBot;
            SentimentDetected = sentiment;
        }


        /// Returns a formatted display string for the chat message.

        public string ToDisplayString()
        {
            string icon = IsBot ? "🤖" : "👤";
            return $"[{Timestamp:HH:mm:ss}] {icon} {Sender}: {Message}";
        }
    }
}
