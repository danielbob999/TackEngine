/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;

using TackEngineLib.Main;
using TackEngineLib.Engine;

namespace TackEngineLib.Main {

    public class SpriteSheet {

        public Sprite[] Sprites { get; private set; }
        public int SpriteCount { get; private set; }

        internal SpriteSheet() {

        }

        public static SpriteSheet LoadFromFile(string path, int sizeX, int sizeY, int countX, int countY) {
            try {
                SpriteSheet newSpriteSheet = new SpriteSheet();

                /*
                Bitmap masterBmp = new Bitmap(path);

                if ((sizeX * countX) > masterBmp.Width || (sizeY * countY) > masterBmp.Height) {
                    throw new Exception("The SpriteSheet master Bitmap is too small for the size/count given");
                }

                newSpriteSheet.Sprites = new Sprite[countX * countY];

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
                */

                newSpriteSheet.SpriteCount = newSpriteSheet.Sprites.Length;

                TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully loaded SpriteSheet with " + newSpriteSheet.SpriteCount + " Sprites");

                return newSpriteSheet;
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to load SpriteSheet from file with path '" + path + "'");
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error Message: " + e.Message);
                return null;
            }
        }
    }
}
