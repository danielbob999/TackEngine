using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Objects.Components {
    public class SpriteSheetRendererComponent : TackComponent, IRendererComponent {
        /// <summary>
        /// The current Sprite being renderer from the SpriteSheet
        /// </summary>
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// The colour of this component
        /// </summary>
        public Colour4b Colour { get; set; }

        /// <summary>
        /// The shader used on this component
        /// </summary>
        public Shader Shader { get; private set; }

        /// <summary>
        /// The render layer of this component. Smaller values will render first (behind components with a higher value)
        /// </summary>
        public int RenderLayer { get; set; }

        /// <summary>
        /// The uniform values that will be sent to the fragment shader when rendering this component
        /// </summary>
        public Dictionary<string, object> ShaderUniformValues { get; private set; }

        /// <summary>
        /// The SpriteSheet used by this component
        /// </summary>
        public SpriteSheet SpriteSheet { get; set; }

        /// <summary>
        /// The amount of time, in seconds, a single Sprite is shown before switching to the next Sprite
        /// </summary>
        public float RefreshTime { get; set; }

        /// <summary>
        /// Is this SpriteSheetRendererComponent switching between different Sprites
        /// </summary>
        public bool IsRefreshing { get; private set; }

        /// <summary>
        /// The time since the Sprite was refreshed last
        /// </summary>
        public float TimeSinceLastRefresh { get; private set; }

        /// <summary>
        /// The index of the current Sprite in the SpriteSheet
        /// </summary>
        public int CurrentSpriteIndex { get; private set; }

        /// <summary>
        /// Should this component continue looping through the Sprites from the SpriteSheet?
        /// If false, this component will stop refreshing on the last Sprite. Call <see cref="Stop"/> followed by <see cref="Start"/> to start refreshing from the first Sprite again
        /// </summary>
        public bool ShouldLoop { get; set; }

        /// <summary>
        /// If true, this rendering component will ALWAYS be rendered (even if completely off screen)
        /// </summary>
        public bool DisableRenderingBoundsCheck { get; set; }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        public SpriteSheetRendererComponent() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = Colour4b.White;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = null;
            ShouldLoop = true;

            SetShader(TackRenderer.Instance.DefaultLitWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        /// <param name="spriteSheet">The SpriteSheet to render</param>
        public SpriteSheetRendererComponent(SpriteSheet spriteSheet) {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = Colour4b.White;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = spriteSheet;
            ShouldLoop = true;
            RefreshTime = 0.2f;

            SetShader(TackRenderer.Instance.DefaultLitWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        /// <param name="spriteSheet">The SpriteSheet to render</param>
        /// <param name="refreshTime">The time, in seconds, each Sprite is shown</param>
        public SpriteSheetRendererComponent(SpriteSheet spriteSheet, float refreshTime) {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = Colour4b.White;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = spriteSheet;
            ShouldLoop = true;
            RefreshTime = refreshTime;

            SetShader(TackRenderer.Instance.DefaultLitWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        /// <param name="spriteSheet">The SpriteSheet to render</param>
        /// <param name="refreshTime">The time, in seconds, each Sprite is shown</param>
        /// <param name="colour">The colour of this component</param>
        public SpriteSheetRendererComponent(SpriteSheet spriteSheet, float refreshTime, Colour4b colour) {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = spriteSheet;
            ShouldLoop = true;
            RefreshTime = refreshTime;

            SetShader(TackRenderer.Instance.DefaultLitWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        /// <param name="spriteSheet">The SpriteSheet to render</param>
        /// <param name="refreshTime">The time, in seconds, each Sprite is shown</param>
        /// <param name="colour">The colour of this component</param>
        /// <param name="shouldLoop">Should the component be looping the SpriteSheet</param>
        public SpriteSheetRendererComponent(SpriteSheet spriteSheet, float refreshTime, Colour4b colour, bool shouldLoop) {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = spriteSheet;
            ShouldLoop = shouldLoop;
            RefreshTime = refreshTime;

            SetShader(TackRenderer.Instance.DefaultLitWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteSheetRendererComponent
        /// </summary>
        /// <param name="spriteSheet">The SpriteSheet to render</param>
        /// <param name="refreshTime">The time, in seconds, each Sprite is shown</param>
        /// <param name="colour">The colour of this component</param>
        /// <param name="shouldLoop">Should the component be looping the SpriteSheet</param>
        /// <param name="shader">The Shader to use when rendering this component</param>
        public SpriteSheetRendererComponent(SpriteSheet spriteSheet, float refreshTime, Colour4b colour, bool shouldLoop, Shader shader) {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = Sprite.DefaultSprite;
            SpriteSheet = spriteSheet;
            ShouldLoop = shouldLoop;
            RefreshTime = refreshTime;

            SetShader(shader);
        }

        public override void OnStart() {
            base.OnStart();

            if (SpriteSheet != null && SpriteSheet.SpriteCount > 0) {
                Sprite = SpriteSheet.Sprites[0];
            }
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (IsRefreshing) {
                TimeSinceLastRefresh += (float)EngineTimer.Instance.LastUpdateTime;

                if (TimeSinceLastRefresh >= RefreshTime) {
                    if (!ShouldLoop) {
                        if (CurrentSpriteIndex == (SpriteSheet.SpriteCount - 1)) {
                            IsRefreshing = false;
                            return;
                        }
                    }

                    // Switch sprites
                    int nextValidId = FindNextValidSpriteIndex();

                    if (nextValidId != -1) {
                        CurrentSpriteIndex = nextValidId;
                        Sprite = SpriteSheet.Sprites[CurrentSpriteIndex];

                        TimeSinceLastRefresh = 0f;
                    } else {
                        Sprite = Sprite.DefaultSprite;
                    }
                }
            }
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();
        }


        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();
        }

        public void SetUniformVariable(string name, object value) {
            if (ShaderUniformValues.ContainsKey(name)) {
                ShaderUniformValues[name] = value;
            }
        }

        public void SetShader(Shader shader) {
            if (shader == null) {
                return;
            }

            Shader = shader;

            ShaderUniformValues.Clear();

            foreach (string str in shader.UniformVariables) {
                ShaderUniformValues.Add(str, null);
            }
        }

        /// <summary>
        /// Starts refreshing Sprites. Will resume from current point if paused
        /// </summary>
        public void Start() {
            IsRefreshing = true;
        }

        /// <summary>
        /// Will pause the refreshing of Sprites
        /// </summary>
        public void Pause() {
            IsRefreshing = false;
        }

        /// <summary>
        /// Will stop the refreshing of Sprites and will reset the counter/index back to it's starting value
        /// </summary>
        public void Stop() {
            CurrentSpriteIndex = 0;
            TimeSinceLastRefresh = 0f;
            IsRefreshing = false;
        }

        private int FindNextValidSpriteIndex() {
            if (SpriteSheet == null) {
                return -1;
            }

            if (CurrentSpriteIndex + 1 >= SpriteSheet.SpriteCount) {
                return 0;
            }

            return CurrentSpriteIndex + 1;
        }
    }
}
