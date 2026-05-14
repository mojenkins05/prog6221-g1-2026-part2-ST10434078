using System;
using System.Collections.Generic;
using System.Linq;

namespace POEProg6221.GUI.Services
{


    ///Manages response generation with random selection for varied interactions.
    // Uses dictionaries and lists to organise keyword responses.

    public class ResponseService
    {
        // Dictionary mapping topics to lists of possible responses (for random selection)
        private readonly Dictionary<string, List<string>> _topicResponses;

        // Dictionary mapping topics to follow-up responses for "tell me more" requests
        private readonly Dictionary<string, List<string>> _followUpResponses;

        // Default responses for unrecognised input
        private readonly List<string> _defaultResponses;

        private readonly Random _random;

        // Track which response index was last used per topic to avoid immediate repeats
        private readonly Dictionary<string, int> _lastResponseIndex;

        public ResponseService()
        {
            _random = new Random();
            _lastResponseIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            // Initialize all dictionaries here in the constructor
            _topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            _followUpResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            _defaultResponses = new List<string>();

            InitialiseTopicResponses();
            InitialiseFollowUpResponses();
            InitialiseDefaultResponses();
        }

        private void InitialiseTopicResponses()
        {
            _topicResponses.Clear();

            _topicResponses.Add("password", new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords. A good password has at least 12 characters with a mix of uppercase, lowercase, numbers, and symbols.",
                "Consider using a password manager like LastPass or Bitwarden to generate and store complex passwords securely. This way you only need to remember one master password!",
                "Never reuse passwords across different accounts. If one gets compromised, all your other accounts would be at risk. Each account deserves its own unique password.",
                "A great technique is using a passphrase — a sequence of random words like 'PurpleTiger$Jumping42Rain'. It's long, strong, and easier to remember than random characters!",
                "Change your passwords immediately if you suspect any account has been compromised. Also, regularly check haveibeenpwned.com to see if your credentials have appeared in any data breaches."
            });

            _topicResponses.Add("phishing", new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations. Always check the sender's email address carefully!",
                "Never click on suspicious links in emails or messages. Hover over links to preview the URL before clicking. If it looks odd or misspelled, don't click it!",
                "Legitimate companies will never ask for your password or sensitive data via email. If an email creates urgency like 'Act now or your account will be closed!' — it's likely a phishing attempt.",
                "Watch out for emails with poor grammar, generic greetings like 'Dear Customer', or mismatched URLs. These are classic signs of phishing attempts.",
                "If you receive a suspicious email from your bank or a service provider, don't reply or click any links. Instead, contact the company directly through their official website or phone number."
            });

            _topicResponses.Add("scam", new List<string>
            {
                "Online scams come in many forms — from fake shopping websites to romance scams. Always verify the legitimacy of offers that seem too good to be true.",
                "Be wary of unsolicited calls or messages claiming you've won a prize or that there's a problem with your account. Legitimate organisations won't pressure you into immediate action.",
                "Research before you buy from unfamiliar websites. Check for reviews, look for contact information, and ensure the site uses HTTPS. If a deal seems too good to be true, it probably is!",
                "Tech support scams are very common. Microsoft, Apple, and other companies will never call you unsolicited to fix your computer. If someone calls claiming this, hang up immediately.",
                "Investment scams promise guaranteed high returns with no risk. Remember: all legitimate investments carry some risk. Never invest based on pressure or promises of guaranteed returns."
            });

            _topicResponses.Add("privacy", new List<string>
            {
                "Review your privacy settings on social media regularly. Limit who can see your posts and personal information. The less you share publicly, the safer you are.",
                "Be mindful of what personal information you share online. Your full name, birthday, address, and phone number can be used for identity theft if they fall into the wrong hands.",
                "Use privacy-focused browsers and search engines like DuckDuckGo or Brave. They don't track your searches or build advertising profiles about you.",
                "Read privacy policies before signing up for new services. Know what data they collect and how they use it. If a free app seems too good to be true, you might be paying with your data.",
                "Consider using a VPN (Virtual Private Network) to encrypt your internet connection, especially when using public Wi-Fi. This prevents others from intercepting your online activity."
            });

            _topicResponses.Add("browsing", new List<string>
            {
                "Always look for HTTPS in the URL bar before entering sensitive information. The padlock icon indicates the connection is encrypted and more secure.",
                "Keep your browser and its extensions updated to patch security vulnerabilities. Outdated software is one of the easiest ways for attackers to compromise your system.",
                "Avoid downloading files from untrusted sources. Malicious downloads can contain viruses, ransomware, or spyware that compromise your system.",
                "Use an ad blocker to prevent malicious advertisements (malvertising) from loading. Some ads can install malware just by appearing on your screen!",
                "Be careful with browser extensions — only install ones from trusted developers with good reviews. Malicious extensions can steal your data or inject unwanted content."
            });

