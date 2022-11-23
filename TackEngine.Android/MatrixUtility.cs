using Android.App;
using Android.Content;
using Android.Icu.Number;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TackEngine.Android {
    public static class MatrixUtility {
        public static OpenTK.Matrix4 CreateScaleMatrix(float scaleX, float scaleY, float scaleZ) {
            OpenTK.Matrix4 newMat = new OpenTK.Matrix4();

            newMat = OpenTK.Matrix4.Identity;
            newMat.Row0.X = scaleX;
            newMat.Row1.Y = scaleY;
            newMat.Row2.Z = scaleZ;

            return newMat;
        } 
    }
}