/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TackEngineLib.Engine;
using TackEngineLib.GUI;
using TackEngineLib.Input;

namespace TackEngineLib.Main {
    /// <summary>
    /// 
    /// </summary>
    public class TackConsole {
        public enum LogType {
            Message = 1,
            Warning = 2,
            Error = 3,
            Debug = 4
        };

        public static TackConsole Instance { get; private set; } = null;

        private bool m_consoleGUIActive = false;
        private KeyboardKey m_activationKey;

        private List<string> m_messages = new List<string>();
        private List<TackCommand> m_validCommands = new List<TackCommand>();
        private List<string> m_commandHistory = new List<string>();
        private int m_previousCommandIndex = -1;
        private bool m_previousCommandInputLocker = false;
        private string m_logPath;
        private bool m_allowLoggingToFile = true;
        private string m_inputString;

        // Implement the new GUI system
        private GUITextArea m_consoleTextArea;
        private GUIInputField m_consoleInputField;

        /// <summary>
        /// Gets or Sets whether the Instance of TackConsole has logging to file enable. 
        /// If Get is called and Instance is null, false is returned
        /// </summary>
        public bool EnableLoggingToFile {
            get {
                if (Instance != null) {
                    return Instance.m_allowLoggingToFile;
                } else {
                    return false;
                }
            }

            set {
                if (Instance != null) {
                    Instance.m_allowLoggingToFile = value;
                }
            }
        }

        /// <summary>
        /// The current log level. Log types with a higher level than this value will be ignored
        /// </summary>
        public LogType LogLevel { get; set; }

        /// <summary>
        /// Whether or not to log messages to an external console (Windows cmd)
        /// </summary>
        public bool LogToExternalConsole { get; set; }

        internal TackConsole() {
            // Set the static instance
            Instance = this;

            LogLevel = LogType.Error;
            LogToExternalConsole = true;

            m_logPath = string.Format("logs/log_{0}_{1}_{2}.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "/logs")) {
                //Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/logs");
            }

            m_consoleInputField = null;
            m_consoleTextArea = null;

            m_consoleGUIActive = false;
        }

        internal void OnStart() {
            double startTime = EngineTimer.Instance.TotalRunTime;

            GetCommandsFromAssembly(typeof(TackEngine).Assembly.FullName);

            EngineLog(LogType.Message, "TackConsole started in " + (EngineTimer.Instance.TotalRunTime - startTime).ToString("0.000") + " seconds");
        }

        internal void OnUpdate() {
            if (m_consoleInputField == null && m_consoleTextArea == null) {
                m_consoleInputField = new GUIInputField();
                m_consoleInputField.Position = new Vector2f(0, 500);
                m_consoleInputField.Size = new Vector2f(TackEngine.Instance.Window.WindowSize.X, 30);
                m_consoleInputField.OnSubmittedEvent += ProcessCommand;
                m_consoleInputField.Active = m_consoleGUIActive;
                m_consoleInputField.Text = "";
                m_consoleInputField.NormalStyle.VerticalAlignment = VerticalAlignment.Middle;

                m_consoleTextArea = new GUITextArea();
                m_consoleTextArea.Position = new Vector2f(0, 0);
                m_consoleTextArea.Size = new Vector2f(TackEngine.Instance.Window.WindowSize.X, 500);
                m_consoleTextArea.NormalStyle = new GUITextArea.GUITextAreaStyle() {
                    Colour = new Colour4b(0, 0, 0, 220),
                    Border = null,
                    Font = BaseTackGUI.Instance.DefaultFont,
                    FontSize = 8f,
                    FontColour = Colour4b.Green,
                    Texture = Sprite.DefaultSprite
                };
                m_consoleTextArea.Active = m_consoleGUIActive;
                m_consoleTextArea.CanScroll = true;

                m_consoleGUIActive = false;
            }

            if (TackInput.KeyDown(KeyboardKey.GraveAccent)) {
                m_consoleGUIActive = !m_consoleGUIActive;

                m_consoleInputField.Active = m_consoleGUIActive;
                m_consoleTextArea.Active = m_consoleGUIActive;
            }

            if (m_consoleGUIActive) {
                string str = "";

                for (int i = 0; i < m_messages.Count; i++) {
                    str += m_messages[i] + "\n";
                }

                m_consoleTextArea.Text = str;
            }
        }

