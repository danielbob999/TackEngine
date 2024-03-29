﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Renderer
{
    internal class RenderObject
    {
        public bool renderSprite = true;
        public RectangleShape rectangle;
        public Colour4b colour;
        public Main.Sprite sprite;

        public RenderObject(RectangleShape _r, Colour4b _c, Main.Sprite _sprite, bool _shs)
        {
            rectangle = _r;
            colour = _c;
            sprite = _sprite;
            renderSprite = _shs;
        }
    }
}
