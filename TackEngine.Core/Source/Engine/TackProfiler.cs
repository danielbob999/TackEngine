using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngineLib.Main;
using TackEngineLib.GUI;

namespace TackEngineLib.Engine {
    public class TackProfiler {
        public static TackProfiler Instance { get; private set; } = null;

        private Dictionary<string, double> m_timerStartTimes;
        private Dictionary<string, List<double>> m_timerResults;
        private Stopwatch m_stopwatch;
        private GUITextArea m_textArea = null;

        public bool ShowUI { get; set; }
        public bool Enabled { get; set; }

        public TackProfiler() {
            Instance = this;

            m_timerStartTimes = new Dictionary<string, double>();
            m_timerResults = new Dictionary<string, List<double>>();
            m_stopwatch = new Stopwatch();
            ShowUI = false;
            Enabled = false;
        }

        public void OnStart() {
            m_stopwatch.Start();
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
                    str += times[i] + "   " + (m_timerResults[times[i]].Last() * 1000d).ToString("0.00000") + "\n";
                }

                m_textArea.Text = str;
            }
        }

        public void OnClose() {

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
                    m_timerResults.Add(name, null);
                }

                if (m_timerResults[name] == null) {
                    m_timerResults[name] = new List<double>();
                }

                if (m_timerResults[name].Count > 2000) {
                    m_timerResults[name].RemoveAt(0);
                }

                m_timerResults[name].Add(elapsedTime);

                m_timerStartTimes.Remove(name);
            }
        }
    }
}
