/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Renderer;
using TackEngine.Core.Math;

namespace TackEngine.Core.Objects.Components
{
    /// <summary>
    /// The TackComponent that is used to render a TackObject on screen
    /// </summary>
    public class QuadRenderer : TackComponent
    {
        /* VERTEX LAYOUT
         * 
         *    v4 ------ v1
         *    |         |
         *    |         |
         *    |         |
         *    v3 ------ v2
         * 
         */

        //public RectangleShape rectange;
        private Sprite mSprite;
        private SpriteSheet mSpriteSheet;
        private Colour4b mColour;
        private float[] mActualVertexPositions; // [0] = v1, [1] = v2, [2] = v3, [3] = v4
        private int mRenderLayer;

        private RendererMode mRenderMode = RendererMode.Colour;

        // PROPERTIES

        /// <summary>
        /// The Sprite to be rendered on this quad.
        /// </summary>
        public Sprite Sprite
        {
            get { return mSprite; }
            set { mSprite = value; }
        }

        /// <summary>
        /// The SpriteSheet that will be used by this QuadRenderer.
        /// </summary>
        public SpriteSheet SpriteSheet
        {
            get { return mSpriteSheet; }
            set { mSpriteSheet = value; }
        }

        /// <summary>
        /// The colour of this quad
        /// </summary>
        public Colour4b Colour
        {
            get { return mColour; }
            set { mColour = value; }
        }

        /// <summary>
        /// The RenderMode of this QuadRenderer. Use this to specify whether to render a Sprite/SpriteSheet/Colour.
        /// </summary>
        public RendererMode RenderMode
        {
            get { return mRenderMode; }
            set { mRenderMode = value; }
        }

        public int RenderLayer
        {
            get { return mRenderLayer; }
            set { mRenderLayer = value; }
        }

        /// <summary>
        /// Creates a new QuadRenderer component width default values
        /// </summary>
        public QuadRenderer() {
            mActualVertexPositions = new float[4];
        }

        public QuadRenderer(RectangleShape _rect, Colour4b _colour) {
            //rectange = _rect;
            mColour = _colour;

            Sprite = Sprite.DefaultSprite;
            Sprite.Create(false);
        }

        public override void OnStart() {
            base.OnStart();

            if (mRenderMode == RendererMode.SpriteSheet && mSpriteSheet != null)
            {
                //mSpriteSheet.StartTimer();
            }
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (mRenderMode == RendererMode.SpriteSheet)
            {
                //mSpriteSheet.SpriteUpdateCheck();
            }

            //FindVertexPoint(1);
            //FindVertexPoint(2);
        }

        public override void OnRender()  {
            base.OnRender();
        }

        /// <summary>
        /// Calculates the actual vertex point with position and rotation factored in
        /// </summary>
        /// <param name="_vertIndex">The index of the vertex of which to find the position of (1 - 4)</param>
        /// <returns>The position, in Vector2f form, of the vertex</returns>
        public Vector2f FindVertexPoint(int _vertIndex)
        {

            /*
             * Multiplying matrix stuff
             * Formulas from: https://math.stackexchange.com/questions/384186/calculate-new-positon-of-rectangle-corners-based-on-angle
             * 
             * x = x0 + (x - x0) * cos(angle) + (y - y0) * sin(angle)
             * y = y0 - (x - x0) * sin(angle) + (y - y0) * cos(angle)
             * 
             * angle = angle of rotation in degrees
             * x/y = the current point if the vertex
             * x0/y0 = the centre point of the rectangle (the rotation point)
             * 
             */

            TackObject parentObject = GetParent();

            if (_vertIndex < 1 || _vertIndex > 4)
            {
                TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("Cannot calculate the position of the vertex with index : {0}. Index values must be within the range (1-4, inclusive)", _vertIndex));
                return new Vector2f();
            }

            RectangleShape objectShape = new RectangleShape()
            {
                X = ((parentObject.Position.X) - (parentObject.Size.X / 2)),
                Y = ((parentObject.Position.Y ) + (parentObject.Size.Y / 2)),
                Width = (parentObject.Size.X),
                Height = (parentObject.Size.Y)
            };

            if (_vertIndex == 1)
            {
                Vector2f vertPos = new Vector2f(objectShape.X + objectShape.Width, objectShape.Y);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 2)
            {
                Vector2f vertPos = new Vector2f(objectShape.X + objectShape.Width, objectShape.Y - objectShape.Height);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 3)
            {
                Vector2f vertPos = new Vector2f(objectShape.X, objectShape.Y - objectShape.Height);

                float x = parentObject.Position.X + (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation));

                float y = parentObject.Position.Y - (vertPos.X - parentObject.Position.X)
                    * (float)System.Math.Sin(TackMath.DegToRad(parentObject.Rotation)) + (vertPos.Y - parentObject.Position.Y)
                    * (float)System.Math.Cos(TackMath.DegToRad(parentObject.Rotation));

                return new Vector2f(x, y);
            }

            if (_vertIndex == 4)
            {
                Vector2f vertPos = new Vector2f(objectShape.X, objectShape.Y);

                float x = GetParent().Position.X + (vertPos.X - GetParent().Position.X)
                    * (float)System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)) + (vertPos.Y - GetParent().Position.Y)
                    * (float)System.Math.Sin(TackMath.DegToRad(GetParent().Rotation));

                float y = GetParent().Position.Y - (vertPos.X - GetParent().Position.X)
                    * (float)System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)) + (vertPos.Y - GetParent().Position.Y)
                    * (float)System.Math.Cos(TackMath.DegToRad(GetParent().Rotation));

                return new Vector2f(x, y);
            }

            return new Vector2f(-1, -1);
        }
    }

    public enum RendererMode
    {
        Colour,
        Sprite,
        SpriteSheet
    }
}
