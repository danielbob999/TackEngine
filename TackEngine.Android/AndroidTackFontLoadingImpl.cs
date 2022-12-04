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

            // set 1 byte pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (int i = 0; i < 128; i++) {
                char c = (char)i;
                newFont.FontCharacters.Add(c, LoadCharacter(null, c));
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded new TackFont with name " + "KJHSAJKDH");

            return newFont;
        }

        public TackFont.FontCharacter LoadCharacter(Face fance, uint charCode) {
            /*
            // Create an empty, mutable bitmap
            Bitmap bitmap = Bitmap.CreateBitmap(256, 256, Bitmap.Config.Argb8888);
            // get a canvas to paint over the bitmap
            Canvas canvas = new Canvas(bitmap);
            bitmap.EraseColor(0);

            // Draw the text
            Paint textPaint = new Paint();
            textPaint.TextSize = 32;
            textPaint.AntiAlias = true;
            textPaint.SetARGB(0xff, 0xFF, 0x00, 0x00);
            // draw the text centered
            canvas.DrawText(((char)charCode).ToString(), 5, 5, textPaint);

            int size = bitmap.Width * bitmap.Width * 4; // 4 bytes per pixel
            byte[] data = new byte[size];
            var byteBuffer = Java.Nio.ByteBuffer.AllocateDirect(size);
            bitmap.CopyPixelsToBuffer(byteBuffer);
            Marshal.Copy(byteBuffer.GetDirectBufferAddress(), data, 0, size);
            byteBuffer.Dispose();

            bitmap.Recycle();

            // create glyph texture
            int texObj = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texObj);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.ES30.PixelFormat.Rgba, PixelType.UnsignedByte, data);
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
            ch.size = new Vector2f(bitmap.Width, bitmap.Height);
            //ch.bearing = new Vector2f(glyph.Metrics.HorizontalBearingX.ToSingle(), glyph.Metrics.HorizontalBearingY.ToSingle());
            //ch.advance = (int)glyph.Advance.X.Value;

            return ch;
            */

            // Draw the text
            Paint textPaint = new Paint();
            textPaint.TextSize = 50;
            textPaint.AntiAlias = true;
            textPaint.SetARGB(255, 255, 255, 255);

            string charAsStr = ((char)charCode).ToString();

            Vector2f bmpSize = new Vector2f(textPaint.MeasureText(charAsStr), textPaint.TextSize);

            if (bmpSize.X == 0 || bmpSize.Y == 0) {
                return default(TackFont.FontCharacter);
            }

            // Create an empty, mutable bitmap
            Bitmap bitmap = Bitmap.CreateBitmap((int)bmpSize.X, (int)bmpSize.Y, Bitmap.Config.Argb8888);
            // get a canvas to paint over the bitmap
            Canvas canvas = new Canvas(bitmap);

            //bitmap.EraseColor(Color.Argb(0, 0, 0, 0).ToArgb());
            bitmap.EraseColor(Color.Argb(0, 255, 0, 0).ToArgb());

            //textPaint.SetTypeface(Typef)
            // draw the text centered
            canvas.DrawText(((char)charCode).ToString(), 0, textPaint.TextSize, textPaint);

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
            ch.bearing = new Vector2f(0, 0);
            ch.advance = 0;

            bitmap.Recycle();

            return ch;
        }
    }
}
