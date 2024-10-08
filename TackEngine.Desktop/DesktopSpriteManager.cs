﻿using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using static TackEngine.Core.Main.Sprite;
using System.IO;

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

            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sprite.Width, sprite.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, sprite.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)sprite.Filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)sprite.Filter);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)sprite.WrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)sprite.WrapMode);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            m_sprites.Add(sprite);
        }

        public override Sprite LoadFromFile(string path) {
            Sprite newSprite = new Sprite();
            Bitmap newBp;

            string fullPath = path;

            if (!Path.IsPathFullyQualified(path)) {
                fullPath = System.IO.Directory.GetCurrentDirectory() + "\\" + path;
            }

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
            newSprite.PixelFormat = (Sprite.SpritePixelFormat)newBp.PixelFormat;
            newSprite.Data = new byte[newBp.Width * newBp.Height * 4];

            for (int y = 0; y < newBp.Height; y++) {
                for (int x = 0; x < newBp.Width; x++) {
                    Color col = newBp.GetPixel(x, y);

                    newSprite.InternalSetPixel(x, y, new Colour4b(col.R, col.G, col.B, col.A));
                }
            }

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

        public override SpriteSheet LoadSpriteSheetFromFile(string path, int sizeX, int sizeY, int countX, int countY) {
            try {
                SpriteSheet newSpriteSheet = new SpriteSheet(countX * countY);

                Bitmap masterBmp = new Bitmap(path);

                if ((sizeX * countX) > masterBmp.Width || (sizeY * countY) > masterBmp.Height) {
                    throw new Exception("The SpriteSheet master Bitmap is too small for the size/count given");
                }

                int spriteId = 0;

                for (int y = 0; y < countY; y++) {
                    for (int x = 0; x < countX; x++) {
                        Bitmap spriteBitmap = masterBmp.Clone(new Rectangle(x * sizeX, y * sizeY, sizeX, sizeY), masterBmp.PixelFormat);

                        Sprite newSprite = Sprite.LoadFromBitmap(spriteBitmap);
                        newSprite.Create();

                        newSpriteSheet.Sprites[spriteId] = newSprite;

                        spriteBitmap.Dispose();

                        spriteId++;
                    }
                }

                masterBmp.Dispose();

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
                newSprite.PixelFormat = (Sprite.SpritePixelFormat)newBp.PixelFormat;
                newSprite.Data = new byte[newBp.Width * newBp.Height * 4];

                for (int y = 0; y < newBp.Height; y++) {
                    for (int x = 0; x < newBp.Width; x++) {
                        Color col = newBp.GetPixel(x, y);

                        newSprite.InternalSetPixel(x, y, new Colour4b(col.R, col.G, col.B, col.A));
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
