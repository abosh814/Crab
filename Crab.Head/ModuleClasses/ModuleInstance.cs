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
        public async Task<ModuleInstanceResult> mainAsync(){
            await startAsync();

            await exitEvent.WaitAsync();

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