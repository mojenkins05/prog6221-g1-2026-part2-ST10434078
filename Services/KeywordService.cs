using System;
using System.Collections.Generic;
using System.Linq;

namespace POEProg6221.GUI.Services
{

    /// Manages keyword recognition for cybersecurity topics.
    /// Maps keywords to topic categories for response routing.

    public class KeywordService
    {
        // Dictionary mapping topic names to their associated keywords
        private readonly Dictionary<string, List<string>> _topicKeywords;

        public KeywordService()
        {
            _topicKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string>
                {
                    "password", "passwords", "pass word", "passcode", "login",
                    "credential", "credentials", "authentication", "strong password"
                },
                ["phishing"] = new List<string>
                {
                    "phishing", "phish", "scam email", "fake email", "suspicious email",
                    "email scam", "spear phishing", "social engineering"
                },
                ["scam"] = new List<string>
                {
                    "scam", "scams", "fraud", "fraudulent", "fake", "con",
                    "swindle", "trick", "deceive", "deception", "rip off"
                },
                ["privacy"] = new List<string>
                {
                    "privacy", "private", "personal data", "data protection",
                    "personal information", "tracking", "surveillance", "data privacy"
                },
                ["browsing"] = new List<string>
                {
                    "browsing", "browse", "internet", "web", "website", "online",
                    "safe browsing", "https", "url", "link", "download"
                },
                ["twofactor"] = new List<string>
                {
                    "2fa", "two factor", "two-factor", "mfa", "multi factor",
                    "multi-factor", "authenticator", "verification code", "otp"
                },
                ["malware"] = new List<string>
                {
                    "malware", "virus", "trojan", "ransomware", "spyware",
                    "adware", "worm", "infected", "antivirus", "anti-virus"
                },
                ["wifi"] = new List<string>
                {
                    "wifi", "wi-fi", "wireless", "hotspot", "public wifi",
                    "network", "vpn", "router"
                },
                ["socialmedia"] = new List<string>
                {
                    "social media", "facebook", "instagram", "twitter", "tiktok",
                    "snapchat", "linkedin", "social network", "profile", "posting"
                }
            };
        }


        /// Identifies which cybersecurity topic the user input relates to.
        /// Returns the topic name or null if no topic matched.

        public string IdentifyTopic(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            string lowerInput = input.ToLower();

            foreach (var topic in _topicKeywords)
            {
                foreach (string keyword in topic.Value)
                {
                    if (lowerInput.Contains(keyword))
                    {
                        return topic.Key;
                    }
                }
            }

            return null;
        }

        /// Returns all available topic names.

        public List<string> GetAllTopics()
        {
            return _topicKeywords.Keys.ToList();
        }

        /// Checks if a specific topic exists.

        public bool TopicExists(string topic)
        {
            return _topicKeywords.ContainsKey(topic);
        }

        /// Gets the keywords associated with a topic.

        public List<string> GetKeywordsForTopic(string topic)
        {
            if (_topicKeywords.ContainsKey(topic))
                return _topicKeywords[topic];
            return new List<string>();
        }
    }
}