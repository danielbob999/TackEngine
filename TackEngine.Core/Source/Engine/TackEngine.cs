﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Engine
{
    /// <summary>
    /// The main engine class
    /// </summary>
    public class TackEngine
    {
        private const int VERSION_MAJOR = 1;
        private const int VERSION_MINOR = 8;
        private const int VERSION_PATCH = 0;

        public class InitalisationSettings {
            public enum TackWindowBorder {
                Resizable = 0,
                Fixed
            }

            public enum TackWindowState {
                Normal = 0,
                Minimized,
                Maximized,
                Fullscreen
            }

            /// <summary>
            /// The size of this window
            /// </summary>
            public Vector2i WindowSize { get; set; }
            /// <summary>
            /// The title of the window
            /// </summary>
            public string WindowTitle { get; set; }
            /// <summary>
            /// The location of this window
            /// </summary>
            public Vector2i WindowLocation { get; set; }
            /// <summary>
            /// The border of this window. Fixed/Resizable
            /// </summary>
            public TackWindowBorder WindowBorder { get; set; }
            /// <summary>
            /// The state of this window. Normal/Fullscreen
            /// </summary>
            public TackWindowState WindowState { get; set; }
            /// <summary>
            /// The target update frequency (per second)
            /// </summary>
            public int TargetUpdateFrequency { get; set; }
            /// <summary>
            /// The target render frequency (per second). This is overrided if VSync == true
            /// </summary>
            public int TargetRenderFrequency { get; set; }
            /// <summary>
            /// Whether or not vsync is used
            /// </summary>
            public bool VSync { get; set; }
            /// <summary>
            /// The sample count for MSAA. Will use the closest value that the current platform supports
            /// </summary>
            public int MSAASampleCount { get; set; }

            public InitalisationSettings() {
                WindowSize = new Vector2i(800, 600);
                WindowLocation = new Vector2i(0, 0);
                WindowTitle = "TackEngine Game";
                WindowBorder = TackWindowBorder.Resizable;
                WindowState = TackWindowState.Normal;
                TargetUpdateFrequency = 60;
                TargetRenderFrequency = 60;
                VSync = false;
                MSAASampleCount = 4;
            }
        }

        public static TackEngine Instance = null;

        //internal TackGameWindow Window { get; private set; }
        internal IBaseTackWindow Window { get; private set; }
        public InitalisationSettings Settings { get; private set; }

        public TackEngine() { }

        public static void Initialise(object window) {
            // Create a new instance of TackEngine. This is tracked and used by calling TackEngine.Instance
            Instance = new TackEngine();
            Instance.Window = (IBaseTackWindow)window;
            

            TackConsole.EngineLog(TackConsole.LogType.Message, "Starting TackEngine.");
            TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("EngineVersion: {0}", GetEngineVersion().ToString()));

            //Instance.Window = new TackGameWindow(settings, startFn, updateFn, closeFn, Instance.Console);
            //Instance.Window.Run();

            //TackConsole.EngineLog(TackConsole.LogType.Message, "Shutdown of TackEngine completed\n\n");          
        }

        public static void Shutdown() {
            TackConsole.EngineLog(TackConsole.LogType.Message, "TackEngine shutdown requested");
        }

        public static TackEngineVersion GetEngineVersion() {
            return new TackEngineVersion(VERSION_MAJOR, VERSION_MINOR, VERSION_PATCH);
        }
    }
}
