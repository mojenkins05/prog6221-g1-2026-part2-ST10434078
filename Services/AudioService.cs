using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;

namespace POEProg6221.GUI.Services
{
    /// Handles audio file playback for the welcome greeting.

    public class AudioService
    {
        private readonly string _audioFilePath;

        public AudioService(string audioFilePath)
        {
            _audioFilePath = audioFilePath;
        }

        /// Plays the welcome audio file asynchronously.
        /// Returns true if played successfully, false otherwise.
 
        public async Task<bool> PlayWelcomeAudioAsync()
        {
            try
            {
                if (!File.Exists(_audioFilePath))
                {
                    return false;
                }

                await Task.Run(() =>
                {
                    using (var audioFile = new AudioFileReader(_audioFilePath))
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
            catch (Exception)
            {
                return false;
            }
        }

        /// Checks if the audio file exists.

        public bool AudioFileExists()
        {
            return File.Exists(_audioFilePath);
        }
    }
}
