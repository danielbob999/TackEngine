using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TackEngine.Desktop {
    public static class ExtensionMethods {
        // from TE Vector2f to OpenTK Vector2
        public static OpenTK.Mathematics.Vector2 ToOpenTKVec2(this TackEngine.Core.Main.Vector2f vec) {
            return new OpenTK.Mathematics.Vector2(vec.X, vec.Y);
        }

        // from OpenTK Vector2 to TE Vector2f
        public static TackEngine.Core.Main.Vector2f ToTEVec2(this OpenTK.Mathematics.Vector2 vec) {
            return new TackEngine.Core.Main.Vector2f(vec.X, vec.Y);
        }

        public static OpenTK.Mathematics.Vector3 ToOpenTKVec3(this TackEngine.Core.Main.Vector3 vec) {
            return new OpenTK.Mathematics.Vector3(vec.X, vec.Y, vec.Z);
        }

        // from TE Vector4 to OpenTK Vector4
        public static OpenTK.Mathematics.Vector4 ToOpenTKVec4(this TackEngine.Core.Main.Vector4 vec) {
            return new OpenTK.Mathematics.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        // from OpenTK Vector4 to TE Vector4
        public static TackEngine.Core.Main.Vector4 ToTEVec4(this OpenTK.Mathematics.Vector4 vec) {
            return new TackEngine.Core.Main.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static OpenTK.Mathematics.Matrix2 ToOpenTKMat2(this TackEngine.Core.Main.Matrix2 mat) {
            return new OpenTK.Mathematics.Matrix2(mat.Row0.ToOpenTKVec2(), mat.Row1.ToOpenTKVec2());
        }

        public static OpenTK.Mathematics.Matrix3 ToOpenTKMat3(this TackEngine.Core.Main.Matrix3 mat) {
            return new OpenTK.Mathematics.Matrix3(mat.Row0.ToOpenTKVec3(), mat.Row1.ToOpenTKVec3(), mat.Row2.ToOpenTKVec3());
        }

        public static OpenTK.Mathematics.Matrix4 ToOpenTKMat4(this TackEngine.Core.Main.Matrix4 mat) {
            return new OpenTK.Mathematics.Matrix4(mat.Row0.ToOpenTKVec4(), mat.Row1.ToOpenTKVec4(), mat.Row2.ToOpenTKVec4(), mat.Row3.ToOpenTKVec4());
        }

        public static TackEngine.Core.Main.Matrix4 ToTEMat4(this OpenTK.Mathematics.Matrix4 mat) {
            return new TackEngine.Core.Main.Matrix4(mat.Row0.ToTEVec4(), mat.Row1.ToTEVec4(), mat.Row2.ToTEVec4(), mat.Row3.ToTEVec4());
        }

        public static OpenTK.Mathematics.Vector4 ToOpenTKVec4(this TackEngine.Core.Main.Colour4b col) {
            return new OpenTK.Mathematics.Vector4(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);
        }
    }
}
