using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using Android.Graphics;
using Java.IO;
using Android.Hardware.Lights;
using Java.Nio;
using static Android.Icu.Text.ListFormatter;
using System.Collections;
using System.Runtime.InteropServices;

namespace TackEngine.Android {
    internal class AndroidSpriteManager : SpriteManager {
        public AndroidSpriteManager() {
            Instance = this;
            m_sprites = new List<Sprite>();

            LoadDefaultSprite();

            LoadDefaultUISprite();
        }


        public override void DeleteSprite(Sprite sprite, bool _debugMsgs = true) {
            int id = sprite.Id;
            GL.DeleteTextures(1, ref id);
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
            Bitmap newBp = BitmapFactory.DecodeStream(AndroidContext.CurrentAssetManager.Open(path));

            if (newBp == null) {
                throw new System.Exception("Could not load bitmap");
            }

            newSprite.Width = newBp.Width;
            newSprite.Height = newBp.Height;
            newSprite.PixelFormat = (Sprite.SpritePixelFormat)newBp.GetBitmapInfo().Format;

            int size = newBp.Width * newBp.Height * 4; // 4 bytes per pixel
            newSprite.Data = new byte[size]; 
            var byteBuffer = Java.Nio.ByteBuffer.AllocateDirect(size);
            newBp.CopyPixelsToBuffer(byteBuffer);
            Marshal.Copy(byteBuffer.GetDirectBufferAddress(), newSprite.Data, 0, size);     
            byteBuffer.Dispose();

            return newSprite;
        }

        internal void LoadDefaultSprite() {
            /*
            Bitmap defaultBitmap = Bitmap.CreateBitmap(32, 32, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(defaultBitmap);
            canvas.DrawColor(Color.White);

            Sprite.DefaultSprite = new Sprite();

            Sprite.DefaultSprite.Width = defaultBitmap.Width;
            Sprite.DefaultSprite.Height = defaultBitmap.Height;

            AndroidBitmapInfo info = defaultBitmap.GetBitmapInfo();

            int[] pixels = new int[(int)TackEngine.Core.Math.TackMath.Abs(defaultBitmap.GetBitmapInfo().Stride) * defaultBitmap.Height];

            defaultBitmap.GetPixels(pixels, 0, (int)info.Stride, 0, 0, 32, 32);

            Sprite.DefaultSprite.Data = new byte[pixels.Length * sizeof(int)];
            System.Buffer.BlockCopy(pixels, 0, Sprite.DefaultSprite.Data, 0, Sprite.DefaultSprite.Data.Length);

            //Sprite.DefaultSprite.PixelFormat = (Sprite.SpritePixelFormat)defaultBitmap.GetBitmapInfo().Format;
            //Sprite.DefaultSprite.m_stride = defaultBitmap.
            //Sprite.DefaultSprite.Data = new byte[TackEngine.Core.Math.TackMath.Abs(defaultBitmap.GetBitmapInfo().Stride) * defaultBitmap.Height];

            //System.Runtime.InteropServices.Marshal.Copy(pixel, Sprite.DefaultSprite.Data, 0, Sprite.DefaultSprite.Data.Length);

            //defaultBitmap.UnlockBits(bmpData);

            Sprite.DefaultSprite.Create();
            Sprite.DefaultSprite.IsNineSliced = false;

            //defaultBitmap.Dispose();*/

            Sprite defaultSprite = LoadFromFile("tackresources/sprites/default_sprite.png");
            defaultSprite.Create();

            Sprite.DefaultSprite = defaultSprite;

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default sprite into Sprite.DefaultSprite");
        }

        internal void LoadDefaultUISprite() {
            /*
            Sprite.DefaultUISprite = LoadSpriteFromFile("tackresources/sprites/ui/ui_panel.png");
            Sprite.DefaultUISprite.Create();
            Sprite.DefaultUISprite.IsNineSliced = true;
            Sprite.DefaultUISprite.NineSlicedData = new TackEngine.Core.Main.Sprite.SliceData(20);
            */

            Sprite.DefaultUISprite = Sprite.DefaultSprite;

            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded the default UI sprite into Sprite.DefaultUISprite");
        }
    }
}
