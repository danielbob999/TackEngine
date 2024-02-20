using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TackEngine.Android {
    public static class AndroidContext {
        public static Context CurrentContext;
        public static AssetManager CurrentAssetManager;
    }
}