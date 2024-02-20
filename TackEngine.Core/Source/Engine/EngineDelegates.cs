/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Engine
{
    public static class EngineDelegates
    {
        public delegate void OnStart();
        public delegate void OnUpdate();
        public delegate void OnGUIRender();
        public delegate void OnClose();
        public delegate void CommandDelegate(string[] a_args);
    }
}
