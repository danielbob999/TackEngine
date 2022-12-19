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

namespace TackEngine.Core.Objects {
    internal class TackObjectManager {
        public static TackObjectManager Instance;

        private Dictionary<string, TackObject> m_tackObjects;
        private int m_nextHashIdNumber = 0;

        public TackObjectManager() {
            Instance = this;

            m_tackObjects = new Dictionary<string, TackObject>();
        }

        public void OnStart() {
            double startTime = EngineTimer.Instance.TotalRunTime;

            RunTackObjectStartMethods();

            TackConsole.EngineLog(TackConsole.LogType.Message, "TackObjectManager started in " + (EngineTimer.Instance.TotalRunTime - startTime).ToString("0.000") + " seconds");
        }

        public void OnUpdate() {
            TackProfiler.Instance.StartTimer("TackObject.Component.OnUpdate");
            RunTackObjectUpdateMethods();
            TackProfiler.Instance.StopTimer("TackObject.Component.OnUpdate");

            TackProfiler.Instance.StartTimer("TackObject.DetectMouse");

            Vector2f mouseWorldCoords = Input.TackInput.Instance.MousePositionInWorld;

            foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                Vector2f objPos = pair.Value.Position;
                Vector2f objScale = pair.Value.Scale;

                if (Physics.AABB.IsPointInAABBWorld(new Physics.AABB(new Vector2f(objPos.X - (objScale.X / 2.0f), objPos.Y - (objScale.Y / 2.0f)), new Vector2f(objPos.X + (objScale.X / 2.0f), objPos.Y + (objScale.Y / 2.0f))), mouseWorldCoords)) {
                    TackComponent[] components = pair.Value.GetAllComponents();
                    
                    for (int c = 0; c < components.Length; c++) {
                        if (pair.Value.MouseHoverLastFrame) {
                            components[c].OnMouseOver();
                        } else {
                            components[c].OnMouseEnter();
                        }

                        if (Input.TackInput.MouseButtonDown(Input.MouseButtonKey.Left)) {
                            if (!TackRenderer.GetInstance().GUIInstance.MouseClickDetectedThisFrame) {
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

        public void RunTackObjectStartMethods() {
            foreach (KeyValuePair<string, TackObject> pair in m_tackObjects) {
                for (int c = 0; c < pair.Value.GetAllComponents().Length; c++) {
                    if (pair.Value.GetAllComponents()[c].Active) {
                        pair.Value.GetAllComponents()[c].OnStart();
                    }
                }
            }
        }

        public void RunTackObjectUpdateMethods() {
            // Load a copy of all TackObjects.
            // This allows new TackObjects to be created in the Update loops.
            // A newly created TackObject will not run the TackComponent.Update methods this frame however.
            TackObject[] objects = GetAllTackObjects();

            for (int i = 0; i < objects.Length; i++) {
                for (int c = 0; c < objects[i].GetAllComponents().Length; c++) {
                    if (objects[i].GetAllComponents()[c].Active) {
                        objects[i].GetAllComponents()[c].OnUpdate();
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
    }
}