        internal void OnGUIRender() {

        }

        internal void OnClose() {
            if (m_allowLoggingToFile) {
                if (File.Exists(Directory.GetCurrentDirectory() + "/" + m_logPath)) {
                    File.AppendAllLines(Directory.GetCurrentDirectory() + "/" + m_logPath, m_messages);
                }
            }
        }

        public static void Log(string _msg) {
            if (string.IsNullOrEmpty(_msg)) {
                return;
            }

            Instance.m_messages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} {4}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _msg));
        }

        internal static void EngineLog(LogType _type, string _msg, params object[] _params) {
            if (_type > Instance.LogLevel) {
                return;
            }

            string formattedString = string.Format(_msg, _params);

            Instance.m_messages.Add(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                _type.ToString(),
                formattedString));

            if (Instance.LogToExternalConsole) {
                Console.WriteLine(string.Format("{0}:{1:00}:{2:00}.{3:000} [{4}] {5}",
                    DateTime.Now.Hour,
                    DateTime.Now.Minute,
                    DateTime.Now.Second,
                    DateTime.Now.Millisecond,
                    _type.ToString(),
                    formattedString));
            }
        }

        private void GetCommandsFromAssembly(string a_assemblyName) {
            Assembly assembly;

            try {
                assembly = Assembly.Load(a_assemblyName);
            } catch (Exception e) {
                EngineLog(LogType.Error, "Failed to load assembly with name: " + a_assemblyName);
                EngineLog(LogType.Error, e.Message);
                return;
            }

            int i = 0;

            EngineLog(LogType.Message, "Looking for ConsoleMethods in Assembly: " + assembly.FullName);
            foreach (Type classType in assembly.GetTypes()) {
                foreach (MethodInfo methodInfo in classType.GetMethods()) {
                    foreach (Attribute methodAttribute in methodInfo.GetCustomAttributes()) {
                        if (methodAttribute.GetType() == typeof(CommandMethod)) {
                            //Console.WriteLine("Class: {0}, Method: {1}, Attribute: {3}", classType.Name, methodInfo.Name, methodAttribute.GetType().Name);
                            m_validCommands.Add(new TackCommand(((CommandMethod)methodAttribute).GetCallString(), (EngineDelegates.CommandDelegate)methodInfo.CreateDelegate(typeof(EngineDelegates.CommandDelegate)), ((CommandMethod)methodAttribute).GetArgList().ToList()));
                            i++;
                        }
                    }
                }
            }

            EngineLog(LogType.Message, "Found " + i + " valid CommandMethods in Assembly: " + assembly.FullName);
        }

        private void ProcessCommand(object sender, EventArgs e) {
            string input = ((GUIInputField)sender).Text;

            ((GUIInputField)sender).Text = "";
            ((GUIInputField)sender).SelectionStart = 0;

            if (string.IsNullOrEmpty(input)) {
                EngineLog(LogType.Message, "Command input string is null or empty");
                return;
            }

            string commandInput = input;
            EngineLog(LogType.Message, "> " + commandInput);
            m_commandHistory.Add(commandInput);
            m_previousCommandIndex = -1;

            string[] splitCommandBySpaces = commandInput.Split(' ');

            foreach (TackCommand command in m_validCommands) {
                if (splitCommandBySpaces[0] == command.CommandCallString) {
                    command.CommandDelegate.Invoke(splitCommandBySpaces);
                    return;
                }
            }

            EngineLog(LogType.Message, "No valid TackCommand with call string '" + splitCommandBySpaces[0] + "'");
            EngineLog(LogType.Message, "Use 'help' to get a list of valid commands");
        }

        internal static List<TackCommand> GetLoadedTackCommands() {
            return Instance.m_validCommands;
        }
    }
}
