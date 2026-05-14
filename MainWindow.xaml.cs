using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using POEProg6221.GUI.Delegates;
using POEProg6221.GUI.Models;
using POEProg6221.GUI.Services;

namespace POEProg6221.GUI
{
    // Main window for the Wooppaa Cybersecurity Awareness Chatbot.
    // Manages the GUI, user interaction, and coordinates all services.

    public partial class MainWindow : Window
    {
        // ─── Services ──────────────────────────────────────────
        private KeywordService _keywordService;
        private ResponseService _responseService;
        private SentimentService _sentimentService;
        private MemoryService _memoryService;
        private SpeechService _speechService;
        private AudioService _audioService;

        // ─── Delegates ─────────────────────────────────────────
        private ResponseHandler _getResponse;
        private SentimentHandler _detectSentiment;
        private MemoryRecallHandler _recallMemory;
        private MessageAddedHandler _onMessageAdded;
        private InputProcessorDelegate _processInput;

        // ─── State ─────────────────────────────────────────────
        private List<ChatMessage> _chatHistory;
        private bool _isConversationStarted;
        private DispatcherTimer _sessionTimer;
        private DateTime _sessionStart;

        public MainWindow()
        {
            InitializeComponent();
            VoiceToggle.Checked += VoiceToggle_Changed;
            VoiceToggle.Unchecked += VoiceToggle_Changed;
            InitialiseServices();
            InitialiseDelegates();
            _chatHistory = new List<ChatMessage>();
            _isConversationStarted = false;
        }

        // ═══════════════════════════════════════════════════════
        // INITIALISATION
        // ═══════════════════════════════════════════════════════


        // Initialises all service classes.

        private void InitialiseServices()
        {
            _keywordService = new KeywordService();
            _responseService = new ResponseService();
            _sentimentService = new SentimentService();
            _memoryService = new MemoryService();
            _speechService = new SpeechService();

            // Set audio file path - adjust as needed
            string audioPath = System.IO.Path.Combine(
     AppDomain.CurrentDomain.BaseDirectory, "Assets", "VoiceGreeting.wav");
            _audioService = new AudioService(audioPath);
        }

        // Initialises delegates for processing pipeline.

        private void InitialiseDelegates()
        {
            // Delegate for getting a response based on identified topic
            _getResponse = (input) =>
            {
                string topic = _keywordService.IdentifyTopic(input);
                if (topic != null)
                {
                    _memoryService.RecordTopicDiscussion(topic);
                    string response = _responseService.GetTopicResponse(topic);
                    string personalised = _memoryService.GetPersonalisedComment(topic);
                    return response + personalised;
                }
                return null;
            };

            // Delegate for sentiment detection
            _detectSentiment = (input) =>
            {
                return _sentimentService.DetectSentiment(input);
            };

            // Delegate for memory recall
            _recallMemory = (topic) =>
            {
                return _memoryService.GetMemorySummary();
            };

            // Delegate for message added event
            _onMessageAdded = (sender, message) =>
            {
                UpdateStatusIndicators();
            };

            // Delegate for input processing pipeline
            _processInput = (input) =>
            {
                return input?.Trim().ToLower() ?? "";
            };
        }

        /// Window loaded event - plays welcome audio.

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus the name input
            NameInput.Focus();

            // Show that we're attempting to play audio
            StatusText.Text = "🔊 Loading welcome audio...";

            // Try to play welcome audio
            bool audioExists = _audioService.AudioFileExists();

            if (audioExists)
            {
                StatusText.Text = "🔊 Playing welcome audio...";
                bool played = await _audioService.PlayWelcomeAudioAsync();

                if (played)
                {
                    StatusText.Text = "💡 Type 'help' for topics | 'quit' to exit | 'wooppaa' for fun!";
                }
                else
                {
                    StatusText.Text = "⚠️ Audio could not play. Type 'help' for topics!";
                }
            }
            else
            {
                StatusText.Text = "⚠️ VoiceGreeting.wav not found. Type 'help' for topics!";

                // Show a helpful message in chat
                MessageBox.Show(
                    "VoiceGreeting.wav not found!\n\n" +
                    "Please ensure the file is in your project and set to:\n" +
                    "• Build Action: Content\n" +
                    "• Copy to Output Directory: Copy if newer\n\n" +
                    $"Looking in: {AppDomain.CurrentDomain.BaseDirectory}",
                    "Audio File Missing",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// Window closing event - cleanup resources.

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _speechService?.Stop();
            _speechService?.Dispose();
            _sessionTimer?.Stop();
        }

        // ═══════════════════════════════════════════════════════
        // NAME INPUT & CHAT START
        // ═══════════════════════════════════════════════════════

     
        /// Handles Enter key in name input.

        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartChat();
            }
        }


