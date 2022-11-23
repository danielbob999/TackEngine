using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TackEngine.Android {
    public static class ExtensionMethods {
        // from TE Vector2f to OpenTK Vector2
        public static OpenTK.Vector2 ToOpenTKVec2(this TackEngineLib.Main.Vector2f vec) {
            return new OpenTK.Vector2(vec.X, vec.Y);
        }

        // from OpenTK Vector2 to TE Vector2f
        public static TackEngineLib.Main.Vector2f ToTEVec2(this OpenTK.Vector2 vec) {
            return new TackEngineLib.Main.Vector2f(vec.X, vec.Y);
        }

        public static OpenTK.Vector3 ToOpenTKVec3(this TackEngineLib.Main.Vector3 vec) {
            return new OpenTK.Vector3(vec.X, vec.Y, vec.Z);
        }

        // from TE Vector4 to OpenTK Vector4
        public static OpenTK.Vector4 ToOpenTKVec4(this TackEngineLib.Main.Vector4 vec) {
            return new OpenTK.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        // from OpenTK Vector4 to TE Vector4
        public static TackEngineLib.Main.Vector4 ToTEVec4(this OpenTK.Vector4 vec) {
            return new TackEngineLib.Main.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static OpenTK.Matrix2 ToOpenTKMat2(this TackEngineLib.Main.Matrix2 mat) {
            return new OpenTK.Matrix2(mat.Row0.X, mat.Row0.Y, mat.Row1.X, mat.Row1.Y);
        }

        public static OpenTK.Matrix3 ToOpenTKMat3(this TackEngineLib.Main.Matrix3 mat) {
            return new OpenTK.Matrix3(
                mat.Row0.X, mat.Row0.Y, mat.Row1.Z, 
                mat.Row1.X, mat.Row1.Y, mat.Row1.Z,
                mat.Row2.X, mat.Row2.Y, mat.Row2.Z
                );
        }

        public static OpenTK.Matrix4 ToOpenTKMat4(this TackEngineLib.Main.Matrix4 mat) {
            return new OpenTK.Matrix4(mat.Row0.ToOpenTKVec4(), mat.Row1.ToOpenTKVec4(), mat.Row2.ToOpenTKVec4(), mat.Row3.ToOpenTKVec4());
        }

        public static TackEngineLib.Main.Matrix4 ToTEMat4(this OpenTK.Matrix4 mat) {
            return new TackEngineLib.Main.Matrix4(mat.Row0.ToTEVec4(), mat.Row1.ToTEVec4(), mat.Row2.ToTEVec4(), mat.Row3.ToTEVec4());
        }

        public static OpenTK.Vector4 ToOpenTKVec4(this TackEngineLib.Main.Colour4b col) {
            return new OpenTK.Vector4(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);
        }
    }
}
