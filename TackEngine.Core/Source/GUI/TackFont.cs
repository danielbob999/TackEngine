using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;

using TackEngine.Core.Main;

namespace TackEngine.Core.GUI {
	public class TackFont {
		internal static ITackFontLoadingImpl FontLoadingImplementation;

		public enum Style {
			Regular,
			Bold,
			Italic
		}

		internal struct FontCharacter {
			public Sprite sprite;
			public int texId;
			public Vector2f size;
			public float advance;
			public Vector2f bearing;
		}

		private Face m_fontFace;
        private Dictionary<uint, FontCharacter> m_fontCharacters;

		internal Face FontFace {
			get { return m_fontFace; }
			set { m_fontFace = value; }
		}

        internal Dictionary<uint, FontCharacter> FontCharacters {
            get { return m_fontCharacters; }
            set { m_fontCharacters = value; }
        }

        internal TackFont() {
			m_fontCharacters = new Dictionary<uint, FontCharacter>();
		}

		internal FontCharacter GetFontCharacter(uint charCode) {
			if (!m_fontCharacters.ContainsKey(charCode)) {
				return new FontCharacter() { texId = -1 };
			}

			return m_fontCharacters[charCode];
		}

		public Vector2f MeasureString(string str, float fontSize) {
			Vector2f size = new Vector2f(0, 0);

			float finalFontSize = fontSize / 30.0f;

			for (int i = 0; i < str.Length; i++) {
				FontCharacter ch = GetFontCharacter(str[i]);

				if (ch.texId == -1) {
					continue;
				}

				size.X += ((int)ch.advance >> 6) * finalFontSize;

				if ((ch.size.Y * finalFontSize) > size.Y) {
					size.Y = ch.size.Y * finalFontSize;
                }
			}

			return size;
        }

		public static TackFont LoadFromFile(string path) {
			return FontLoadingImplementation.LoadFromFile(path);
		}
	}
}
