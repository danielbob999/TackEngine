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
        private int m_itemsRenderedThisFrame = 0;
        private int m_jobsRendererThisFrame = 0;

        internal int ItemsRenderedLastFrame { get; private set; }
        internal int JobsRenderedLastFrame { get; private set; }

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
            
            if (m_lineJobQueue.Count == 0) {
                return;
            }

            m_renderingBehaviour.OnPreRender();

            for (int i = 0; i < m_lineJobQueue.Count; i++) {
                if (m_lineJobQueue[i].LineContext != context) { continue; }

                m_jobsRendererThisFrame++;

                for (int l = 0; l < m_lineJobQueue[i].Lines.Count; l++) {
                    m_renderingBehaviour.RenderLineToScreen(m_lineJobQueue[i].Lines[l], context);
                    m_itemsRenderedThisFrame++;
                }
            }

            m_renderingBehaviour.OnPostRender();
        }


        internal void Close() {
            if (m_renderingBehaviour == null) {
                return;
            }

            m_renderingBehaviour.Close();
        }

        internal void ClearLineJobQueue() {
            m_lineJobQueue.Clear();

            ItemsRenderedLastFrame = m_itemsRenderedThisFrame;
            JobsRenderedLastFrame = m_jobsRendererThisFrame;

            m_itemsRenderedThisFrame = 0;
            m_jobsRendererThisFrame = 0;
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
