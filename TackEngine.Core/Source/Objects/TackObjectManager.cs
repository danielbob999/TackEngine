/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Renderer;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.SceneManagement;

namespace TackEngine.Core.Objects {
    internal class TackObjectManager {
        public static TackObjectManager Instance;

        private Dictionary<string, TackObject> m_tackObjects;
        private int m_nextHashIdNumber = 0;
        private bool m_runComponentCallbacks = true;

        public TackObjectManager(bool runCompCallbacks = true) {
            Instance = this;
            m_runComponentCallbacks = runCompCallbacks;

            m_tackObjects = new Dictionary<string, TackObject>();
        }

        public void OnStart() {
            double startTime = EngineTimer.Instance.TotalRunTime;

            if (m_runComponentCallbacks) {
                RunTackObjectStartMethods();
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "TackObjectManager started in " + (EngineTimer.Instance.TotalRunTime - startTime).ToString("0.000") + " seconds");
        }

        public void OnUpdate() {
            if (m_runComponentCallbacks) {
                TackProfiler.Instance.StartTimer("TackObject.Component.OnUpdate");
                RunTackObjectUpdateMethods();
                TackProfiler.Instance.StopTimer("TackObject.Component.OnUpdate");
            }

            TackProfiler.Instance.StartTimer("TackObject.DetectMouse");

            Vector2f mouseWorldCoords = Input.TackInput.Instance.MousePositionInWorld;

            if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                mouseWorldCoords = Input.TackInput.Instance.TouchPositionInWorld;
            }

            foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                if (!TackObject.IsActiveInHierarchy(pair.Value)) {
                    continue;
                }

                Vector2f objPos = pair.Value.Position;
                Vector2f objScale = pair.Value.Size;

                if (Physics.AABB.IsPointInAABBWorld(new Physics.AABB(new Vector2f(objPos.X - (objScale.X / 2.0f), objPos.Y - (objScale.Y / 2.0f)), new Vector2f(objPos.X + (objScale.X / 2.0f), objPos.Y + (objScale.Y / 2.0f))), mouseWorldCoords)) {
                    TackComponent[] components = pair.Value.GetAllComponents();
                    
                    for (int c = 0; c < components.Length; c++) {
                        if (pair.Value.MouseHoverLastFrame) {
                            components[c].OnMouseOver();
                        } else {
                            components[c].OnMouseEnter();
                        }

                        if (Input.TackInput.MouseButtonDown(Input.MouseButtonKey.Left) || Input.TackInput.TouchDown()) {
                            if (!TackRenderer.Instance.GUIInstance.MouseClickDetectedThisFrame) {
                                components[c].OnMouseClick();
                            }
                        }
                    }

                    pair.Value.MouseHoverLastFrame = true;
                } else {
                    if (pair.Value.MouseHoverLastFrame) {
                        pair.Value.MouseHoverLastFrame = false;

                        TackComponent[] components = pair.Value.GetAllComponents();

                        for (int c = 0; c < components.Length; c++) {
                            components[c].OnMouseExit();
                        }
                    }
                }
            }

            TackProfiler.Instance.StopTimer("TackObject.DetectMouse");
        }

        private void RunTackObjectStartMethods() {
            TackObject[] objList = m_tackObjects.Values.ToArray();

            foreach (TackObject tobj in objList) {
                TackComponent[] components = tobj.GetAllComponents();

                for (int c = 0; c < components.Length; c++) {
                    if (TackObject.IsComponentActiveInHierarchy(components[c])) {
                       components[c].OnStart();
                    }
                }

                tobj.InternalState = TackObject.TackObjectState.Looping;
            }
        }

        private void RunTackObjectUpdateMethods() {
            TackObject[] objList = m_tackObjects.Values.ToArray();

            foreach (TackObject tobj in objList) {
                if (tobj != null) {
                    TackComponent[] components = tobj.GetAllComponents();

                    for (int c = 0; c < components.Length; c++) {
                        if (components[c] != null) {
                            if (TackObject.IsComponentActiveInHierarchy(components[c])) {
                                if (tobj.InternalState == TackObject.TackObjectState.Initialised) {
                                    components[c].OnStart();
                                } else {
                                    components[c].OnUpdate();
                                }
                            }
                        }
                    }

                    if (tobj.InternalState == TackObject.TackObjectState.Initialised) {
                        tobj.InternalState = TackObject.TackObjectState.Looping;
                    }
                }
            }
        }

        internal TackObject GetByHash(string hash) {
            if (hash == null) {
                return null;
            }

            if (m_tackObjects.ContainsKey(hash)){
                return m_tackObjects[hash];
            }

            return null;
        }

        internal TackObject GetByName(string name) {
            foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                if (pair.Value.Name == name) {
                    return pair.Value;
                }
            }

            return null;
        }

        internal void RegisterTackObject(TackObject obj) {
            if (SceneManager.Instance.CurrentScene != null) {
                obj.LinkedSceneType = SceneManager.Instance.CurrentScene.GetType();
            }

            m_tackObjects.Add(obj.Hash, obj);
        }

        internal void DeregisterTackObject(TackObject obj) {
            if (m_tackObjects.ContainsKey(obj.Hash)) {
                // Create a list of all of the objects that we will remove
                // This will be populated with this object and all it's children
                List<string> objectsToRemove = new List<string>();
                objectsToRemove.Add(obj.Hash);

                foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                    List<string> currentParentChain = new List<string>();
                    TackObject currentObject = pair.Value;
                    bool parentMarkedForDeletion = false;


                    // Keep going through the parents whilst there is a valid parent
                    while (currentObject != null) {
                        if (objectsToRemove.Contains(currentObject.Hash)) {
                            parentMarkedForDeletion = true;
                            break;
                        }

                        currentParentChain.Add(currentObject.Hash);

                        currentObject = currentObject.Parent;
                    }

                    // Once we have gotten here, we have either found the root object OR the current object is already marked for deletion

                    // If we have gotten here because we found a parent marked for deletion,
                    //      mark all objects between this object and the one marked for deletion, for deletion
                    if (parentMarkedForDeletion) {
                        objectsToRemove.AddRange(currentParentChain);
                    }
                }

                for (int i = 0; i < objectsToRemove.Count; i++) {
                    TackComponent[] components = m_tackObjects[objectsToRemove[i]].GetAllComponents();

                    for (int c = 0; c < components.Length; c++) {
                        m_tackObjects[objectsToRemove[i]].RemoveComponent(components[c]);
                    }

                    m_tackObjects.Remove(objectsToRemove[i]);
                }
            }
        }

        public TackObject[] GetAllTackObjects() {
            return m_tackObjects.Values.ToArray();
        }

        internal string GenerateNewHash() {
            int hashNum = m_nextHashIdNumber;
            m_nextHashIdNumber++;

            return hashNum.ToString("0000000000000000");
        }

        internal void DeregisterAllObjectsOfSceneType(Type type) {
            List<TackObject> objectsToDeregister = new List<TackObject>();

            foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                if (pair.Value.LinkedSceneType == SceneManager.Instance.CurrentScene.GetType() && string.IsNullOrEmpty(pair.Value.InternalParentHash)) {
                    objectsToDeregister.Add(pair.Value);
                }
            }

            for (int i = 0; i < objectsToDeregister.Count; i++) {
                DeregisterTackObject(objectsToDeregister[i]);
            }
        }
    }
}
