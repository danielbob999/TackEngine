/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Main {
    /// <summary>
    /// 
    /// </summary>
    public struct Vector2i {
        public static readonly Vector2i Zero = new Vector2i(0, 0);

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Get this vector with a magnitude of 1
        /// </summary>
        public Vector2i Normalised {
            get {
                int distance = (int)System.Math.Sqrt(X * X + Y * Y);
                return new Vector2i(X / distance, Y / distance);
            }
        }

        /// <summary>
        /// Gets the magnitude of this vector
        /// </summary>
        public int Magnitude {
            get {
                return (int)System.Math.Sqrt(X * X + Y * Y);
            }
        }

        public Vector2i(Vector2i vec) {
            X = vec.X;
            Y = vec.Y;
        }

        public Vector2i(int _x, int _y) {
            X = _x;
            Y = _y;
        }

        public void Normalise() {
            int distance = (int)System.Math.Sqrt(X * X + Y * Y);
            X = (X / distance);
            Y = (Y / distance);
        }

        public static Vector2i operator +(Vector2i _a, Vector2i _b) {
            return new Vector2i(_a.X + _b.X, _a.Y + _b.Y);
        }

        public static Vector2i operator -(Vector2i _a, Vector2i _b) {
            return new Vector2i(_b.X - _a.X, _b.Y - _a.Y);
        }

        public static Vector2i operator -(Vector2i _a, int _b) {
            return new Vector2i(_a.X - _b, _a.Y - _b);
        }

        public static Vector2i operator *(Vector2i _a, Vector2i _b) {
            return new Vector2i(_a.X * _b.X, _a.Y * _b.Y);
        }

        public static Vector2i operator *(Vector2i _a, int _b) {
            return new Vector2i(_a.X * _b, _a.Y * _b);
        }

        public static Vector2i operator /(Vector2i _a, int _b) {
            return new Vector2i(_a.X / _b, _a.Y / _b);
        }

        public static bool operator ==(Vector2i _a, Vector2i _b) {
            if (_a.X == _b.X) {
                if (_a.Y == _b.Y) {
                    return true;
                }
            }

            return false;
        }

        public static bool operator !=(Vector2i _a, Vector2i _b) {
            if (_a.X == _b.X) {
                if (_a.Y == _b.Y) {
                    return false;
                }
            }

            return true;
        }

        public static Vector2i operator -(Vector2i _a) {
            return new Vector2i(_a.X * -1, _a.Y * -1);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return string.Format("Vector2i({0}, {1})", X, Y);
        }

        public Vector2f ToVector2f() {
            return new Vector2f(X, Y);
        }

        public static int Distance(Vector2i a, Vector2i b) {
            int xDiff = (int)System.Math.Pow((b.X - a.X), 2);
            int yDiff = (int)System.Math.Pow((b.Y - a.Y), 2);

            return (int)System.Math.Sqrt(xDiff + yDiff);
        }

        public static Vector2i Lerp(Vector2i from, Vector2i to, float time) {
            return new Vector2i((int)(from.X + (to.X - from.X) * time), (int)(from.Y + (to.Y - from.Y) * time));
        }
    }
}
