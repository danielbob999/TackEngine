/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.Math
{
    public static class TackMath {

        public static readonly float PiOver2 = (float)System.Math.PI / 2f;

        public static int Abs(int value) {
            if (value > 0) {
                return value;
            }
            
            return value * -1;
        }

        public static float Abs(float value) {
            if (value > 0) {
                return value;
            }

            return value * -1f;
        }

        public static double Abs(double value) {
            if (value > 0) {
                return value;
            }

            return value * -1d;
        }

        public static float Clamp(float val, float min, float max) {
            if (val < min) {
                return min;
            }

            if (val > max) {
                return max;
            }

            return val;
        }

        /// <summary>
        /// Converts and angle in degrees to an angle in radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegToRad(double angleInDegrees) {
            return angleInDegrees * (System.Math.PI / 180f);
        }

        /// <summary>
        /// Converts and angle in degrees to an angle in radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float DegToRad(float angleInDegrees) {
            return angleInDegrees * ((float)System.Math.PI / 180f);
        }

        /// <summary>
        /// Converts an angle in radians to an angle in degrees
        /// </summary>
        /// <param name="angleInRadians"></param>
        /// <returns></returns>
        public static double RadToDeg(double angleInRadians) {
            return angleInRadians * (180f / System.Math.PI);
        }

        public static float RadToDeg(float angleInRadians) {
            return (angleInRadians * (180f / (float)System.Math.PI));
        }

        /// <summary>
        /// Finds the intersection point of two lines
        /// </summary>
        /// <param name="_lineA">The linear equation of the first line</param>
        /// <param name="_lineB">The linear equation of the second line</param>
        /// <param name="_intersectPoint">The point at which these two lines intersect. (0, 0) if lines do not intersect</param>
        /// <returns>True if the lines intersect at a point, false if lines do not intersect</returns>
        /// <returntype>bool</returntype>
        public static bool GetLinearIntersectionPoint(LinearEquation _lineA, LinearEquation _lineB, out Vector2f _intersectPoint)  {
            // Get X value from equations.
            float xValue = (_lineB.YIntercept - _lineA.YIntercept) / (_lineA.Gradient - _lineB.Gradient);

            // Calculate intersection point on the Y axis, using both equations
            float yPointLineA = _lineA.Gradient * xValue + _lineA.YIntercept;
            float yPointLineB = _lineB.Gradient * xValue + _lineB.YIntercept;

            // See if both Y points are equal, if so, _lineA and _lineB intersect. If not equal, lines do not intersect
            if (yPointLineA == yPointLineB)
            {
                _intersectPoint = new Vector2f(xValue, yPointLineA);
                return true;
            }
            else
            {
                _intersectPoint = new Vector2f(-1, -1);
                return false;
            }
        }

        /// <summary>
        /// Calculates the LinearEquation of a line given two points
        /// </summary>
        /// <param name="_pointA">The first point</param>
        /// <param name="_pointB">The second point</param>
        /// <returns>The LinearEquation of the line</returns>
        /// <returntype>LinearEquation</returntype>
        public static LinearEquation GetLinearEquationFromPoints(Vector2f _pointA, Vector2f _pointB) {
            LinearEquation equation = new LinearEquation();

            // Calculate the gradient
            equation.Gradient = (_pointB.Y - _pointA.Y) / (_pointB.X - _pointA.X);

            // Calculate the y-intercept using the values if _pointA. The values of _pointB can also be used, the answer will be the same
            equation.YIntercept = _pointA.Y - (equation.Gradient * _pointA.X);

            return equation;
        }

        public static float Lerp(float a, float b, float f) {
            return (a * (1.0f - f)) + (b * f);
        }

        public static double Lerp(double a, double b, double f) {
            return (a * (1.0f - f)) + (b * f);
        }

        public static float InverseSqrtFast(float x) {
            unsafe {
                var xhalf = 0.5f * x;
                var i = *(int*)&x; // Read bits as integer.
                i = 0x5f375a86 - (i >> 1); // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i; // Convert bits back to float
                x = x * (1.5f - (xhalf * x * x)); // Perform left single Newton-Raphson step.
                return x;
            }
        }

        public static double InverseSqrtFast(double x) {
            unsafe {
                double xhalf = 0.5 * x;
                long i = *(long*)&x; // Read bits as long.
                i = 0x5fe6eb50c7b537a9 - (i >> 1); // Make an initial guess for Newton-Raphson approximation
                x = *(double*)&i; // Convert bits back to double
                x = x * (1.5 - (xhalf * x * x)); // Perform left single Newton-Raphson step.
                return x;
            }
        }
    }
}
