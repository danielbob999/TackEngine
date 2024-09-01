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

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)sprite.Filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)sprite.Filter);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)sprite.WrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)sprite.WrapMode);

            m_sprites.Add(sprite);
        }

        internal override void UpdateSpriteFilterMode(Sprite sprite) {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)sprite.Filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)sprite.Filter);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        internal override void UpdateSpriteWrapMode(Sprite sprite) {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)sprite.WrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)sprite.WrapMode);

            GL.BindTexture(TextureTarget.Texture2D, 0);
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

        public override SpriteSheet LoadSpriteSheetFromFile(string path, int sizeX, int sizeY, int countX, int countY) {
            try {
                SpriteSheet newSpriteSheet = new SpriteSheet(countX * countY);

                Bitmap masterBmp = BitmapFactory.DecodeStream(AndroidContext.CurrentAssetManager.Open(path));

                if ((sizeX * countX) > masterBmp.Width || (sizeY * countY) > masterBmp.Height) {
                    throw new Exception("The SpriteSheet master Bitmap is too small for the size/count given");
                }

                int spriteId = 0;

                for (int y = 0; y < countY; y++) {
                    for (int x = 0; x < countX; x++) {
                        Bitmap spriteBitmap = Bitmap.CreateBitmap(masterBmp, x * sizeX, y * sizeY, sizeX, sizeY);

                        Sprite newSprite = Sprite.LoadFromBitmap(spriteBitmap);
                        newSprite.Create();

                        spriteBitmap.Recycle();

                        newSpriteSheet.Sprites[spriteId] = newSprite;

                        spriteId++;
                    }
                }

                masterBmp.Recycle();

                TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully loaded SpriteSheet with " + newSpriteSheet.SpriteCount + " Sprites");

                return newSpriteSheet;
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to load SpriteSheet from file with path '" + path + "'");
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error Message: " + e.Message);
                return null;
            }
        }

        public override Sprite LoadSpriteFromBitmap(object bitmap) {
            try {
                Sprite newSprite = new Sprite();
                Bitmap newBp = (Bitmap)bitmap;

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

                return newSprite;
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to load sprite");
                TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("'{0}'", e.ToString()));
                return null;
            }
        }
    }
}
