using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Physics {
    public class AABB {
        private Vector2f m_bottomLeft;
        private Vector2f m_topRight;

        /// <summary>
        /// The bottom left point of the AABB
        /// </summary>
        public Vector2f BottomLeft {
            get { return m_bottomLeft; }
            set { m_bottomLeft = value; }
        }

        /// <summary>
        /// The top right point of the AABB
        /// </summary>
        public Vector2f TopRight {
            get { return m_topRight; }
            set { m_topRight = value; }
        }

        // Shortcuts to get top/bottom/left/right float values

        public float Top {
            get { return m_topRight.Y; }
        }

        public float Bottom {
            get { return m_bottomLeft.Y; }
        }

        public float Left {
            get { return m_bottomLeft.X; }
        }

        public float Right {
            get { return m_topRight.X; }
        }

        public Vector2f Origin {
            get {
                Vector2f origin = new Vector2f() {
                    X = m_bottomLeft.X + ((m_topRight.X - m_bottomLeft.X) / 2.0f),
                    Y = m_topRight.Y - ((m_topRight.Y - m_bottomLeft.Y) / 2.0f)
                };

                return origin;
            }
        }

        public float Width {
            get {
                return m_topRight.X - m_bottomLeft.X;
            }
        }

        public float Height {
            get {
                return m_topRight.Y - m_bottomLeft.Y;
            }
        }

        /// <summary>
        /// Returns the 4 vertex points of the AABB
        /// 
        /// Vertex Layout: v1, v2, v3, v4
        /// 
        ///  v4 --- v3
        ///  |      |
        ///  |      |
        ///  v1 --- v2
        /// 
        /// </summary>
        public Vector2f[] VertexPoints {
            get {
                Vector2f[] points = new Vector2f[4];
                points[0] = m_bottomLeft;
                points[1] = new Vector2f(Right, Bottom);
                points[2] = m_topRight;
                points[3] = new Vector2f(Left, Top);

                return points;
            }
        }

        internal AABB() {
            m_bottomLeft = new Vector2f(-1, -1);
            m_topRight = new Vector2f(1, 1);
        }

        public AABB(Vector2f bottomLeft, Vector2f topRight) {
            m_bottomLeft = bottomLeft;
            m_topRight = topRight;
        }

        public AABB(RectangleShape shape) {
            m_bottomLeft = new Vector2f(shape.X, shape.Y + shape.Height);
            m_topRight = new Vector2f(shape.X + shape.Width, shape.Y);
        }

        /// <summary>
        /// Returns true if the specified point is anywhere inside the AABB
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPointInAABB(AABB aabb, Vector2f point) {
            if (point.X >= aabb.Left && point.X <= aabb.Right) {
                if (point.Y >= aabb.Top && point.Y <= aabb.Bottom) {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPointInAABBWorld(AABB aabb, Vector2f point) {
            if (point.X >= aabb.Left && point.X <= aabb.Right) {
                if (point.Y <= aabb.Top && point.Y >= aabb.Bottom) {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckForAABBCollision(AABB aabb1, AABB aabb2) {
            if (System.Math.Abs(aabb1.Origin.X - aabb2.Origin.X) > ((aabb1.Width / 2f) + (aabb2.Width / 2f))) {
                return false;
            }

            if (System.Math.Abs(aabb1.Origin.Y - aabb2.Origin.Y) > ((aabb1.Height / 2f) + (aabb2.Height / 2f))) {
                return false;
            }

            return true;
        }
    }
}
