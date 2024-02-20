/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Physics;
using TackEngine.Core.Math;

namespace TackEngine.Core.Physics
{
    static class PhysicsMovement
    {
        public static Vector2f MovementLeft(Vector2f _currentMovementVec, Dictionary<int, Vector2f> _movingObjVerts, Dictionary<int, Vector2f> _stationaryObjVerts)
        {
            Vector2f returnVec = _currentMovementVec;

            var movingObjVertsSorted = from entry in _movingObjVerts orderby entry.Value.X ascending select entry;
            var stationaryObjVertsSorted = from entry in _stationaryObjVerts orderby entry.Value.X descending select entry;

            //Console.WriteLine(string.Format("{0} {1} {2} {3}", movingObjVertsSorted.ToList()[0], movingObjVertsSorted.ToList()[1], movingObjVertsSorted.ToList()[2], movingObjVertsSorted.ToList()[3]));

            LinearEquation movingObjEqu = TackMath.GetLinearEquationFromPoints(movingObjVertsSorted.ToList()[0].Value, movingObjVertsSorted.ToList()[1].Value);
            LinearEquation stationaryObjEqu = TackMath.GetLinearEquationFromPoints(stationaryObjVertsSorted.ToList()[0].Value, stationaryObjVertsSorted.ToList()[1].Value);

            Console.WriteLine("vert: " + movingObjVertsSorted.ToList()[0].Value);
            Console.WriteLine(string.Format("{0} | {1}", movingObjEqu.ToString(), stationaryObjEqu.ToString()));

            if (TackMath.GetLinearIntersectionPoint(movingObjEqu, stationaryObjEqu, out Vector2f interestionPoint))
            {
                Console.WriteLine("1 yes");
                if (interestionPoint.X <= stationaryObjVertsSorted.ToList()[0].Value.X && interestionPoint.X >= stationaryObjVertsSorted.ToList()[1].Value.X)
                {
                    Console.WriteLine("2 yes");
                    if (interestionPoint.Y <= stationaryObjVertsSorted.ToList()[0].Value.Y && interestionPoint.Y >= stationaryObjVertsSorted.ToList()[1].Value.Y)
                    {
                        Console.WriteLine("Intersection has been achieved: " + interestionPoint.ToString());
                    }
                }
            }

            Console.WriteLine("Point: " + interestionPoint.ToString());

            return returnVec;
        }
    }
}
