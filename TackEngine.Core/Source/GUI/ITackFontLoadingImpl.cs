using SharpFont;
using System;
using System.Collections.Generic;
using System.Text;

namespace TackEngineLib.GUI {
    internal interface ITackFontLoadingImpl {
        TackFont LoadFromFile(string path);
        TackFont.FontCharacter LoadCharacter(Face face, uint charCode);
    }
}
