🤖 Wooppaa Cybersecurity Awareness Bot - Part 2
📖 Overview
This project is Part 2 of the Cybersecurity Awareness Chatbot. It expands on the initial console application by introducing a fully functional Graphical User Interface (GUI) built with WPF (Windows Presentation Foundation). The chatbot is designed to be engaging, user-friendly, and educational, helping users learn about cybersecurity best practices through dynamic conversations, sentiment detection, and memory recall.

✨ Key Features
This application meets all the requirements of Part 2:

GUI Design & Implementation: Modern, dark interface (WPF), ASCII, Dynamic chat-bubbles, fast access topic buttons.
Voice Implementation: Custom .wav file for welcome greeting and integrated Text-to-Speech (TTS) for bot responses.
Keyword Recognition: Recognizes Cybersecurity topics (e.g., passwords, phishing, scams, privacy) and offers appropriate information.
Random Responses: Permits the use of generic responses (Lists) to provide a variety of responses to keep interactions varied and random.
Handling follow-up queries like "tell me more", "what else", etc., with transitioning the conversation smoothly.
Memory and Recall: Recalls the user's name, favorite topics and subjects discussed to make the experience personalised.
Sentiment Detection: Identifies user emotions (worried, curious, frustrated, happy, overwhelmed) and responds sympathetically to them.
Error Handling: Graceful default response for unrecognized inputs to avoid crashes.
Code Optimisation: Works with Object Oriented Programming (OOP), Delegates and efficient data structures (Dictionaries, Lists)..
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
Visual Studio 2019 or 2022 (Community Edition is okay).
Make sure to install the workload, "Visual Studio desktop development", in the Visual Studio Installer.
Steps to Run
Clone/Download project repository.
Open POEProg6221.The Visual Studio version of GUI.sln.
Install NuGet Package:
Right-click the project in Solution Explorer -> Manage NuGet Packages.
Install NAudio.
Add System.Speech Reference:
Right-click References in Solution Explorer -> Add Reference.
Select Assemblies -> Check System.Speech -> Click OK.
Configure Audio File:
Make sure that VoiceGreeting.wav is in the project folder.
Click on VoiceGreeting.wav in Solution Explorer.
On the Properties window set:
Build Action: Content
Copy to Output Directory: Copy if newer
Build and Run: Press F5 or click Start button.
🎮 Chatbot Use Case
Start: The ASCII art header will load when the program is started. Enter your name in the text box and click "Start Chat".
Interact: Put your questions in the entry box (e.g., "I'm concerned about phishing").
Quick Topics: For instant tips, click the buttons at the top, such as 🔑 Passwords, 📧 Phishing.
Follow-ups: For ongoing conversations, suggest to the bot that he "tell me more" or "give me another tip".
Memory: Give or hold the button "🧠 What I Remember?" and ask "What do you remember about me?".
Voice: Check/Uncheck the voice checkbox in the top-right corner to turn Text-to-Speech on/off.
Exit: Click the "🚪 Quit" button to quit the session or type quit.
