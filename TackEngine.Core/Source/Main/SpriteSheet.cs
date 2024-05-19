/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;

namespace TackEngine.Core.Main {

    public class SpriteSheet {

        public Sprite[] Sprites { get; private set; }
        public int SpriteCount { 
            get {
                if (Sprites != null) {
                    return Sprites.Length;
                }

                return 0;
            } 
        }

        internal SpriteSheet(int count) {
            Sprites = new Sprite[count];
        }

        public static SpriteSheet LoadFromFile(string path, int sizeX, int sizeY, int countX, int countY) {
            return SpriteManager.Instance.LoadSpriteSheetFromFile(path, sizeX, sizeY, countX, countY);
        }
    }
}
