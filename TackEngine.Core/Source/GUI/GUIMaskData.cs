using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;

namespace TackEngineLib.GUI {
    internal class GUIMaskData {
        public static readonly int MAX_MASK_COUNT = 10;

        public List<RectangleShape> Masks { get; private set; }

        public GUIMaskData(List<RectangleShape> masks) {
            Masks = new List<RectangleShape>();
            Masks.AddRange(masks);
        }

        public void AddMask(RectangleShape rect) {
            if (Masks == null) {
                Masks = new List<RectangleShape>();
            }

            if (Masks.Count >= MAX_MASK_COUNT) {
                return;
            }

            Masks.Add(rect);
        }
    }
}
