/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.GUI
{
    public class GUIBorder
    {
        private int mLeft;
        private int mRight;
        private int mUp;
        private int mBottom;

        private Colour4b mColour;

        public int Left
        {
            get { return mLeft; }
            set { mLeft = value; }
        }

        public int Right
        {
            get { return mRight; }
            set { mRight = value; }
        }

        public int Up
        {
            get { return mUp; }
            set { mUp = value; }
        }

        public int Bottom
        {
            get { return mBottom; }
            set { mBottom = value; }
        }

        public Colour4b Colour
        {
            get { return mColour; }
            set { mColour = value; }
        }

        public GUIBorder(int _left, int _right, int _up, int _bottom, Colour4b _colour)
        {
            mLeft = _left;
            mRight = _right;
            mUp = _up;
            mBottom = _bottom;
            mColour = _colour;
        }
    }
}
