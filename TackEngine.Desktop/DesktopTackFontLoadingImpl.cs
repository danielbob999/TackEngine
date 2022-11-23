using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.GUI;
using TackEngineLib.Main;
using OpenTK.Graphics.OpenGL;

namespace TackEngine.Desktop {
    internal class DesktopTackFontLoadingImpl : ITackFontLoadingImpl {
        public TackEngineLib.GUI.TackFont LoadFromFile(string path) {
            byte[] fileData = System.IO.File.ReadAllBytes(path);

            TackFont newFont = new TackFont();
            newFont.FontFace = new Face(BaseTackGUI.Instance.FontLibrary, fileData, 0);
            newFont.FontFace.SetPixelSizes(0, 50);

            // set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (int i = 0; i < 128; i++) {
                char c = (char)i;
                newFont.FontCharacters.Add(c, LoadCharacter(newFont.FontFace, c));
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded new TackFont with name " + newFont.FontFace.FamilyName);

            return newFont;
        }

        public TackFont.FontCharacter LoadCharacter(Face face, uint charCode) {
            face.LoadChar(charCode, LoadFlags.Render, LoadTarget.Normal);

            GlyphSlot glyph = face.Glyph;
            FTBitmap bitmap = glyph.Bitmap;

            // create glyph texture
            int texObj = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texObj);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, bitmap.Width, bitmap.Rows, 0, PixelFormat.Red, PixelType.UnsignedByte, bitmap.Buffer);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Rows, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.Buffer);

            // set texture filter parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // add character
            TackFont.FontCharacter ch = new TackFont.FontCharacter();
            ch.texId = texObj;
            ch.size = new Vector2f(bitmap.Width, bitmap.Rows);
            ch.bearing = new Vector2f(glyph.Metrics.HorizontalBearingX.ToSingle(), glyph.Metrics.HorizontalBearingY.ToSingle());
            ch.advance = (int)glyph.Advance.X.Value;

            return ch;
        }
    }
}
