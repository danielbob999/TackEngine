using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.GUI;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Engine
{
    /// <summary>
    /// The holder class for all TackConsole commands
    /// 
    /// Command Args info
    /// - First value in the args array will always be the cmd string, the rest of the args will be values
    ///         - e.g command: "some.command hello 6 "
    ///             - Array: [some.command] [hello] [6]
    /// </summary>
    internal static class EngineConsoleCommands
    {
        [CommandMethod("help", "", "commandName:string")]
        public static void HelpCommmand(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "Commands:");

                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    TackConsole.EngineLog(TackConsole.LogType.Message, "     " + command.CommandCallString);
                }

                return;
            }

            if (args.Length == 2) {
                TackCommand com = null;

                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    if (args[1] == command.CommandCallString) {
                        com = command;
                    }
                }

                if (com != null) {
                    TackConsole.EngineLog(TackConsole.LogType.Message, com.CommandCallString + ":");

                    foreach (string overloadArgs in com.CommandArgList) {
                        if (overloadArgs != "") {
                            TackConsole.EngineLog(TackConsole.LogType.Message, "     [" + overloadArgs + "]");
                        } else {
                            TackConsole.EngineLog(TackConsole.LogType.Message, "     [No Args]");
                        }
                    }
                }

                return;
            }

            //TackConsole.EngineLog(TackConsole.LogType.Message, "The TackCommand with call string '" + thisCommandData.GetCallString() + "' has no definition that takes " + args.Length + " arguments");
        }

        [CommandMethod("renderer.printWidth", "")]
        public static void RendererPrintWidthCommand(string[] args) {
            //TackConsole.EngineLog(TackConsole.LogType.Message, "Current Window Width: {0}", Camera.MainCamera.CameraScreenWidth);
        }

        [CommandMethod("renderer.printHeight", "")]
        public static void RendererPrintHeightCommand(string[] args) {
            //TackConsole.EngineLog(TackConsole.LogType.Message, "Current Window Height: {0}", Camera.MainCamera.CameraScreenHeight);
        }

        [CommandMethod("renderer.enableFpsCounter", "")]
        public static void RendererEnableFpsCounter(string[] args) {
            TackRenderer.Instance.FpsCounterActive = true;
        }

        [CommandMethod("renderer.disableFpsCounter", "")]
        public static void RendererDisableFpsCounter(string[] args) {
            TackRenderer.Instance.FpsCounterActive = false;
        }

        [CommandMethod("console.printOperationsOfCommandclass", "commandClassName:string")]
        public static void ConsolePrintOperationsOfCommandclass(string[] args) {
            if (args.Length == 2) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "Operations of Commandclass: " + args[1]);
                foreach (TackCommand command in TackConsole.GetLoadedTackCommands()) {
                    if (command.CommandCallString.StartsWith(args[1])) {
                        TackConsole.EngineLog(TackConsole.LogType.Message, "      {0} {1} ({2} overloads)", command.CommandCallString.Remove(0, (args[1].Length + 1)), command.CommandArgList.FirstOrDefault(), command.CommandArgList.Count - 1);
                    }
                }
            }
        }

        [CommandMethod("tackengine.restartModule", "keepState:bool")]
        public static void TackEngineRestartModuleCommand(string[] args) {
            if (args.Length == 2) {
                if (args[1] != "") {
                    //TackGameWindow.RestartModule(args[1], false);
                }
            } else if (args.Length == 3) {
                if (args[1] != "") {
                    if (bool.TryParse(args[2], out bool res)) {
                        //TackGameWindow.RestartModule(args[1], res);
                    }
                }
            }
        }

        [CommandMethod("renderer.v-sync", "", "state:bool")]
        public static void RendererSetVSync(string[] args) {
            if (args.Length == 1) {
                //TackConsole.EngineLog(TackConsole.LogType.Message, "Value: " + (TackEngine.Instance.Window.VSync == OpenTK.Windowing.Common.VSyncMode.Off ? "Off" : "On"));
                return;
            }

            if (args.Length == 2) {
                if (bool.TryParse(args[1], out bool result)) {
                    if (result) {
                        //TackEngine.Instance.Window.VSync = OpenTK.Windowing.Common.VSyncMode.On;
                        TackConsole.EngineLog(TackConsole.LogType.Message, "Set renderer.v-sync to value: On");
                    } else {
                        //TackEngine.Instance.Window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
                        TackConsole.EngineLog(TackConsole.LogType.Message, "Set renderer-v-sync to value: Off");
                    }
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Couldn't convert '{0}' to type: bool", args[1]);
                }

                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Incorrect number of args for command: " + args[0]);
        }

        [CommandMethod("renderer.backgroundColour", "", "red:byte green:byte blue:byte")]
        public static void ChangeBackgroundColour(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "Value: " + TackEngine.Core.Renderer.TackRenderer.Instance.BackgroundColour.ToString());
                return;
            }

            if (args.Length == 4) {
                if (byte.TryParse(args[1], out byte r)) {
                    if (byte.TryParse(args[2], out byte g)) {
                        if (byte.TryParse(args[3], out byte b)) {
                            TackEngine.Core.Renderer.TackRenderer.Instance.BackgroundColour = new Colour4b(r, g, b);
                            TackConsole.EngineLog(TackConsole.LogType.Message, "Set tackrenderer.backgroundColour to value: " + Renderer.TackRenderer.Instance.BackgroundColour.ToString());
                        } else {
                            TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[3]);
                            return;
                        }
                    } else {
                        TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[2]);
                        return;
                    }
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[1]);
                    return;
                }
                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Incorrect number of arguments for command: " + args[0]);
        }

        [CommandMethod("lighting.enabled", "", "value:bool")]
        public static void LightingEnableDisabled(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "lighting.enabled current value: " + Renderer.TackLightingSystem.Instance.Enabled);
                return;
            } 
            
            if (args.Length == 2) {
                if (bool.TryParse(args[1], out bool res)) {
                    Renderer.TackLightingSystem.Instance.Enabled = res;
                    TackConsole.EngineLog(TackConsole.LogType.Message, "Set lighting.enabled to value: " + res);
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: bool", args[1]);
                }
            }
        }

        [CommandMethod("lighting.ambientColour", "", "r:byte g:byte b:byte a:byte")]
        public static void LightingSetGetAmbientColour(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "lighting.ambientColour current value: " + Renderer.TackLightingSystem.Instance.AmbientLightColour.ToString());
                return;
            } 
            
            if (args.Length == 5){
                if (byte.TryParse(args[1], out byte r)) {
                    if (byte.TryParse(args[2], out byte g)) {
                        if (byte.TryParse(args[3], out byte b)) {
                            Renderer.TackLightingSystem.Instance.AmbientLightColour = new Colour4b(r, g, b, 255);
                            TackConsole.EngineLog(TackConsole.LogType.Message, "Set lighting.ambientColour to value: " + Renderer.TackLightingSystem.Instance.AmbientLightColour.ToString());
                        } else {
                            TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[3]);
                            return;
                        }
                    } else {
                        TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[2]);
                        return;
                    }
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: byte", args[1]);
                    return;
                }
                return;
            }
        }

        [CommandMethod("lighting.ambientIntensity", "", "value:float")]
        public static void LightingGetSetAmbientIntensity(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "lighting.ambientIntensity current value: " + Renderer.TackLightingSystem.Instance.Enabled);
                return;
            }

            if (args.Length == 2) {
                if (float.TryParse(args[1], out float res)) {
                    Renderer.TackLightingSystem.Instance.AmbientLightIntensity = res;
                    TackConsole.EngineLog(TackConsole.LogType.Message, "Set lighting.ambientIntensity to value: " + res.ToString("0.000"));
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to type: float", args[1]);
                }
            }
        }

        [CommandMethod("tackobject.listAll", "")]
        public static void TackObjectListAllObjects(string[] args) {
            TackConsole.EngineLog(TackConsole.LogType.Message, "Loaded TackObjects:\n");
            TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("{0,-20} | {1,-20} | {2,5}", "Name", "IsActive", "Hash"));
            TackConsole.EngineLog(TackConsole.LogType.Message, "--------------------------------------------------------------");

            Objects.TackObject[] tackObjects = TackObjectManager.Instance.GetAllTackObjects();

            for (int i = 0; i < tackObjects.Length; i++) {
                TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("{0,-20} | {1,-20} | {2,5}", tackObjects[i].Name, true, tackObjects[i].GetHash()));
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "--------------------------------------------------------------");
        }

        [CommandMethod("tackobject.info", "name:string", "hash:string")]
        public static void TackObjectGetInfo(string[] args) {
            if (args.Length == 2) {
                // Try and get the TackObject by treating args[1] has a name
                TackObject calledObject = TackObject.Get(args[1]);

                // If there is no TackObject with name that is equal to args[1]
                //  - Treat args[1] as a hash and look for TackObject based on hash
                if (calledObject == null) {
                    calledObject = TackObject.GetByHash(args[1]);
                }

                // If there is no TackObject with name OR hash equal to args[1], return
                if (calledObject == null) {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "There is no TackObject that has name/hash with value: " + args[1]);
                    return;
                }

                TackConsole.EngineLog(TackConsole.LogType.Message, "TackObject Info");
                TackConsole.EngineLog(TackConsole.LogType.Message, "--------------------------------------");
                TackConsole.EngineLog(TackConsole.LogType.Message, "Name: {0:-20}", calledObject.Name);
                TackConsole.EngineLog(TackConsole.LogType.Message, "Hash: {0:-20}", calledObject.GetHash());
                TackConsole.EngineLog(TackConsole.LogType.Message, "Position: {0:-20}", calledObject.Position.ToString());
                TackConsole.EngineLog(TackConsole.LogType.Message, "Scale: {0:-20}", calledObject.Size.ToString());
                TackConsole.EngineLog(TackConsole.LogType.Message, "Rotation: {0:-20}", calledObject.Rotation);
                TackConsole.EngineLog(TackConsole.LogType.Message, "Components ({0}):", calledObject.GetAllComponents().Length);

                TackComponent[] components = calledObject.GetAllComponents();

                foreach (TackComponent comp in components) {
                    TackConsole.EngineLog(TackConsole.LogType.Message, "          - {0}", comp.GetType().Name);
                }

                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Incorrect number of arguments for command: " + args[0]);
        }

        [CommandMethod("audiomanager.masterVolume", "", "newValue:float")]
        public static void ChangeAudioMasterVolume(string[] args) {
            if (args.Length == 1) {
                //TackConsole.EngineLog(TackConsole.LogType.Message, "Value: " + Audio.AudioManager.Instance.MasterVolume.ToString("0.000"));
                return;
            }

            if (args.Length == 2) {
                if (float.TryParse(args[1], out float res)) {
                    //Audio.AudioManager.Instance.MasterVolume = res;
                    //TackConsole.EngineLog(TackConsole.LogType.Message, "Changed audiomanager.mastervolume to value: " + Audio.AudioManager.Instance.MasterVolume);
                    return;
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Couldn't convert '{0}' to float", args[1]);
                    return;
                }
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Incorrect number of args for command: " + args[1]);
        }

        [CommandMethod("physics.debugdraw", "", "state:bool")]
        public static void EnableDebugDrawCommand(string[] args) {
            if (args.Length == 1) {
                TackConsole.EngineLog(TackConsole.LogType.Message, "Value: " + TackPhysics.GetInstance().ShouldDebugDrawBodies);
                return;
            }

            if (args.Length == 2) {
                if (bool.TryParse(args[1], out bool res)) {
                    TackPhysics.GetInstance().ShouldDebugDrawBodies = res;
                    TackConsole.EngineLog(TackConsole.LogType.Message, "Set {0} to Value: {1}", args[0], res);
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Failed to convert '{0}' to a boolean value", args[1]);
                }

                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Incorrect number of arguments for command: " + args[0]);
        }

        [CommandMethod("quit", "")]
        public static void QuitCommand(string[] args) {
            TackEngineInstance.Quit();
        }
    }
}
