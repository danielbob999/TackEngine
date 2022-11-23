using System;
using System.Collections.Generic;
using System.Text;
using TackEngineLib.Engine;
using TackEngineLib.Main;
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
            newSprite.Data = new byte[TackEngineLib.Math.TackMath.Abs(bmpData.Stride) * newBp.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newSprite.Data, 0, newSprite.Data.Length);

            newBp.UnlockBits(bmpData);
            newBp.Dispose();

            return newSprite;
        }

        internal void LoadDefaultSprite() {
            Bitmap defaultBitmap = new Bitmap(32, 32);
            Graphics g = Graphics.FromImage(defaultBitmap);
            g.Clear(Color.White);

            Sprite.DefaultSprite = new Sprite();

            Sprite.DefaultSprite.Width = defaultBitmap.Width;
            Sprite.DefaultSprite.Height = defaultBitmap.Height;

            BitmapData bmpData = defaultBitmap.LockBits(new System.Drawing.Rectangle(0, 0, defaultBitmap.Width, defaultBitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Sprite.DefaultSprite.PixelFormat = (Sprite.SpritePixelFormat)bmpData.PixelFormat;
            //Sprite.DefaultSprite.m_stride = bmpData.Stride;
            Sprite.DefaultSprite.Data = new byte[TackEngineLib.Math.TackMath.Abs(bmpData.Stride) * defaultBitmap.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, Sprite.DefaultSprite.Data, 0, Sprite.DefaultSprite.Data.Length);

            defaultBitmap.UnlockBits(bmpData);

            Sprite.DefaultSprite.Create();
            Sprite.DefaultSprite.IsNineSliced = false;

            defaultBitmap.Dispose();

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }

        internal void LoadDefaultUISprite() {
            Sprite.DefaultUISprite = LoadFromFile("tackresources/sprites/ui/ui_panel.png");
            Sprite.DefaultUISprite.Create();
            Sprite.DefaultUISprite.IsNineSliced = true;
            Sprite.DefaultUISprite.NineSlicedData = new TackEngineLib.Main.Sprite.SliceData(20);

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default UI sprite into Sprite.DefaultUISprite");
        }
    }
}
