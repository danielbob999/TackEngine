/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TackEngine.Core.Resources
{
    /// <summary>
    /// Main class used to load resources into TackEngine
    /// </summary>
    public class TackResources
    {
        private Dictionary<string, byte[]> mResourceDictionary = new Dictionary<string, byte[]>();

        internal TackResources(string aFileName) {
            FileStream fs = File.OpenRead(aFileName);

            using (BinaryReader reader = new BinaryReader(fs)) {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++) {
                    int nameLength = (int)reader.ReadChar();
                    char[] name = reader.ReadChars(nameLength);
                    int byteAmount = reader.ReadInt32();
                    byte[] data = reader.ReadBytes(byteAmount);
                }
            }
        }

        /// <summary>
        /// Returns a resource from the loaded resource file
        /// </summary>
        /// <typeparam name="T">The type of the resource</typeparam>
        /// <param name="aFileName">The filename of the resource</param>
        /// <param name="aOutVar">The variable the resource will be loaded into</param>
        /// <returns></returns>
        public static bool GetResource<T>(string aFileName, out T aOutVar) {
            aOutVar = default;
            return false;
        }
    }

    [Serializable]
    internal struct ResourceObject
    {
        const string VERSION = "V1.1";

        public ResourceObject(string aName, byte[] aData) {
            mName = aName;
            mSize = aData.Length;
            mData = aData;
        }

        public string mName;
        public int mSize;
        public byte[] mData;
    }
}
