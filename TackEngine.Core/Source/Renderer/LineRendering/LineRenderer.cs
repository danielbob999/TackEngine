using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Renderer.LineRendering;
using TackEngine.Core.Main;

namespace TackEngine.Core.Source.Renderer.LineRendering {
    public class LineRenderer {
        public enum LineContext {
            World,
            GUI
        }

        public static LineRenderer Instance { get; private set; }

        private LineRenderingBehaviour m_renderingBehaviour;
        private List<LineRenderingJob> m_lineJobQueue;

        internal LineRenderer(LineRenderingBehaviour behaviour) {
            m_renderingBehaviour = behaviour;
            m_lineJobQueue = new List<LineRenderingJob>();

            Instance = this;
        }

        internal void Initialise() {
            if (m_renderingBehaviour == null) {
                return;
            }

            m_renderingBehaviour.Initialise();
        }

        internal void DrawLinesToScreen(LineContext context) {
            if (m_renderingBehaviour == null) {
                return;
            }

            List<LineRenderingJob> jobs = m_lineJobQueue.FindAll(j => j.LineContext == context);

            for (int i = 0; i < jobs.Count; i++) {
                for (int l = 0; l < jobs[i].Lines.Count; l++) {
                    m_renderingBehaviour.RenderLineToScreen(jobs[i].Lines[l], context);
                }
            }
        }


        internal void Close() {
            if (m_renderingBehaviour == null) {
                return;
            }

            m_renderingBehaviour.Close();
        }

        internal void ClearLineJobQueue() {
            m_lineJobQueue.Clear();
        }

        public void DrawLine(Line line, LineContext context) {
            if (line == null) {
                return;
            }

            m_lineJobQueue.Add(new LineRenderingJob(new List<Line>() { line }, context));
        }

        public void DrawLines(List<Line> lines, LineContext context) {
            if (lines == null) {
                return;
            }

            m_lineJobQueue.Add(new LineRenderingJob(lines, context));
        }
    }
}
