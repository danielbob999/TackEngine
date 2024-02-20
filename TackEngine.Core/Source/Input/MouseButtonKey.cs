/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Input
{
    public enum MouseButtonKey
    {
        //
        // Summary:
        //     The left mouse button.
        Left,
        //
        // Summary:
        //     The middle mouse button.
        Middle,
        //
        // Summary:
        //     The right mouse button.
        Right,
        //
        // Summary:
        //     The first extra mouse button.
        Button1,
        //
        // Summary:
        //     The second extra mouse button.
        Button2,
        //
        // Summary:
        //     The third extra mouse button.
        Button3,
        //
        // Summary:
        //     The fourth extra mouse button.
        Button4,
        //
        // Summary:
        //     The fifth extra mouse button.
        Button5,
        //
        // Summary:
        //     The sixth extra mouse button.
        Button6,
        //
        // Summary:
        //     The seventh extra mouse button.
        Button7,
        //
        // Summary:
        //     The eigth extra mouse button.
        Button8,
        //
        // Summary:
        //     The ninth extra mouse button.
        Button9,
        //
        // Summary:
        //     Indicates the last available mouse button.
        LastButton
    }

    public enum MouseButtonAction {
        Down,
        Up,
        Held
    }
}
