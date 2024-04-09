using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TackEngine.Core.GUI;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;

namespace TackEngine.Core.SceneManagement {
    public class SceneManager {
        public static SceneManager Instance { get; private set; }

        internal List<Type> SceneTypes { get; private set; }
        internal Scene CurrentScene { get; private set; }

        internal SceneManager(List<Type> sceneTypes) {
            Instance = this;

            if (sceneTypes == null || sceneTypes.Count == 0) {
                throw new Exception("Cannot start TackEngine without atleast one scene");
            }

            SceneTypes = new List<Type>();

            foreach (Type type in sceneTypes) {
                if (type.IsSubclassOf(typeof(Scene))) {
                    if (!SceneTypes.Contains(type)) {
                        SceneTypes.Add(type);
                    }
                }
            }
        }

        internal void LoadFirstScene() {
            LoadScene(SceneTypes[0]);
        }

        public void LoadScene(Type type) {
            if (!SceneTypes.Contains(type)) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Cannot load scene of type '{0}' because it is not a valid scene type", type.Name);
                return;
            }

            if (CurrentScene != null) {
                if (type == CurrentScene.GetType()) {
                    TackConsole.EngineLog(TackConsole.LogType.Warning, "Cannot load scene of type '{0}' because it is already loaded", type.Name);
                    return;
                }

                // Unload the old scene, if one exists
                CurrentScene.Close();

                // Call for the TackObjectManager to destroy all TackObjects associated with the old scene
                TackObjectManager.Instance.DeregisterAllObjectsOfSceneType(CurrentScene.GetType());

                // Call for all the scene linked GUIObjects to be deleted
                BaseTackGUI.Instance.DeregisterAllSceneLinkedGUIObjects();
            }

            CurrentScene = (Scene)Activator.CreateInstance(type);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            TackConsole.EngineLog(TackConsole.LogType.Message, "Start loading scene: '{0}'", CurrentScene.GetType().Name);

            CurrentScene.Initialise();
            watch.Stop();

            TackConsole.EngineLog(TackConsole.LogType.Message, "Finished load scene '{0}'. Took {1}", CurrentScene.GetType().Name, (watch.ElapsedMilliseconds > 1000 ? watch.Elapsed.TotalSeconds.ToString("0.000") + "s" : watch.ElapsedMilliseconds.ToString("0") + "ms"));
        }
    }
}
