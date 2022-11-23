using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Engine;

namespace TackEngineLib.Main {
    public class EngineTimer {
        public static EngineTimer Instance = null;

        private System.Diagnostics.Stopwatch m_watch;
        private double m_timeAtLastUpdate;
        private double m_timeAtLastRender;
        private List<double> m_lastSecondUpdateTimes;
        private List<double> m_lastSecondRenderTimes;

        /// <summary>
        /// Gets the total time that this instance of TackEngine has been running, in seconds 
        /// </summary>
        public double TotalRunTime {
            get { return m_watch.Elapsed.TotalSeconds; }
        }

        /// <summary>
        /// Gets the time it took to complete the last update loop, in seconds
        /// </summary>
        public double LastUpdateTime { get; private set; }

        /// <summary>
        /// Gets the average update time in the last second
        /// </summary>
        public double UpdateTimeAverageLastSecond { 
            get { 
                return (m_lastSecondUpdateTimes.Count > 0 ? m_lastSecondUpdateTimes.Average() : 0.0d); 
            } 
        }

        /// <summary>
        /// Gets the time it took to complete the last render loop, in seconds
        /// </summary>
        public double LastRenderTime { get; private set; }

        /// <summary>
        /// Gets the average render time in the last second
        /// </summary>
        public double RenderTimeAverageLastSecond { 
            get {
                return (m_lastSecondRenderTimes.Count > 0 ? m_lastSecondRenderTimes.Average() : 0.0d); 
            } 
        }

        internal EngineTimer() {
            m_watch = new System.Diagnostics.Stopwatch();
        }

        internal void OnStart() {
            LastUpdateTime = 0;
            LastRenderTime = 0;

            m_lastSecondUpdateTimes = new List<double>();
            m_lastSecondRenderTimes = new List<double>();

            Instance = this;

            m_watch.Start();
        }

        internal void OnUpdate() {
            double lastUT = m_watch.Elapsed.TotalSeconds;

            LastUpdateTime = lastUT - m_timeAtLastUpdate;
            m_timeAtLastUpdate = lastUT;

            while (m_lastSecondUpdateTimes.Sum() > 1.0f) {
                m_lastSecondUpdateTimes.RemoveAt(0);
            }

            m_lastSecondUpdateTimes.Add(LastUpdateTime);
        }

        internal void OnRender() {
            double lastRT = m_watch.Elapsed.TotalSeconds;

            LastRenderTime = lastRT - m_timeAtLastRender;
            m_timeAtLastRender = lastRT;

            while (m_lastSecondRenderTimes.Sum() > 1.0f) {
                m_lastSecondRenderTimes.RemoveAt(0);
            }

            m_lastSecondRenderTimes.Add(LastRenderTime);
        }

        internal void OnClose() {
            m_watch.Stop();
        }
    }
}
