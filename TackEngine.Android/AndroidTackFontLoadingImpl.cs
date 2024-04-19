using SharpFont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.GUI;
using TackEngine.Core.Main;
using OpenTK.Graphics.ES30;
using System.Diagnostics.Tracing;
using TackEngine.Core.Renderer;
using Android.Content;
using Android.Graphics;
using System.Runtime.InteropServices;
using Android.Graphics.Fonts;

namespace TackEngine.Android {
    internal class AndroidTackFontLoadingImpl : ITackFontLoadingImpl {
        public TackEngine.Core.GUI.TackFont LoadFromFile(string path) {
            byte[] fileData;

            using (Stream stream = AndroidContext.CurrentAssetManager.Open(path)) {
                using (MemoryStream memStream = new MemoryStream()) {
                    stream.CopyTo(memStream);

                    fileData = memStream.ToArray();
                }
            }

            TackFont newFont = new TackFont();
            newFont.FontFace = new Face(BaseTackGUI.Instance.FontLibrary, fileData, 0);
            newFont.FontFace.SetPixelSizes(0, 50);

            // set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (int i = 0; i < 128; i++) {
                char c = (char)i;
                newFont.FontCharacters.Add(c, LoadCharacter(newFont.FontFace, c));
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded new TackFont with name " + "KJHSAJKDH");

            return newFont;
        }

        public TackFont.FontCharacter LoadCharacter(Face face, uint charCode) {
            face.LoadChar(charCode, LoadFlags.Render, LoadTarget.Normal);

            GlyphSlot glyph = face.Glyph;
            FTBitmap tbmp = glyph.Bitmap;

            if (tbmp.Width == 0 || tbmp.Rows == 0) {
                return new TackFont.FontCharacter() { texId = -1 };
            }

            // Create an empty, mutable bitmap
            Bitmap bitmap = Bitmap.CreateBitmap((int)tbmp.Width, (int)tbmp.Rows, Bitmap.Config.Argb8888);

            int byteIndex = 0;

            for (int y = 0; y < bitmap.Height; y++) {
                for (int x = 0; x < bitmap.Width; x++) {
                    bitmap.SetPixel(x, y, new Color(
                        (byte)0,
                        (byte)0,
                        (byte)0,
                        (byte)tbmp.BufferData[byteIndex]));

                    byteIndex += 1;
                }
            }

            int size = bitmap.Width * bitmap.Height * 4; // 4 bytes per pixel
            byte[] data = new byte[size];
            var byteBuffer = Java.Nio.ByteBuffer.AllocateDirect(size);
            bitmap.CopyPixelsToBuffer(byteBuffer);
            Marshal.Copy(byteBuffer.GetDirectBufferAddress(), data, 0, size);
            byteBuffer.Dispose();

            // create glyph texture
            int texObj = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texObj);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.ES30.PixelFormat.Rgba, PixelType.UnsignedByte, data);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Rows, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bitmap.Buffer);

            // set texture filter parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // add character
            TackFont.FontCharacter ch = new TackFont.FontCharacter();
            ch.texId = texObj;
            ch.size = new Vector2f(bitmap.Width, bitmap.Height);
            ch.bearing = new Vector2f(glyph.Metrics.HorizontalBearingX.ToSingle(), glyph.Metrics.HorizontalBearingY.ToSingle());
            ch.advance = (int)glyph.Advance.X.Value;

            bitmap.Recycle();

            return ch;
        }
    }
}
