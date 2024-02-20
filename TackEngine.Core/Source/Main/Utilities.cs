using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Main {
    public static class Utilities {
        public static object FirstNotNull(params object[] objects) {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i] != null) {
                    return objects[i];
                }
            }

            return null;
        }
    }
}
