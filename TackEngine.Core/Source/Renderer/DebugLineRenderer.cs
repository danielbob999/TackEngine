using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Renderer {
    public class DebugLineRenderer  {
        public class Line {
            public Vector2f Position { get; set; }
            public float Rotation { get; set; }
            public Colour4b Colour { get; set; }
            public float Width { get; set; }
            public float Length { get; set; }

            public Line() { }
        }

        internal static DebugLineRenderer Instance { get; set; }

        protected Shader m_shader;
        protected List<Line> m_lines;
        protected float[] m_vertexData;
        protected int[] m_indiceData;

        public DebugLineRenderer() {
            m_lines = new List<Line>();
        }

        public virtual void OnStart() {
            m_vertexData = new float[12] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,
                    /* v3 */ -1f,  1f, 1.0f,
                    /* v4 */ -1f, -1f, 1.0f
            };

            m_indiceData = new int[] {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
            };
        }

        public virtual void OnRender() {
            m_lines.Clear();
        }

        public virtual void OnClose() {

        }

        public void AddLine(Line line) {
            m_lines.Add(line);
        }

        public static void DrawLine(Vector2f pos, float rotation, Colour4b colour, float length, float width = 3) {
            Instance.AddLine(new Line() { Position = pos, Colour = colour, Length = length, Width = width, Rotation = rotation });
        }

        public static void DrawLine(Vector2f start, Vector2f finish, Colour4b colour, float width = 3) {
            float length = Vector2f.Distance(start, finish);
            Vector2f dir = (finish - start).Normalized();
            Vector2f middlePos = start + (dir * (length / 2f));

            float rotation = (float)TackMath.RadToDeg(System.Math.Atan2(finish.Y - start.Y, finish.X - start.X));

            Instance.AddLine(new Line() { Position = middlePos, Colour = colour, Length = length, Width = width, Rotation = rotation });
        }
    }
}
