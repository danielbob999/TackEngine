/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Engine;

namespace TackEngine.Core.Engine {
    internal abstract class SpriteManager {
        public static SpriteManager Instance { get; protected set; }

        protected List<Sprite> m_sprites;

        public virtual void OnStart() {

        }

        public virtual void OnClose() {
            foreach (Sprite s in m_sprites) {
                DeleteSprite(s);
            }
        }

        public abstract void RegisterSprite(Sprite sprite, bool debugMsgs = true);

        public abstract void DeleteSprite(Sprite _sprite, bool _debugMsgs = true);

        public abstract Sprite LoadFromFile(string path);
    }
}
