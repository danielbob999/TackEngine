/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Engine;

namespace TackEngineLib.Main
{
    internal class TackCommand
    {
        private string mCommandCallString;
        private EngineDelegates.CommandDelegate mCommandDelegate;
        private List<string> mCommandArgList = new List<string>();

        /// <summary>
        /// The string used to call the command
        /// </summary>
        public string CommandCallString
        {
            get { return mCommandCallString; }
        }

        /// <summary>
        /// The delegate called when this command is run
        /// </summary>
        public EngineDelegates.CommandDelegate CommandDelegate
        {
            get { return mCommandDelegate; }
        }

        /// <summary>
        /// The list of arg combinations used with this command. If arg option is "", no argument is required
        /// </summary>
        public List<string> CommandArgList
        {
            get { return mCommandArgList; }
        }

        public TackCommand(string a_callName, EngineDelegates.CommandDelegate a_delegate, List<string> a_argList)
        {
            mCommandCallString = a_callName;
            mCommandDelegate = a_delegate;
            mCommandArgList = a_argList;
        }
    }
}
