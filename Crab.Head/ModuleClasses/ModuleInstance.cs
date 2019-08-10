using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Xml.Serialization;

namespace Crab
{
    public abstract class ModuleInstance //if you want instances for your modules
    {
        public bool shutting_down {get; private set;} //for loops to detect a shutdown
        public async Task<ModuleInstanceResult> mainAsync(){
            shutting_down = false;
            startAsync();

            await exitEvent.WaitAsync();
            shutting_down = true;

            shutdown();

            asyncFinished.Set();
            return exitCode;
        }

        public abstract Task startAsync();

        public abstract void shutdown(); //this should not be async because of obvious reasons

        public abstract JObject get_jobject();

        public abstract void load_jobject(JObject obj);

        private ModuleInstanceResult exitCode = ModuleInstanceResult.NONE; 

        private AsyncManualResetEvent exitEvent = new AsyncManualResetEvent();

        public AsyncManualResetEvent asyncFinished = new AsyncManualResetEvent();
        public void exit(ModuleInstanceResult code){
            exitCode = code;
            exitEvent.Set();
        }
    }
}