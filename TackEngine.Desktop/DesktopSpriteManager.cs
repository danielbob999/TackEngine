using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace TackEngine.Desktop {
    internal class DesktopSpriteManager : SpriteManager {
        public DesktopSpriteManager() {
            Instance = this;
            m_sprites = new List<Sprite>();

            // load default sprite
            LoadDefaultSprite();

            // load default ui sprite
            LoadDefaultUISprite();
        }


        public override void DeleteSprite(Sprite sprite, bool _debugMsgs = true) {
            OpenTK.Graphics.OpenGL.GL.DeleteTexture(sprite.Id);
        }

        public override void RegisterSprite(Sprite sprite, bool debugMsgs = true) {
            int newId;
            GL.GenTextures(1, out newId);

            sprite.Id = newId;

            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sprite.Width, sprite.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, sprite.Data);

            m_sprites.Add(sprite);
        }

        public override Sprite LoadFromFile(string path) {
            Sprite newSprite = new Sprite();
            Bitmap newBp;

            string fullPath = System.IO.Directory.GetCurrentDirectory() + "\\" + path;

            try {
                newBp = new Bitmap(fullPath);
            } catch (System.IO.FileNotFoundException) {
                TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("Failed to load image data. No file found at path: '{0}'", path));
                return newSprite;
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("'{0}'", e.ToString()));
                return newSprite;
            }

            newSprite.Width = newBp.Width;
            newSprite.Height = newBp.Height;

            BitmapData bmpData = newBp.LockBits(new System.Drawing.Rectangle(0, 0, newBp.Width, newBp.Height),
                ImageLockMode.ReadOnly, newBp.PixelFormat);

            newSprite.PixelFormat = (Sprite.SpritePixelFormat)bmpData.PixelFormat;
            //newSprite.m_stride = bmpData.Stride;
            newSprite.Data = new byte[TackEngine.Core.Math.TackMath.Abs(bmpData.Stride) * newBp.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newSprite.Data, 0, newSprite.Data.Length);

            newBp.UnlockBits(bmpData);
            newBp.Dispose();

            return newSprite;
        }

        internal void LoadDefaultSprite() {
            Sprite.DefaultSprite = LoadFromFile("tackresources/sprites/default_sprite.png");
            Sprite.DefaultSprite.Create();

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }

        internal void LoadDefaultUISprite() {
            Sprite.DefaultUISprite = LoadFromFile("tackresources/sprites/ui/ui_panel.png");
            Sprite.DefaultUISprite.Create();
            Sprite.DefaultUISprite.IsNineSliced = true;
            Sprite.DefaultUISprite.NineSlicedData = new TackEngine.Core.Main.Sprite.SliceData(20);

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default UI sprite into Sprite.DefaultUISprite");
        }
    }
}
