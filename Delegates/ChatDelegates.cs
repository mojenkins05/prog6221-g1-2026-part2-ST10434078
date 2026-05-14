using System;

namespace POEProg6221.GUI.Delegates
{ /// <summary>
  /// Custom delegates used to create a clean, extensible architecture for 
  /// processing user input, sentiment detection and memory recall.
  /// This follows the recommendation of using delegates for loose coupling 
  /// in C# applications (Microsoft, 2024b).
  /// </summary>
  
  /// Delegate for handling response generation events.
    public delegate string ResponseHandler(string userInput);

    /// Delegate for handling sentiment detection events.

    public delegate string SentimentHandler(string userInput);

    /// Delegate for handling memory recall events.

    public delegate string MemoryRecallHandler(string topic);

    /// Delegate for notifying when a message is added to the chat.

    public delegate void MessageAddedHandler(string sender, string message);

    /// Delegate for processing user input through a pipeline.

    public delegate string InputProcessorDelegate(string input);
}
