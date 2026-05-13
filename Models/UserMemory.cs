using System;
using System.Collections.Generic;
using System.Linq;

namespace POEProg6221.GUI.Models
{
    /// Stores user information and preferences for memory/recall functionality.
    
    public class UserMemory
    {
        public string UserName { get; set; }
        public string FavouriteTopic { get; set; }
        public List<string> TopicsDiscussed { get; private set; }
        public Dictionary<string, int> TopicInteractionCount { get; private set; }
        public string LastTopic { get; set; }
        public string LastSentiment { get; set; }
        public DateTime SessionStart { get; set; }
        public Dictionary<string, string> CustomMemory { get; private set; }

        public UserMemory()
        {
            TopicsDiscussed = new List<string>();
            TopicInteractionCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            CustomMemory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            SessionStart = DateTime.Now;
            LastTopic = "";
            LastSentiment = "neutral";
            FavouriteTopic = "";
        }

        /// Records that a topic was discussed and updates interaction counts.
    
        public void RecordTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic)) return;

            topic = topic.ToLower().Trim();
            LastTopic = topic;

            if (!TopicsDiscussed.Contains(topic))
            {
                TopicsDiscussed.Add(topic);
            }

            if (TopicInteractionCount.ContainsKey(topic))
            {
                TopicInteractionCount[topic]++;
            }
            else
            {
                TopicInteractionCount[topic] = 1;
            }

            // Auto-detect favourite topic based on most interactions
            if (string.IsNullOrEmpty(FavouriteTopic) ||
                TopicInteractionCount[topic] > TopicInteractionCount.GetValueOrDefault(FavouriteTopic, 0))
            {
                FavouriteTopic = topic;
            }
        }


        /// Stores a custom key-value pair in memory.

        public void Remember(string key, string value)
        {
            CustomMemory[key] = value;
        }

        /// Retrieves a stored memory value by key.

        public string Recall(string key)
        {
            return CustomMemory.ContainsKey(key) ? CustomMemory[key] : null;
        }

        /// Gets a summary of user preferences and history.

        public string GetMemorySummary()
        {
            string summary = $"I remember that your name is {UserName}. ";

            if (!string.IsNullOrEmpty(FavouriteTopic))
            {
                summary += $"Your favourite topic seems to be {FavouriteTopic}. ";
            }

            if (TopicsDiscussed.Count > 0)
            {
                summary += $"We've discussed: {string.Join(", ", TopicsDiscussed)}. ";
            }

            TimeSpan sessionLength = DateTime.Now - SessionStart;
            summary += $"We've been chatting for {sessionLength.Minutes} minutes.";

            return summary;
        }


        /// Gets the most discussed topic.

        public string GetMostDiscussedTopic()
        {
            if (TopicInteractionCount.Count == 0) return null;
            return TopicInteractionCount.OrderByDescending(kvp => kvp.Value).First().Key;
        }
    }
}
