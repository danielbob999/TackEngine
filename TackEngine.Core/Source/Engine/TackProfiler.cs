using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngine.Core.Main;
using TackEngine.Core.GUI;

namespace TackEngine.Core.Engine {
    public class TackProfiler {
        public static TackProfiler Instance { get; private set; } = null;

        private struct ProfilerEntry {
            public string name;
            public Dictionary<ulong, double> entries;
        }

        private Dictionary<string, double> m_timerStartTimes;
        private Dictionary<string, ProfilerEntry> m_timerResults;
        private Stopwatch m_stopwatch;
        private GUITextArea m_textArea = null;
        private readonly string m_profilerFilePath = "profiler/profiler_run_{0}.csv";

        public bool ShowUI { get; set; }
        public bool Enabled { get; set; }
        public bool SaveToFile { get; set; }

        public TackProfiler() {
            Instance = this;

            m_timerStartTimes = new Dictionary<string, double>();
            m_timerResults = new Dictionary<string, ProfilerEntry>();
            m_stopwatch = new Stopwatch();
            ShowUI = false;
            Enabled = false;
            SaveToFile = false;
        }

        public void OnStart() {
            m_stopwatch.Start();

            if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Windows) {
                // Check for profile directory, if not there, create one
                if (!System.IO.Directory.Exists("profiler")) {
                    System.IO.Directory.CreateDirectory("profiler");
                }
            }
        }

        public void OnUpdate() {
            if (m_textArea == null) {
                m_textArea = new GUITextArea();
                m_textArea.Position = new Vector2f(5, 5);
                m_textArea.Size = new Vector2f(350, 300);
            }

            if (!Enabled) {
                m_textArea.Active = false;
                return;
            }

            m_textArea.Active = ShowUI;

            if (ShowUI) {
                string str = "";

                List<string> times = m_timerResults.Keys.OrderBy(x => x).ToList();

                for (int i = 0; i < times.Count; i++) { 
                    str += times[i] + "   " + (m_timerResults[times[i]].entries.Last().Value * 1000d).ToString("0.00000") + "\n";
                }

                m_textArea.Text = str;
            }
        }

        public void OnClose() {
            if (!Enabled) {
                return;
            }

            int rerun = 0;

            while (System.IO.File.Exists(string.Format(m_profilerFilePath, rerun))) {
                rerun++;
            }

            string finalPath = string.Format(m_profilerFilePath, rerun);

            string str = "";

            List<string> keys = new List<string>();

            foreach (KeyValuePair<string, ProfilerEntry> pair in m_timerResults) {
                keys.Add(pair.Key);

                str += pair.Key + ",";
            }

            str += "\n";

            for (ulong i = 0; i < m_timerResults["UserUpdate"].entries.Keys.Max(); i++) {
                str += GenerateSaveFileLine(i, keys);
            }

            System.IO.File.WriteAllText(finalPath, str);

            TackConsole.EngineLog(TackConsole.LogType.Message, "Logged TackProfiler file to {0}", finalPath);
        }

        public void StartTimer(string name) {
            if (!Enabled) {
                return;
            }

            if (m_timerStartTimes == null) {
                return;
            }

            if (!m_timerStartTimes.ContainsKey(name)) {
                m_timerStartTimes.Add(name, m_stopwatch.Elapsed.TotalSeconds);
            }
        }

        public void StopTimer(string name) {
            if (!Enabled) {
                return;
            }

            if (m_timerStartTimes == null) {
                return;
            }

            if (m_timerStartTimes.ContainsKey(name)) {
                double elapsedTime = m_stopwatch.Elapsed.TotalSeconds - m_timerStartTimes[name];

                if (!m_timerResults.ContainsKey(name)) {
                    ProfilerEntry entry = new ProfilerEntry() {
                        name = name,
                        entries = null
                    };

                    m_timerResults.Add(name, entry);
                }

                if (m_timerResults[name].entries == null) {
                    ProfilerEntry entry = m_timerResults[name];
                    entry.entries = new Dictionary<ulong, double>();

                    m_timerResults[name] = entry;
                }

                if (m_timerResults[name].entries.Count > 2000) {
                    //m_timerResults[name].entries.Remove(m_timerResults[name].entries.First().Key); // remove the "first" timing entry
                }

                // if there is already an finished timer entry for this frame index, sum it


                if (m_timerResults[name].entries.ContainsKey(TackEngineInstance.Instance.Window.CurrentUpdateLoopIndex)) {
                    ProfilerEntry entry = m_timerResults[name];
                    entry.entries[TackEngineInstance.Instance.Window.CurrentUpdateLoopIndex] += elapsedTime;

                    m_timerResults[name] = entry;
                } else {
                    ProfilerEntry entry = m_timerResults[name];
                    entry.entries.Add(TackEngineInstance.Instance.Window.CurrentUpdateLoopIndex, elapsedTime);

                    m_timerResults[name] = entry;
                }

                m_timerStartTimes.Remove(name);
            }
        }

        private string GenerateSaveFileLine(ulong lineNumber, List<string> columns) {
            string generatedString = "";
            List<object> values = new List<object>();

            for (int i = 0; i < columns.Count; i++) {
                generatedString += "{" + i + "},";

                if (m_timerResults[columns[i]].entries.ContainsKey(lineNumber)) {
                    values.Add(m_timerResults[columns[i]].entries[lineNumber]);
                }
            }

            generatedString += "\n";

            return string.Format(generatedString, values.ToArray());
        }
    }
}