        /// Handles Start Chat button click.

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            StartChat();
        }


        /// Starts the chat session with the entered name.

        private void StartChat()
        {
            string name = NameInput.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                AddBotMessage("❌ Please enter a valid name to get started!");
                return;
            }

            _memoryService.SetUserName(name);
            _isConversationStarted = true;
            _sessionStart = DateTime.Now;

            // Switch panels
            NameInputPanel.Visibility = Visibility.Collapsed;
            MessageInputPanel.Visibility = Visibility.Visible;

            // Update header
            WelcomeText.Text = $"Chatting with {name} | I'm here to keep you safe online!";

            // Start session timer
            StartSessionTimer();

            // Welcome messages
            string welcomeMessage = $"🎉 Wooppaa {name}! Welcome to the Cybersecurity Awareness Bot!\n\n" +
                                    "I'm here to help you learn about cybersecurity and staying safe online.\n\n" +
                                    "You can ask me about:\n" +
                                    "🔑 Passwords  |  📧 Phishing  |  🛡️ Scams  |  🔐 Privacy\n" +
                                    "🌐 Safe Browsing  |  📱 2FA  |  🦠 Malware  |  📶 Wi-Fi Safety\n\n" +
                                    "Or click any of the quick topic buttons above! Type 'help' anytime for assistance.";

            AddBotMessage(welcomeMessage);

            // Voice welcome
            if (_speechService.IsEnabled && VoiceToggle.IsChecked == true)
            {
                _speechService.SpeakWelcome(name);
            }

            MessageInput.Focus();
        }

        /// Starts the session duration timer display.

        private void StartSessionTimer()
        {
            _sessionTimer = new DispatcherTimer();
            _sessionTimer.Interval = TimeSpan.FromSeconds(30);
            _sessionTimer.Tick += (s, e) =>
            {
                TimeSpan duration = DateTime.Now - _sessionStart;
                TimerText.Text = $"Session: {duration.Minutes}m {duration.Seconds}s";
            };
            _sessionTimer.Start();
        }

        // ═══════════════════════════════════════════════════════
        // MESSAGE HANDLING
        // ═══════════════════════════════════════════════════════


        /// Handles Enter key in message input.

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserMessage();
            }
        }


        /// Handles Send button click.

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            ProcessUserMessage();
        }


        /// Main message processing method - coordinates all services via delegates.

        private void ProcessUserMessage()
        {
            // Process user input through a pipeline of delegates:
            // 1. Sentiment detection (Addison, 2022)
            // 2. Memory and interest detection (Nugroho, 2021)
            // 3. Keyword-based response generation
            // This modular approach improves maintainability and meets 
            // the requirement to use delegates (Microsoft, 2024b).
            string rawInput = MessageInput.Text;
            MessageInput.Clear();
            MessageInput.Focus();

            // Process input through delegate pipeline
            string input = _processInput(rawInput);

            if (string.IsNullOrWhiteSpace(input))
            {
                AddBotMessage("I didn't catch that! Please type something.");
                return;
            }

            // Add user message to chat
            string userName = _memoryService.GetUserName();
            AddUserMessage(rawInput);

            // ─── Handle special commands ───
            if (HandleSpecialCommands(input)) return;

            // ─── Detect sentiment via delegate ───
            string sentiment = _detectSentiment(input);
            _memoryService.RecordSentiment(sentiment);
            UpdateSentimentDisplay(sentiment);

            // ─── Check for interest/memory storage ───
            string interestResponse = _memoryService.DetectAndStoreInterest(input);

            // ─── Check for follow-up requests ───
            if (IsFollowUpRequest(input))
            {
                HandleFollowUpRequest(input, sentiment, interestResponse);
                return;
            }

            // ─── Get response via delegate ───
            string topicResponse = _getResponse(input);

            if (topicResponse != null)
            {
                // Build sentiment-aware response
                string fullResponse = "";

                if (!string.IsNullOrEmpty(interestResponse))
                {
                    fullResponse += interestResponse + "\n\n";
                }

                if (sentiment != "neutral")
                {
                    fullResponse += _sentimentService.BuildSentimentAwareResponse(sentiment, topicResponse);
                }
                else
                {
                    fullResponse += topicResponse;
                }

                AddBotMessage(fullResponse);
                SpeakIfEnabled(topicResponse);
            }
            else
            {
                // No topic found - check if there's an interest acknowledgement
                if (!string.IsNullOrEmpty(interestResponse))
                {
                    AddBotMessage(interestResponse);
                    SpeakIfEnabled(interestResponse);
                }
                else
                {
                    // Default response for unrecognised input
                    string defaultResponse = _responseService.GetDefaultResponse();
                    AddBotMessage(defaultResponse);
                    SpeakIfEnabled(defaultResponse);
                }
            }

            // Trigger message added delegate
            _onMessageAdded?.Invoke(userName, rawInput);
        }

        /// Handles special commands like help, quit, wooppaa.
        /// Returns true if a special command was handled.

        private bool HandleSpecialCommands(string input)
        {
            if (input == "quit" || input == "exit" || input == "bye")
            {
                string userName = _memoryService.GetUserName();
                string goodbye = $"Goodbye {userName}! 👋\n\n" +
                                 _memoryService.GetMemorySummary() +
                                 "\n\nStay safe online! Wooppaa! 🎉";
                AddBotMessage(goodbye);

                if (_speechService.IsEnabled && VoiceToggle.IsChecked == true)
                {
                    _speechService.SpeakGoodbye(userName);
                }

                // Disable input
                MessageInput.IsEnabled = false;
                StatusText.Text = "💤 Chat session ended. Close the window to exit.";
                return true;
            }

            if (input == "help" || input == "menu" || input == "topics")
            {
                string helpText = "📋 Here are the topics I can help you with:\n\n" +
                                  "🔑 Passwords — Learn about creating and managing strong passwords\n" +
                                  "📧 Phishing — How to identify and avoid phishing attacks\n" +
                                  "🛡️ Scams — Recognise and protect yourself from online scams\n" +
                                  "🔐 Privacy — Protecting your personal data and privacy online\n" +
                                  "🌐 Safe Browsing — Tips for browsing the internet safely\n" +
                                  "📱 Two-Factor Authentication (2FA) — Adding extra security to your accounts\n" +
                                  "🦠 Malware — Understanding and preventing malware infections\n" +
                                  "📶 Wi-Fi Safety — Securing your wireless connections\n" +
                                  "📱 Social Media — Staying safe on social platforms\n\n" +
                                  "You can type a topic name, ask a question, or click the quick buttons above!";
                AddBotMessage(helpText);
                SpeakIfEnabled("Here are the topics I can help you with. You can ask about passwords, phishing, scams, privacy, safe browsing, two-factor authentication, malware, Wi-Fi safety, or social media.");
                return true;
            }

            if (input.Contains("wooppaa"))
            {
                string[] funResponses = {
                    "🎉 WOOPPAA! You're awesome for learning about cybersecurity! Keep it up!",
                    "🎉 WOOPPAA! That's the spirit! Let's keep learning and staying safe!",
                    "🎉 WOOPPAA! You've got this! Cybersecurity knowledge is power!",
                    "🎉 WOOPPAA right back at you! You're on your way to becoming a cyber-hero!"
                };
                Random rand = new Random();
                string response = funResponses[rand.Next(funResponses.Length)];
                AddBotMessage(response);
                SpeakIfEnabled("Wooppaa! You're awesome!");
                return true;
            }

            return false;
        }

        /// Checks if the user input is a follow-up request.

        private bool IsFollowUpRequest(string input)
        {
            string[] followUpPhrases = {
                "tell me more", "more", "another tip", "give me another",
                "explain more", "more info", "more information", "continue",
                "go on", "what else", "another one", "keep going",
                "more details", "elaborate", "can you explain"
            };

            foreach (string phrase in followUpPhrases)
            {
                if (input.Contains(phrase)) return true;
            }

            return false;
        }

        /// Handles follow-up requests by continuing the last topic.

        private void HandleFollowUpRequest(string input, string sentiment, string interestResponse)
        {
            string lastTopic = _memoryService.GetLastTopic();

            // Check if user is asking about a specific new topic in the follow-up
            string newTopic = _keywordService.IdentifyTopic(input);
            if (newTopic != null)
            {
                lastTopic = newTopic;
                _memoryService.RecordTopicDiscussion(newTopic);
            }

            if (!string.IsNullOrEmpty(lastTopic))
            {
                string followUpResponse = _responseService.GetFollowUpResponse(lastTopic);
                string personalised = _memoryService.GetPersonalisedComment(lastTopic);

                string fullResponse = "";

                if (!string.IsNullOrEmpty(interestResponse))
                {
                    fullResponse += interestResponse + "\n\n";
                }

                if (sentiment != "neutral")
                {
                    fullResponse += _sentimentService.BuildSentimentAwareResponse(sentiment, followUpResponse + personalised);
                }
                else
                {
                    fullResponse += followUpResponse + personalised;
                }

                AddBotMessage(fullResponse);
                SpeakIfEnabled(followUpResponse);
            }
            else
            {
                AddBotMessage("I'd love to tell you more, but we haven't started a topic yet! Try asking about passwords, phishing, privacy, or any other cybersecurity topic.");
                SpeakIfEnabled("We haven't started a topic yet. Try asking about a cybersecurity topic.");
            }
        }

        // ═══════════════════════════════════════════════════════
        // TOPIC BUTTON HANDLERS
        // ═══════════════════════════════════════════════════════

        /// Handles quick topic button clicks.

        private void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConversationStarted) return;

            Button button = sender as Button;
            string topic = button?.Tag?.ToString();

            if (string.IsNullOrEmpty(topic)) return;

            // Add user message showing what topic was clicked
            AddUserMessage($"Tell me about {topic}");

            // Get and display response
            _memoryService.RecordTopicDiscussion(topic);
            string response = _responseService.GetTopicResponse(topic);
            string personalised = _memoryService.GetPersonalisedComment(topic);

            AddBotMessage(response + personalised);
            SpeakIfEnabled(response);

            UpdateStatusIndicators();
            MessageInput.Focus();
        }

        /// Handles the memory recall button click.

        private void MemoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConversationStarted) return;

            AddUserMessage("What do you remember about me?");

            // Use recall delegate
            string summary = _recallMemory("all");
            AddBotMessage("🧠 Here's what I remember:\n\n" + summary);
            SpeakIfEnabled(summary);

            MessageInput.Focus();
        }

        // ═══════════════════════════════════════════════════════
        // QUIT HANDLER
        // ═══════════════════════════════════════════════════════

        // Handles the quit button click.
 
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = _memoryService.GetUserName();
            string goodbye = $"Goodbye {userName}! 👋\n\n" +
                             _memoryService.GetMemorySummary() +
                             "\n\nStay safe online! Wooppaa! 🎉";
            AddBotMessage(goodbye);

            if (_speechService.IsEnabled && VoiceToggle.IsChecked == true)
            {
                _speechService.SpeakGoodbye(userName);
            }

            MessageInput.IsEnabled = false;
            StatusText.Text = "💤 Chat session ended. Close the window to exit.";
        }

        // ═══════════════════════════════════════════════════════
        // VOICE TOGGLE
        // ═══════════════════════════════════════════════════════

        /// Handles voice toggle checkbox changes.

        private void VoiceToggle_Changed(object sender, RoutedEventArgs e)
        {
            // Null check - the event fires during initialization before controls are loaded
            if (VoiceStatusText == null || _speechService == null)
                return;

            if (VoiceToggle.IsChecked == true)
            {
                VoiceStatusText.Text = "ON";
                VoiceStatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D4AA"));
                _speechService.IsEnabled = true;
            }
            else
            {
                VoiceStatusText.Text = "OFF";
                VoiceStatusText.Foreground = new SolidColorBrush(Colors.Gray);
                _speechService.IsEnabled = false;
                _speechService.Stop();
            }
        }

        // ═══════════════════════════════════════════════════════
        // CHAT DISPLAY METHODS
        // ═══════════════════════════════════════════════════════

        // Adds a user message to the chat panel.

        private void AddUserMessage(string message)
        {
            string userName = _memoryService.GetUserName() ?? "User";
            ChatMessage chatMsg = new ChatMessage(userName, message, false);
            _chatHistory.Add(chatMsg);

            // Create message UI
            Border messageBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D44")),
                CornerRadius = new CornerRadius(10, 10, 0, 10),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(150, 5, 10, 5),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            StackPanel msgPanel = new StackPanel();

            TextBlock senderBlock = new TextBlock
            {
                Text = $"👤 {userName}",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00D4AA")),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 3)
            };

            TextBlock messageBlock = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap
            };

            TextBlock timeBlock = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 3, 0, 0)
            };

            msgPanel.Children.Add(senderBlock);
            msgPanel.Children.Add(messageBlock);
            msgPanel.Children.Add(timeBlock);
            messageBorder.Child = msgPanel;

            ChatPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        // Adds a bot message to the chat panel.

        private void AddBotMessage(string message)
        {
            ChatMessage chatMsg = new ChatMessage("Wooppaa Bot", message, true);
            _chatHistory.Add(chatMsg);

            Border messageBorder = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F3460")),
                CornerRadius = new CornerRadius(10, 10, 10, 0),
                Padding = new Thickness(12, 8, 12, 8),
                Margin = new Thickness(10, 5, 150, 5),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            StackPanel msgPanel = new StackPanel();

            TextBlock senderBlock = new TextBlock
            {
                Text = "🤖 Wooppaa Bot",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD700")),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 3)
            };

            TextBlock messageBlock = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            TextBlock timeBlock = new TextBlock
            {
                Text = DateTime.Now.ToString("HH:mm"),
                FontSize = 9,
                Foreground = new SolidColorBrush(Colors.Gray),
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 3, 0, 0)
            };

            msgPanel.Children.Add(senderBlock);
            msgPanel.Children.Add(messageBlock);
            msgPanel.Children.Add(timeBlock);
            messageBorder.Child = msgPanel;

            ChatPanel.Children.Add(messageBorder);
            ScrollToBottom();
        }

        // Scrolls the chat to the bottom to show the latest message.

        private void ScrollToBottom()
        {
            ChatScrollViewer.Dispatcher.BeginInvoke(
                DispatcherPriority.Loaded,
                new Action(() => ChatScrollViewer.ScrollToEnd()));
        }

        // ═══════════════════════════════════════════════════════
        // UI UPDATE METHODS
        // ═══════════════════════════════════════════════════════


        // Updates the sentiment display indicator based on detected sentiment.

        private void UpdateSentimentDisplay(string sentiment)
        {
            string emoji;
            string colour;

            switch (sentiment)
            {
                case "worried":
                    emoji = "😟";
                    colour = "#FF6B6B";
                    break;
                case "curious":
                    emoji = "🤔";
                    colour = "#4ECDC4";
                    break;
                case "frustrated":
                    emoji = "😤";
                    colour = "#FF8C42";
                    break;
                case "happy":
                    emoji = "😊";
                    colour = "#00D4AA";
                    break;
                case "overwhelmed":
                    emoji = "😰";
                    colour = "#C44DFF";
                    break;
                default:
                    emoji = "😊";
                    colour = "#FFD700";
                    break;
            }

            SentimentIndicator.Text = $"Mood: {emoji} {char.ToUpper(sentiment[0]) + sentiment.Substring(1)}";
            SentimentIndicator.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colour));
        }

        // Updates memory and topic indicators in the header.

        private void UpdateStatusIndicators()
        {
            int topicCount = _memoryService.GetTopicsDiscussed().Count;
            MemoryIndicator.Text = $"Topics discussed: {topicCount}";

            string favTopic = _memoryService.GetFavouriteTopic();
            if (!string.IsNullOrEmpty(favTopic))
            {
                MemoryIndicator.Text += $" | Favourite: {favTopic}";
            }
        }

        //Speaks text if voice is enabled.

        private void SpeakIfEnabled(string text)
        {
            if (_speechService.IsEnabled && VoiceToggle.IsChecked == true)
            {
                _speechService.SpeakAsync(text);
            }
        }
    }
}/* Addison, T. (2022) ‘Sentiment analysis techniques for conversational agents’, Journal of Artificial Intelligence in Education, 14(3), pp. 112–129.

    Gamma, E., Helm, R., Johnson, R. and Vlissides, J. (1995) Design patterns: elements of reusable object-oriented software. Reading, MA: Addison-Wesley.

    Microsoft (2023) Windows Presentation Foundation (WPF). Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/ (Accessed: 14 May 2026).

    Microsoft (2024a) SpeechSynthesizer Class. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.speech.synthesis.speechsynthesizer (Accessed: 14 May 2026).

    Microsoft (2024b) Delegates - C# programming guide. Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/ (Accessed: 14 May 2026).

    National Cyber Security Centre (2024) Cyber security for small businesses and individuals. Available at: https://www.ncsc.gov.uk/collection/cyber-awareness (Accessed: 14 May 2026).

    Nugroho, A. (2021) Object-oriented programming concepts in C#. 2nd edn. Birmingham: Packt Publishing.

    Russell, S. and Norvig, P. (2021) Artificial intelligence: a modern approach. 4th edn. Harlow: Pearson.

    Troelsen, A. and Japikse, P. (2022) Pro C# 10 with .NET 6: foundational principles and practices in programming. 11th edn. Berkeley, CA: Apress.
 */