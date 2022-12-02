/* Copyright (c) 2019 Daniel Phillip Robinson */

namespace TackEngine.Core.Main {
    public class RectangleShape {

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2f Position { get { return new Vector2f(X, Y); } }
        public Vector2f Size { get { return new Vector2f(Width, Height); } }

        public RectangleShape() {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public RectangleShape(float x, float y, float w, float h) {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public RectangleShape(Vector2f position, Vector2f size) {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public RectangleShape(Vector2i position, Vector2i size) {
            X = position.X;
            Y = position.Y;
            Width = size.X;
            Height = size.Y;
        }

        public override string ToString() {
            return (string.Format("({0}, {1}, {2}, {3})", X, Y, Width, Height));
        }
    }
}
