/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace TackEngine.Core.Physics {
    public class TackPhysics : EngineModule {
        public static readonly Colour4b BoundsColour = new Colour4b(150, 255, 150, 255); // Light pastey green
        public static readonly Colour4b WheelColour = new Colour4b(255, 175, 0, 255); // Orange
        public static readonly Colour4b JointColour = new Colour4b(255, 0, 255, 255); // Pink
        public static readonly Colour4b AnchorColour = new Colour4b(0, 0, 0, 255); // Black

        public static TackPhysics Instance = null;

        private Vector2f m_gravityForce;
        private List<BasePhysicsComponent> m_physicBodyComponents;
        private bool m_runBroadphaseAlgorithm;
        private bool m_debugDrawBodies;
        private World m_physicsWorld;
        private List<Body> m_bodiesToBeDeleted;
        private float m_timeToSimulate;
        private SolverIterations m_solverIterations;

        public Vector2f Gravity {
            get { return m_gravityForce; }
            set { 
                m_gravityForce = value;
                m_physicsWorld.Gravity = new Vector2(value.X, value.Y);
            }
        }

        /// <summary>
        /// Gets/Sets whether the render should draw the AABBs on screen
        /// </summary>
        public bool ShouldDebugDrawBodies {
            get { return m_debugDrawBodies; }
            set { m_debugDrawBodies = value; }
        }

        /// <summary>
        /// Gets/Sets whether to run the broadphase algorithm to determine if bodies are potentially colliding
        /// </summary>
        public bool RunBroadphaseAlgorithm {
            get { return m_runBroadphaseAlgorithm; }
            set { m_runBroadphaseAlgorithm = value; }
        }

        internal TackPhysics(int targetSimulationRate) {
            m_gravityForce = new Vector2f(0, -9.8f);
            m_physicBodyComponents = new List<BasePhysicsComponent>();
            m_runBroadphaseAlgorithm = true;
            m_timeToSimulate = 1f / (float)targetSimulationRate;
            m_solverIterations = new SolverIterations();

            if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Windows ||
                TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Linux ||
                TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.MacOS
            ) {
                m_solverIterations.PositionIterations = 10;
                m_solverIterations.VelocityIterations = 10;
            } else {
                m_solverIterations.PositionIterations = 5;
                m_solverIterations.VelocityIterations = 5;
            }

            Instance = this;
        }

        /// <summary>
        /// Starts this TackPhysics instance
        /// </summary>
        internal override void Start() {
            double startTime = EngineTimer.Instance.TotalRunTime;

            m_debugDrawBodies = false;
            m_physicsWorld = new World(new Vector2(m_gravityForce.X, m_gravityForce.Y));
            m_bodiesToBeDeleted = new List<Body>();

            TackConsole.EngineLog(TackConsole.LogType.Message, "TackPhysics started in " + (EngineTimer.Instance.TotalRunTime - startTime).ToString("0.000") + " seconds");
        }

        internal override void Update() {
            base.Update();

            TackProfiler.Instance.StartTimer("TackPhysics.OnUpdate");

            TackProfiler.Instance.StartTimer("TackPhysics.OnUpdate.RemoveBodies");

            if (m_bodiesToBeDeleted.Count > 0) {
                if (!m_physicsWorld.IsLocked) {
                    int count = 0;
                    foreach (Body body in m_bodiesToBeDeleted) {
                        m_physicsWorld.Remove(body);
                        count++;
                    }

                    m_bodiesToBeDeleted.Clear();
                }
            }

            DebugDraw();

            TackProfiler.Instance.StopTimer("TackPhysics.OnUpdate.RemoveBodies");

            TackProfiler.Instance.StartTimer("TackPhysics.OnUpdate.WorldStep");

            m_physicsWorld.Step(m_timeToSimulate, ref m_solverIterations);

            TackProfiler.Instance.StopTimer("TackPhysics.OnUpdate.WorldStep");
        }

        internal override void Render() {
            base.Render();
        }

        /// <summary>
        /// Closes this TackPhysics instance
        /// </summary>
        internal override void Close() {
        }

        public static TackPhysics GetInstance() {
            return Instance;
        }

        internal World GetWorld() {
            return m_physicsWorld;
        }

        internal void RegisterPhysicsComponent(BasePhysicsComponent component) {
            if (!m_physicBodyComponents.Contains(component)) {
                m_physicBodyComponents.Add(component);
            }
        }

        internal void DeregisterPhysicsComponent(BasePhysicsComponent component) {
            if (component == null) {
                return;
            }

            if (component.PhysicsBody == null) {
                return;
            }

            // Disable collisions and detection of the physics body
            component.PhysicsBody.IgnoreCCD = true;
            component.PhysicsBody.Awake = false;
            m_bodiesToBeDeleted.Add(component.PhysicsBody);

            m_physicBodyComponents.Remove(component);
        }

        private bool CheckAABBToAABBCollision(AABB aabb1, AABB aabb2, out PhysicsCollision collision) {
            AABB comp1AABB = aabb1;
            AABB comp2AABB = aabb2;
            //TackObject obj1 = comp1.GetParent();
            //TackObject obj2 = comp2.GetParent();

            bool isCollidingXAxis = false;
            bool isCollidingYAxis = false;
            Vector2f penetration = new Vector2f(0, 0);
            Vector2f normal = new Vector2f();

            if (comp1AABB.Origin.X < comp2AABB.Origin.X) {
                if (comp1AABB.Right > comp2AABB.Left) {
                    isCollidingXAxis = true;
                    penetration.X = Math.TackMath.Abs(comp1AABB.Right - comp2AABB.Left);
                    normal.X = 1;
                }
            } else {
                if (comp1AABB.Left < comp2AABB.Right) {
                    isCollidingXAxis = true;
                    penetration.X = Math.TackMath.Abs(comp2AABB.Right - comp1AABB.Left);
                    normal.X = -1;
                }
            }


            if (comp1AABB.Origin.Y > comp2AABB.Origin.Y) {
                if (comp1AABB.Bottom < comp2AABB.Top) {
                    isCollidingYAxis = true;
                    penetration.Y = Math.TackMath.Abs(comp1AABB.Bottom - comp2AABB.Top);
                    normal.Y = -1;
                }
            } else {
                if (comp1AABB.Top > comp2AABB.Bottom) {
                    isCollidingYAxis = true;
                    penetration.Y = Math.TackMath.Abs(comp1AABB.Top - comp2AABB.Bottom);
                    normal.Y = 1;
                }
            }

            if (isCollidingXAxis && isCollidingYAxis) {
                Vector2f distances = new Vector2f(Math.TackMath.Abs(comp1AABB.Origin.X - comp2AABB.Origin.X), Math.TackMath.Abs(comp1AABB.Origin.Y - comp2AABB.Origin.Y));

                if (distances.X >= distances.Y) {
                    normal.Y = 0;
                    collision = new PhysicsCollision(null, null, normal, penetration.X);
                    return true;
                } else {
                    normal.X = 0;
                    collision = new PhysicsCollision(null, null, normal, penetration.Y);
                    return true;
                }
            }

            collision = null;
            return false;
        }

        public void Shutdown() {
            Instance.Close();
        }

        private void DebugDraw() {
            if (!m_debugDrawBodies) {
                return;
            }

            TackObject[] physObjects = TackObject.Get();

            for (int i = 0; i < physObjects.Length; i++) {
                BasePhysicsComponent comp = physObjects[i].GetComponent<BasePhysicsComponent>();

                if (comp != null) {
                    comp.OnDebugDraw();
                }
            }
        }
    }
}
