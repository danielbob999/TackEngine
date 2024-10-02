using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;

namespace TackEngine.Core.Main {
    public class Line {
        public Vector2f PointA { get; set; }
        public Vector2f PointB { get; set; }
        public float Width { get; set; }
        public Colour4b Colour { get; set; }

        public Line(Vector2f pointA, Vector2f pointB) {
            PointA = pointA;
            PointB = pointB;
            Width = 2f;
            Colour = Colour4b.Black;
        }

        public Line(Vector2f pointA, Vector2f pointB, float width) {
            PointA = pointA;
            PointB = pointB;
            Width = width;
            Colour = Colour4b.Black;
        }

        public Line(Vector2f pointA, Vector2f pointB, float width, Colour4b colour) {
            PointA = pointA;
            PointB = pointB;
            Width = width;
            Colour = colour;
        }
    }
}