            _topicResponses.Add("twofactor", new List<string>
            {
                "Two-factor authentication (2FA) adds an extra layer of security beyond just your password. Even if someone steals your password, they can't access your account without the second factor!",
                "Use authenticator apps like Google Authenticator or Authy instead of SMS-based 2FA when possible. SMS codes can be intercepted through SIM-swapping attacks.",
                "Enable 2FA on all accounts that offer it — especially email, banking, and social media. It's one of the most effective security measures available to you.",
                "Hardware security keys like YubiKey provide the strongest form of 2FA. They're physical devices that must be present to log in, making remote attacks virtually impossible.",
                "Keep backup codes for your 2FA in a secure location. If you lose your phone or authenticator app, these codes will help you regain access to your accounts."
            });

            _topicResponses.Add("malware", new List<string>
            {
                "Keep your antivirus software up to date and run regular scans. Modern malware can be very stealthy, so automated protection is essential.",
                "Ransomware encrypts your files and demands payment for their return. Regular backups to an external drive or cloud service are your best defence against ransomware.",
                "Be cautious with email attachments, even from people you know. Their account could be compromised and sending malware. When in doubt, verify with the sender directly.",
                "Avoid using pirated software or downloading from unofficial sources. These are common vectors for malware distribution and can compromise your entire system.",
                "Keep your operating system and all software updated. Many malware attacks exploit known vulnerabilities that have already been patched in newer versions."
            });

            _topicResponses.Add("wifi", new List<string>
            {
                "Avoid accessing sensitive accounts like banking on public Wi-Fi. Use your mobile data or a VPN for secure connections when you're away from home.",
                "Change the default password on your home router. Default passwords are well-known to attackers and can give them access to your entire network.",
                "Use WPA3 encryption on your home Wi-Fi if your router supports it. Older encryption standards like WEP are easily cracked by attackers.",
                "A VPN encrypts your internet traffic, making it much harder for anyone to intercept your data on public networks. Consider using one whenever you're on public Wi-Fi.",
                "Disable auto-connect to Wi-Fi on your devices. Attackers can set up fake hotspots with common names to trick your device into connecting automatically."
            });

