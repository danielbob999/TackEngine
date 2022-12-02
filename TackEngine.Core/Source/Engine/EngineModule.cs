using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.Engine
{
    /// <summary>
    /// The base class of every module used in TackEngine
    /// </summary>
    public class EngineModule
    {
        public enum ModuleStatus {
            None,               // The state when the module constructor has been called by the Start method hasn't been called
            Starting,           // The state when the module is starting
            Running,            // The state when the module is running correctly
            Stopping,           // The state when the module is stopping
            Shutdown,           // The state after the module has been shutdown. 
                                //      - If EngineModule.Status is equal to this state, the Update/Render/Close methods shouldn't be run
            NotResponding,      // The state if the module has stopped responding
            RunningWithWarning  // The state if the module is finishing the Update method but is encountering errors
        }

        private ModuleStatus m_status;

        public ModuleStatus Status
        {
            get { return m_status; }
        }

        internal virtual void Start() {
        }

        internal virtual void Update() {
            // If the m_status is ModuleStatus.Starting, set it to Running. This will only get called on the very first Update call
            if (m_status == ModuleStatus.Starting) {
                m_status = ModuleStatus.Running;
            }
        }

        internal virtual void Render() {

        }

        internal virtual void Close() {
        }
    }
}
