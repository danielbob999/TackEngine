﻿using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;

namespace TackEngine.Core.Engine {
    public interface IBaseTackWindow {
        Vector2f WindowSize { get; }
        ulong CurrentUpdateLoopIndex { get; }
        ulong CurrentRenderLoopIndex { get; }

        public void Quit();
    }
}
