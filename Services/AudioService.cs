using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace POEProg6221.GUI.Services
{
    public class AudioService
    {
        private string _audioFilePath;

        public AudioService(string audioFilePath)
        {
            _audioFilePath = audioFilePath;
        }

        public async Task<bool> PlayWelcomeAudioAsync()
        {
            try
            {
                // Try multiple possible paths
                string[] possiblePaths = {
                    _audioFilePath,
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VoiceGreeting.wav"),
                    Path.Combine(Directory.GetCurrentDirectory(), "VoiceGreeting.wav"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "VoiceGreeting.wav"),
                    @"C:\Users\muham\source\repos\prog6221-g1-2026-part1-mojenkins05\POEProg6221\VoiceGreeting.wav"
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        System.Diagnostics.Debug.WriteLine($"✅ Found audio at: {path}");
                        break;
                    }
                }

                if (foundPath == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ VoiceGreeting.wav NOT FOUND in any location!");
                    System.Diagnostics.Debug.WriteLine($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
                    return false;
                }

                await Task.Run(() =>
                {
                    using (var audioFile = new AudioFileReader(foundPath))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Audio Error: {ex.Message}");
                return false;
            }
        }

        public bool AudioFileExists()
        {
            // Check multiple locations
            string[] possiblePaths = {
                _audioFilePath,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VoiceGreeting.wav"),
                Path.Combine(Directory.GetCurrentDirectory(), "VoiceGreeting.wav"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "VoiceGreeting.wav")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path)) return true;
            }
            return false;
        }
    }
}