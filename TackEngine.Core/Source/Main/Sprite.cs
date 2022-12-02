/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using TackEngine.Core.Engine;

namespace TackEngine.Core.Main {
    /// <summary>
    /// 
    /// </summary>
    public class Sprite {
        public enum SpriteFilter {
            Linear = 0x2601,
            Nearest = 0x2600
        }


        public enum SpriteWrapMode {
            ClampToEdge = 0x812F,
            Clamp = 0x2900,
            ClampToBorder = 0x812D,
            Repeat = 0x2901
        }

        public enum SpritePixelFormat {
            //
            // Summary:
            //     The pixel data contains color-indexed values, which means the values are an index
            //     to colors in the system color table, as opposed to individual color values.
            Indexed = 0x10000,
            //
            // Summary:
            //     The pixel data contains GDI colors.
            Gdi = 0x20000,
            //
            // Summary:
            //     The pixel data contains alpha values that are not premultiplied.
            Alpha = 0x40000,
            //
            // Summary:
            //     The pixel format contains premultiplied alpha values.
            PAlpha = 0x80000,
            //
            // Summary:
            //     Reserved.
            Extended = 0x100000,
            //
            // Summary:
            //     The default pixel format of 32 bits per pixel. The format specifies 24-bit color
            //     depth and an 8-bit alpha channel.
            Canonical = 0x200000,
            //
            // Summary:
            //     The pixel format is undefined.
            Undefined = 0,
            //
            // Summary:
            //     No pixel format is specified.
            DontCare = 0,
            //
            // Summary:
            //     Specifies that the pixel format is 1 bit per pixel and that it uses indexed color.
            //     The color table therefore has two colors in it.
            Format1bppIndexed = 196865,
            //
            // Summary:
            //     Specifies that the format is 4 bits per pixel, indexed.
            Format4bppIndexed = 197634,
            //
            // Summary:
            //     Specifies that the format is 8 bits per pixel, indexed. The color table therefore
            //     has 256 colors in it.
            Format8bppIndexed = 198659,
            //
            // Summary:
            //     The pixel format is 16 bits per pixel. The color information specifies 65536
            //     shades of gray.
            Format16bppGrayScale = 1052676,
            //
            // Summary:
            //     Specifies that the format is 16 bits per pixel; 5 bits each are used for the
            //     red, green, and blue components. The remaining bit is not used.
            Format16bppRgb555 = 135173,
            //
            // Summary:
            //     Specifies that the format is 16 bits per pixel; 5 bits are used for the red component,
            //     6 bits are used for the green component, and 5 bits are used for the blue component.
            Format16bppRgb565 = 135174,
            //
            // Summary:
            //     The pixel format is 16 bits per pixel. The color information specifies 32,768
            //     shades of color, of which 5 bits are red, 5 bits are green, 5 bits are blue,
            //     and 1 bit is alpha.
            Format16bppArgb1555 = 397319,
            //
            // Summary:
            //     Specifies that the format is 24 bits per pixel; 8 bits each are used for the
            //     red, green, and blue components.
            Format24bppRgb = 137224,
            //
            // Summary:
            //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
            //     red, green, and blue components. The remaining 8 bits are not used.
            Format32bppRgb = 139273,
            //
            // Summary:
            //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
            //     alpha, red, green, and blue components.
            Format32bppArgb = 2498570,
            //
            // Summary:
            //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
            //     alpha, red, green, and blue components. The red, green, and blue components are
            //     premultiplied, according to the alpha component.
            Format32bppPArgb = 925707,
            //
            // Summary:
            //     Specifies that the format is 48 bits per pixel; 16 bits each are used for the
            //     red, green, and blue components.
            Format48bppRgb = 1060876,
            //
            // Summary:
            //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
            //     alpha, red, green, and blue components.
            Format64bppArgb = 3424269,
            //
            // Summary:
            //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
            //     alpha, red, green, and blue components. The red, green, and blue components are
            //     premultiplied according to the alpha component.
            Format64bppPArgb = 1851406,
            //
            // Summary:
            //     The maximum value for this enumeration.
            Max = 0xF
        }

