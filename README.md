🤖 Wooppaa Cybersecurity Awareness Bot - Part 2
📖 Overview
This project is Part 2 of the Cybersecurity Awareness Chatbot. It expands on the initial console application by introducing a fully functional Graphical User Interface (GUI) built with WPF (Windows Presentation Foundation). The chatbot is designed to be engaging, user-friendly, and educational, helping users learn about cybersecurity best practices through dynamic conversations, sentiment detection, and memory recall.

✨ Key Features
This application fulfills all Part 2 requirements:

GUI Design & Implementation: A modern, dark-themed WPF interface featuring ASCII art, dynamic chat bubbles, and quick-access topic buttons.
Voice Implementation: Integrated Text-to-Speech (TTS) for bot responses and a custom .wav file for the welcome greeting.
Keyword Recognition: Identifies cybersecurity topics (e.g., passwords, phishing, scams, privacy) and provides relevant guidance.
Random Responses: Uses generic collections (Lists) to randomly select from multiple predefined responses to keep interactions varied.
Conversation Flow: Handles follow-up requests (e.g., "tell me more", "another tip") seamlessly without restarting the conversation.
Memory and Recall: Remembers the user's name, favourite topics, and discussed subjects to personalize the experience.
Sentiment Detection: Detects user moods (worried, curious, frustrated, happy, overwhelmed) and adjusts responses empathetically.
Error Handling: Provides graceful default responses for unrecognized inputs to prevent crashes.
Code Optimisation: Utilizes Object-Oriented Programming (OOP), Delegates, and efficient data structures (Dictionaries, Lists).
🛠️ Technologies & Dependencies
Language: C#
UI Framework: WPF (Windows Presentation Foundation)
Audio Library: NAudio (via NuGet)
Speech Library: System.Speech (Native .NET Assembly)
📁 Project Structure
text

POEProg6221.GUI/
├── App.xaml & App.xaml.cs          # Application entry point and global styles
├── MainWindow.xaml & .cs           # Main GUI layout and logic
├── Models/
│   ├── ChatMessage.cs              # Represents a single chat message
│   └── UserMemory.cs               # Stores user data and preferences
├── Services/
│   ├── AudioService.cs             # Handles .wav file playback
│   ├── KeywordService.cs           # Maps keywords to cybersecurity topics
│   ├── MemoryService.cs            # Manages user memory and recall
│   ├── ResponseService.cs          # Generates random and follow-up responses
│   ├── SentimentService.cs         # Detects user sentiment and provides empathy
│   └── SpeechService.cs            # Manages Text-to-Speech functionality
├── Delegates/
│   └── ChatDelegates.cs            # Custom delegates for event handling
└── VoiceGreeting.wav               # Audio file for welcome message
🚀 Setup & Installation Instructions
Prerequisites
Visual Studio 2019 or 2022 (Community Edition is fine).
Ensure the ".NET desktop development" workload is installed in the Visual Studio Installer.
Steps to Run
Clone/Download the project repository.
Open POEProg6221.GUI.sln in Visual Studio.
Install NuGet Package:
Right-click the project in Solution Explorer -> Manage NuGet Packages.
Search for NAudio and install it.
Add System.Speech Reference:
Right-click References in Solution Explorer -> Add Reference.
Go to Assemblies -> Check System.Speech -> Click OK.
Configure Audio File:
Ensure VoiceGreeting.wav is present in the project folder.
Click on VoiceGreeting.wav in Solution Explorer.
In the Properties window, set:
Build Action: Content
Copy to Output Directory: Copy if newer
Build and Run: Press F5 or click the "Start" button.
🎮 How to Use the Chatbot
Start: Upon launching, the ASCII art header will load. Enter your name in the text box and click "Start Chat".
Interact: Type your questions in the input box (e.g., "I'm worried about phishing").
Quick Topics: Click the buttons at the top (e.g., 🔑 Passwords, 📧 Phishing) for instant tips.
Follow-ups: Ask the bot to "tell me more" or "give me another tip" to continue the current topic.
Memory: Ask "What do you remember about me?" or click the "🧠 What I Remember" button.
Voice: Toggle the voice checkbox in the top right to enable/disable Text-to-Speech.
Exit: Type quit or click the "🚪 Quit" button to end the session.
