using System;
using System.Speech.Synthesis;

namespace POEProg6221.GUI.Services
{
    /// <summary>
    /// Manages text-to-speech functionality for the chatbot.
    /// </summary>
    public class SpeechService
    {
        private readonly SpeechSynthesizer _synth;
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public SpeechService()
        {
            try
            {
                _synth = new SpeechSynthesizer();
                _synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
                _synth.Rate = 1;
                _synth.Volume = 100;
                _isEnabled = true;
            }
            catch (Exception)
            {
                _isEnabled = false;
            }
        }

        /// <summary>
        /// Speaks the given text asynchronously (non-blocking).
        /// </summary>
        public void SpeakAsync(string text)
        {
            if (!_isEnabled || _synth == null || string.IsNullOrWhiteSpace(text)) return;

            try
            {
                _synth.SpeakAsyncCancelAll();

                // Clean text for speech (remove emojis and special characters)
                string cleanText = CleanTextForSpeech(text);
                _synth.SpeakAsync(cleanText);
            }
            catch (Exception)
            {
                // Silently fail — speech is a nice-to-have
            }
        }

        /// <summary>
        /// Stops any current speech.
        /// </summary>
        public void Stop()
        {
            try
            {
                _synth?.SpeakAsyncCancelAll();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Speaks a welcome message for the user.
        /// </summary>
        public void SpeakWelcome(string userName)
        {
            SpeakAsync($"Hello {userName}! Welcome to the Wooppaa Cybersecurity Awareness Bot! I'm here to help you stay safe online.");
        }

        /// <summary>
        /// Speaks a goodbye message for the user.
        /// </summary>
        public void SpeakGoodbye(string userName)
        {
            SpeakAsync($"Goodbye {userName}! Stay safe online! Wooppaa!");
        }

        /// <summary>
        /// Cleans text by removing emojis and special formatting characters for cleaner speech.
        /// </summary>
        private string CleanTextForSpeech(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Remove common emoji patterns and special characters
            string cleaned = text;
            string[] removePatterns = { "🔒", "🤖", "👤", "💬", "🎉", "🗣️", "📋", "🔑", "📧", "🌐", "🔐",
                                        "✅", "❌", "⚠️", "💡", "🔊", "║", "╔", "╗", "╚", "╝", "═", "░",
                                        "▄", "▀", "█", "*" };

            foreach (string pattern in removePatterns)
            {
                cleaned = cleaned.Replace(pattern, "");
            }

            return cleaned.Trim();
        }

        /// <summary>
        /// Disposes of the speech synthesizer.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _synth?.SpeakAsyncCancelAll();
                _synth?.Dispose();
            }
            catch (Exception) { }
        }
    }
}