        public class SliceData {
            public float BorderSize { get; set; }

            public SliceData(float border = 20) {
                BorderSize = border;
            }
        }

        internal static bool mLogMessageOverride = false;

        internal static bool LogMessageOverride {
            get { return mLogMessageOverride; }
            set { mLogMessageOverride = value; }
        }

        /// <summary>
        /// The default sprite.
        /// </summary>
        public static Sprite DefaultSprite { get; internal set; }

        /// <summary>
        /// The default UI sprite
        /// </summary>
        public static Sprite DefaultUISprite { get; internal set; }

        /// <summary>
        /// The width of the Sprite
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// The height of the Sprite
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// The auto-generated texture ID for this Sprite
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The image filter used when rendering
        /// </summary>
        public SpriteFilter Filter { get; set; }

        /// <summary>
        /// The image wrap mode to use when rendering
        /// </summary>
        public SpriteWrapMode WrapMode { get; set; }

        /// <summary>
        /// The PixelFormat of the data for this Sprite
        /// </summary>
        public SpritePixelFormat PixelFormat { get; internal set; }

        /// <summary>
        /// Is this sprite using 9 slicing for displaying in the UI?
        /// </summary>
        public bool IsNineSliced { get; set; }

        /// <summary>
        /// The nine sliced data for this Sprite
        /// </summary>
        public SliceData NineSlicedData { get; set; }

        /// <summary>
        /// The RGBA data of this Sprite
        /// </summary>
        public byte[] Data { get; internal set; }
        //internal int m_stride;

        internal Sprite(int w = 0, int h = 0) {
            Width = w;
            Height = h;

            Filter = SpriteFilter.Linear;
            WrapMode = SpriteWrapMode.ClampToEdge;

            IsNineSliced = false;
            NineSlicedData = null;
        }

        /// <summary>
        /// Loads the sprite into memory.
        /// </summary>
        /// <param name="_logMsgs">True if messages should be logged to console, false otherwise</param>
        public void Create(bool _logMsgs = true) {
            SpriteManager.Instance.RegisterSprite(this);
        }

        public void Destroy(bool _logMsgs = true) {
            SpriteManager.Instance.DeleteSprite(this, _logMsgs);
        }

        public object GetBitmapCopy() {
            /*
            Bitmap bmpCopy = new Bitmap(Width, Height, PixelFormat);
            BitmapData bmpCopyData = bmpCopy.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, PixelFormat);
            bmpCopyData.Stride = m_stride;

            // Copy m_data back to the bitmap Scan0 
            System.Runtime.InteropServices.Marshal.Copy(Data, 0, bmpCopyData.Scan0, Data.Length);

            bmpCopy.UnlockBits(bmpCopyData);

            return bmpCopy;
            */

            return null;
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            return (Id == ((Sprite)obj).Id);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static Sprite LoadFromFile(string path) {
            return SpriteManager.Instance.LoadFromFile(path);
        }

        public static Sprite LoadFromFile(byte[] data) {
            Sprite newSprite = new Sprite();


            return newSprite;
        }

        /*
        public static Sprite LoadFromBitmap(Bitmap original) {
            Sprite newSprite = new Sprite();

            newSprite.Width = original.Width;
            newSprite.Height = original.Height;

            BitmapData bmpData = original.LockBits(new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            newSprite.PixelFormat = bmpData.PixelFormat;
            newSprite.m_stride = bmpData.Stride;
            newSprite.Data = new byte[Math.TackMath.Abs(bmpData.Stride) * original.Height];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, newSprite.Data, 0, newSprite.Data.Length);

            original.UnlockBits(bmpData);

            return newSprite;
        }
        */
    }
}
