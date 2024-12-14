/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Main
{
    public class CommandMethod : Attribute
    {
        private string mCommandCallName;
        private List<string> mArgList;

        public CommandMethod(string a_cmdName, string a_defaultArg, params string[] a_argList)
        {
            mCommandCallName = a_cmdName;
            mArgList = new List<string>();
            mArgList.Add(a_defaultArg);
            mArgList.AddRange(a_argList);
        }

        public string GetCallString()
        {
            return mCommandCallName;
        }

        public string[] GetArgList()
        {
            return mArgList.ToArray();
        }
    }
}
