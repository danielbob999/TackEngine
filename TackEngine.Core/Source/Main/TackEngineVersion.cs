/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Main
{
    public class TackEngineVersion {
        private int mMajor;
        private int mMinor;
        private int mPatch;

        // Properties

        /// <summary>
        /// The major number of this TackEngineVersion. E.g [1].2.3 FullRelease
        /// </summary>
        public int Major {
            get { return mMajor; }
        }

        /// <summary>
        /// The minor number of this TackEngineVersion E.g 1.[2].3 FullRelease
        /// </summary>
        public int Minor {
            get { return mMinor; }
        }

        /// <summary>
        /// The patch number of this TackEngineVersion. E.g 1.2.[3]
        /// </summary>
        public int Patch {
            get { return mPatch; }
        }

        internal TackEngineVersion(int _major, int _minor, int _patch) {
            mMajor = _major;
            mMinor = _minor;
            mPatch = _patch;
        }

        public override string ToString() {
            return string.Format("{0}.{1}.{2}", mMajor, mMinor, mPatch);
        }

    }
}