            _topicResponses.Add("socialmedia", new List<string>
            {
                "Be careful about what you share on social media. Information like your location, vacation plans, or workplace can be used for social engineering attacks.",
                "Review your friends/followers list regularly. Accept connection requests only from people you actually know. Fake profiles are often used for information gathering.",
                "Adjust your social media privacy settings so only friends can see your personal information and posts. Public profiles give attackers a wealth of information to work with.",
                "Think before you post — once something is online, it can be very difficult to remove completely. Screenshots last forever, even if you delete the original post.",
                "Be sceptical of quizzes and games that ask for personal information. 'What's your mother's maiden name?' style quizzes are designed to harvest common security question answers."
            });
        }

        private void InitialiseFollowUpResponses()
        {
            _followUpResponses.Clear();

            _followUpResponses.Add("password", new List<string>
            {
                "Here's another password tip: Consider enabling breach notifications on your email. Services like 'Have I Been Pwned' will alert you if your credentials appear in a data breach.",
                "Additionally, never share your passwords with anyone — not even IT support. Legitimate support staff will never need your password to help you.",
                "One more thing about passwords: Avoid writing them on sticky notes or storing them in plain text files. If you must write them down, keep them in a locked, secure location."
            });

            _followUpResponses.Add("phishing", new List<string>
            {
                "Another phishing tip: If you're unsure about an email, forward it to your IT department or the company's official phishing report address. Most major companies have these.",
                "Also, be aware of 'spear phishing' — targeted attacks that use personal information about you to seem more convincing. These are harder to detect but follow the same principles.",
                "Remember, phishing isn't limited to email. 'Smishing' uses SMS messages and 'vishing' uses phone calls. Apply the same caution to all communication channels."
            });

            _followUpResponses.Add("scam", new List<string>
            {
                "Here's more on scam prevention: If you suspect you've been scammed, act quickly. Contact your bank, change your passwords, and report the scam to relevant authorities.",
                "Watch out for 'urgency tactics' in scams. Phrases like 'limited time offer' or 'act now' are designed to prevent you from thinking critically about the situation.",
                "Catfishing and romance scams are on the rise. Be cautious of online relationships where the person avoids video calls or always has excuses not to meet in person."
            });

            _followUpResponses.Add("privacy", new List<string>
            {
                "Here's more on privacy: Regularly Google yourself to see what information about you is publicly available. You might be surprised what you find!",
                "Consider using different email addresses for different purposes — one for personal use, one for shopping, and one for social media. This limits damage if one gets compromised.",
                "Be cautious with smart home devices. They often listen for voice commands and may store recordings. Review and delete your voice history regularly."
            });

            _followUpResponses.Add("browsing", new List<string>
            {
                "More on safe browsing: Clear your browser cookies and cache regularly. This removes tracking data and can improve both privacy and performance.",
                "Use bookmarks for frequently visited sites instead of typing URLs. This prevents typosquatting attacks where fake sites use misspelled versions of popular domains.",
                "Consider using separate browser profiles for work and personal browsing. This keeps your accounts and data compartmentalised."
            });

            _followUpResponses.Add("twofactor", new List<string>
            {
                "More on 2FA: Some services support passwordless authentication, using only biometrics or security keys. This eliminates password risks entirely!",
                "If you use SMS-based 2FA, be aware of SIM-swapping attacks. Consider switching to an authenticator app for better security.",
                "Remember to set up 2FA on your email account first — since email is often used for password resets, it's the most critical account to protect."
            });

            _followUpResponses.Add("malware", new List<string>
            {
                "More on malware protection: Consider using a sandboxed environment or virtual machine to test files you're unsure about. This isolates potential threats from your main system.",
                "Browser-based malware can exploit JavaScript. Consider using a script blocker extension for added security, though some websites may not work properly without scripts.",
                "Regularly back up your important data using the 3-2-1 rule: 3 copies, on 2 different types of media, with 1 copy stored offsite or in the cloud."
            });

            _followUpResponses.Add("wifi", new List<string>
            {
                "More on Wi-Fi safety: Create a separate guest network for visitors. This keeps your main network and connected devices isolated from guest devices.",
                "Check your router's admin panel for any unknown connected devices. If you see something you don't recognise, change your Wi-Fi password immediately.",
                "Consider disabling WPS (Wi-Fi Protected Setup) on your router. While convenient, it has known security vulnerabilities that attackers can exploit."
            });

            _followUpResponses.Add("socialmedia", new List<string>
            {
                "More on social media safety: Enable login alerts to be notified when someone accesses your account from a new device or location.",
                "Be cautious of third-party apps that request access to your social media accounts. Review and revoke permissions for apps you no longer use.",
                "Consider doing a 'digital declutter' — remove old posts, photos, and personal information that you no longer want publicly available."
            });
        }

        private void InitialiseDefaultResponses()
        {
            _defaultResponses.Clear();

            _defaultResponses.Add("I'm not sure I understand. Can you try rephrasing? You can also type 'help' to see the topics I can help with!");
            _defaultResponses.Add("I didn't quite catch that. Could you ask about a specific cybersecurity topic like passwords, phishing, or privacy?");
            _defaultResponses.Add("Hmm, I'm not familiar with that topic yet. Try asking me about password safety, scams, safe browsing, or two-factor authentication!");
            _defaultResponses.Add("I'm still learning! Could you try asking about cybersecurity topics like privacy, malware, Wi-Fi safety, or social media security?");
            _defaultResponses.Add("I want to help but I'm not sure what you mean. Type 'help' to see all the topics I can discuss!");
        }

            // Gets a random response for a given topic. Avoids repeating the last response.

        public string GetTopicResponse(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic) || !_topicResponses.ContainsKey(topic))
                return GetDefaultResponse();

            List<string> responses = _topicResponses[topic];
            int index;

            do
            {
                index = _random.Next(responses.Count);
            } while (responses.Count > 1 &&
                     _lastResponseIndex.ContainsKey(topic) &&
                     _lastResponseIndex[topic] == index);

            _lastResponseIndex[topic] = index;
            return responses[index];
        }


        // Gets a follow-up response for a topic when user asks for more information.

        public string GetFollowUpResponse(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic) || !_followUpResponses.ContainsKey(topic))
                return "I don't have more information on that specific topic right now. Would you like to explore a different cybersecurity topic?";

            List<string> responses = _followUpResponses[topic];
            return responses[_random.Next(responses.Count)];
        }


        // Gets a default response for unrecognised input.

        public string GetDefaultResponse()
        {
            return _defaultResponses[_random.Next(_defaultResponses.Count)];
        }

        /// Gets all available topic names.

        public List<string> GetAvailableTopics()
        {
            return _topicResponses.Keys.ToList();
        }
    }
}