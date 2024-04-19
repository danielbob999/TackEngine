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
using PixelFormat = OpenTK.Graphics.ES30.PixelFormat;

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

            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sprite.Width, sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, sprite.Data);

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
            newSprite.Data = new byte[newBp.Width * newBp.Height * 4];

            for (int y = 0; y < newBp.Height; y++) {
                for (int x = 0; x < newBp.Width; x++) {
                    int col = newBp.GetPixel(x, y);

                    newSprite.InternalSetPixel(x, y, new Colour4b((byte)Color.GetRedComponent(col), (byte)Color.GetGreenComponent(col), (byte)Color.GetBlueComponent(col), (byte)Color.GetAlphaComponent(col)));
                }
            }

            newBp.Recycle();

            return newSprite;
        }

        internal void LoadDefaultSprite() {
            Sprite defaultSprite = LoadFromFile("tackresources/sprites/default_sprite.png");
            defaultSprite.Create();

            Sprite.DefaultSprite = defaultSprite;

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
