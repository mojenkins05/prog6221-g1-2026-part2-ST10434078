using System;
using System.Collections.Generic;
using POEProg6221.GUI.Models;

namespace POEProg6221.GUI.Services
{
    /// Manages user memory, recall, and personalisation features.

    public class MemoryService
    {
        private readonly UserMemory _memory;
        private readonly Random _random;

        // Chance to include a memory-based personalisation (percentage)
        private const int MemoryRecallChance = 30;

        public MemoryService()
        {
            _memory = new UserMemory();
            _random = new Random();
        }

        /// Sets the user's name in memory.

        public void SetUserName(string name)
        {
            _memory.UserName = name;
        }

        /// Gets the user's name from memory.

        public string GetUserName()
        {
            return _memory.UserName;
        }

        /// Records that a topic was discussed.

        public void RecordTopicDiscussion(string topic)
        {
            _memory.RecordTopic(topic);
        }

        /// Records the user's sentiment.

        public void RecordSentiment(string sentiment)
        {
            _memory.LastSentiment = sentiment;
        }

        /// Gets the last topic discussed.

        public string GetLastTopic()
        {
            return _memory.LastTopic;
        }

        /// Gets the user's favourite topic.

        public string GetFavouriteTopic()
        {
            return _memory.FavouriteTopic;
        }

        /// Sets the user's favourite topic explicitly (when they state interest).

        public void SetFavouriteTopic(string topic)
        {
            _memory.FavouriteTopic = topic;
        }

        /// Stores a custom memory key-value pair.

        public void Remember(string key, string value)
        {
            _memory.Remember(key, value);
        }

        /// Recalls a custom memory value.

        public string Recall(string key)
        {
            return _memory.Recall(key);
        }

        /// Gets a personalised comment based on memory, to append to responses occasionally.

        public string GetPersonalisedComment(string currentTopic)
        {
            // Only add personalisation some of the time to keep it natural
            if (_random.Next(100) > MemoryRecallChance) return "";

            List<string> comments = new List<string>();

            // Reference favourite topic if discussing something else
            if (!string.IsNullOrEmpty(_memory.FavouriteTopic) &&
                !string.IsNullOrEmpty(currentTopic) &&
                !_memory.FavouriteTopic.Equals(currentTopic, StringComparison.OrdinalIgnoreCase))
            {
                comments.Add($"By the way, as someone interested in {_memory.FavouriteTopic}, this topic connects well with your interests!");
            }

            // Reference how many topics they've explored
            if (_memory.TopicsDiscussed.Count > 2)
            {
                comments.Add($"You've explored {_memory.TopicsDiscussed.Count} topics so far — you're becoming quite the cybersecurity expert, {_memory.UserName}!");
            }

            // Reference session duration
            TimeSpan sessionDuration = DateTime.Now - _memory.SessionStart;
            if (sessionDuration.TotalMinutes > 5)
            {
                comments.Add($"We've been chatting for {sessionDuration.Minutes} minutes now — great dedication to learning about cybersecurity!");
            }

            if (comments.Count == 0) return "";

            return "\n\n💡 " + comments[_random.Next(comments.Count)];
        }


        /// Gets the full memory summary.

        public string GetMemorySummary()
        {
            return _memory.GetMemorySummary();
        }


        /// Gets all topics discussed.

        public List<string> GetTopicsDiscussed()
        {
            return _memory.TopicsDiscussed;
        }

        /// Checks if user has expressed interest in a topic.

        public bool HasDiscussedTopic(string topic)
        {
            return _memory.TopicsDiscussed.Contains(topic.ToLower());
        }

        /// Detects if the user is expressing interest in a topic and stores it.
        /// Returns a memory acknowledgement string or empty.

        public string DetectAndStoreInterest(string input)
        {
            string lowerInput = input.ToLower();

            // Detect patterns like "I'm interested in X" or "I like X"
            string[] interestPatterns = {
                "interested in", "i like", "i love", "fascinated by",
                "passionate about", "curious about", "want to learn about"
            };

            foreach (string pattern in interestPatterns)
            {
                if (lowerInput.Contains(pattern))
                {
                    // Extract the topic after the pattern
                    int index = lowerInput.IndexOf(pattern) + pattern.Length;
                    string interest = lowerInput.Substring(index).Trim().TrimEnd('.', '!', '?');

                    if (!string.IsNullOrWhiteSpace(interest))
                    {
                        _memory.FavouriteTopic = interest;
                        _memory.Remember("interest", interest);
                        return $"Great! I'll remember that you're interested in {interest}. It's a crucial part of staying safe online.";
                    }
                }
            }

            return "";
        }
    }
}