using System;
using System.Collections.Generic;
using System.Linq;

namespace POEProg6221.GUI.Services
{ /// <summary>
/// SentimentService implements simple keyword-based sentiment analysis 
/// (worried, curious, frustrated, happy, overwhelmed). 
/// The service adjusts responses to be more empathetic, improving 
/// user engagement as discussed in conversational agent design 
/// (Addison, 2022; Russell and Norvig, 2021).
/// </summary>

    /// Detects user sentiment from input text and provides appropriate emotional responses.

    public class SentimentService
    {/// <summary>
     /// Detects the sentiment of the user input based on predefined keyword lists.
     /// Returns the sentiment category or "neutral" if none detected.
     /// </summary>
     /// <param name="input">The raw text input from the user.</param>
     /// <returns>A string representing the detected sentiment.</returns>
        // Dictionary mapping sentiment categories to their trigger words
        private readonly Dictionary<string, List<string>> _sentimentKeywords;

        // Dictionary mapping sentiments to empathetic prefix responses
        private readonly Dictionary<string, List<string>> _sentimentResponses;

        private readonly Random _random;

        public SentimentService()
        {
            _random = new Random();
            _sentimentKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["worried"] = new List<string>
                {
                    "worried", "worry", "anxious", "nervous", "scared", "afraid",
                    "fear", "frightened", "concerned", "uneasy", "stressed", "panic"
                },
                ["curious"] = new List<string>
                {
                    "curious", "wonder", "wondering", "interested", "tell me",
                    "how does", "what is", "explain", "learn", "know more", "want to know"
                },
                ["frustrated"] = new List<string>
                {
                    "frustrated", "annoyed", "angry", "mad", "irritated", "confused",
                    "don't understand", "makes no sense", "ugh", "hate", "stupid",
                    "complicated", "difficult", "hard", "can't"
                },
                ["happy"] = new List<string>
                {
                    "happy", "great", "awesome", "thanks", "thank you", "cool",
                    "wonderful", "excellent", "love", "amazing", "fantastic", "good"
                },
                ["overwhelmed"] = new List<string>
                {
                    "overwhelmed", "too much", "so much", "lost", "helpless",
                    "give up", "impossible", "overloaded", "swamped"
                }
            };

            _sentimentResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["worried"] = new List<string>
                {
                    "It's completely understandable to feel that way. Let me help you feel more secure.",
                    "I hear your concern, and I want to reassure you. Knowledge is your best defence.",
                    "Don't worry — many people share these concerns. Let me share some tips to help you stay safe.",
                    "Your concern shows you take security seriously, which is already a great first step!"
                },
                ["curious"] = new List<string>
                {
                    "Great question! I love your curiosity about cybersecurity!",
                    "That's wonderful that you want to learn more! Here's what I can tell you:",
                    "Excellent! Being curious about security is the first step to staying safe!",
                    "I'm glad you're interested! Let me share some useful information."
                },
                ["frustrated"] = new List<string>
                {
                    "I understand this can be frustrating. Let me try to explain it more simply.",
                    "I'm sorry you're feeling that way. Let's take this step by step together.",
                    "Don't give up! Cybersecurity can seem complex, but I'll break it down for you.",
                    "I know it can feel overwhelming. Let's simplify things and focus on what matters most."
                },
                ["happy"] = new List<string>
                {
                    "Wonderful! I'm glad you're feeling positive about cybersecurity!",
                    "That's great to hear! Your positive attitude will help you stay safe online!",
                    "Awesome! Let's keep the momentum going!",
                    "I love your enthusiasm! Here's more to keep you excited about staying secure!"
                },
                ["overwhelmed"] = new List<string>
                {
                    "Take a deep breath. We don't need to cover everything at once — let's go one step at a time.",
                    "I understand it feels like a lot. Let's focus on the most important thing first.",
                    "No need to feel overwhelmed! Even small steps make a big difference in cybersecurity.",
                    "Let's slow down and tackle this together, one topic at a time."
                }
            };
        }

        /// Detects the sentiment of the user input.
        /// Returns the sentiment category or "neutral" if none detected.

        public string DetectSentiment(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "neutral";

            string lowerInput = input.ToLower();

            // Check each sentiment category
            foreach (var sentimentCategory in _sentimentKeywords)
            {
                foreach (string keyword in sentimentCategory.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        return sentimentCategory.Key;
                    }
                }
            }

            return "neutral";
        }

        /// Gets an empathetic response prefix based on detected sentiment.

        public string GetSentimentResponse(string sentiment)
        {
            if (string.IsNullOrEmpty(sentiment) || sentiment == "neutral")
                return "";

            if (_sentimentResponses.ContainsKey(sentiment))
            {
                List<string> responses = _sentimentResponses[sentiment];
                return responses[_random.Next(responses.Count)];
            }

            return "";
        }

        /// Gets a combined sentiment-aware response: empathetic prefix + the actual content.

        public string BuildSentimentAwareResponse(string sentiment, string topicResponse)
        {
            string sentimentPrefix = GetSentimentResponse(sentiment);

            if (string.IsNullOrEmpty(sentimentPrefix))
                return topicResponse;

            return $"{sentimentPrefix}\n\n{topicResponse}";
        }
    }
}