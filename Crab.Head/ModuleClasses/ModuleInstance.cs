using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using System.Xml;
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

        //https://stackoverflow.com/questions/4094940/c-sharp-create-simple-xml-file
        public abstract void saveData(ref XmlDocument doc, ref XmlNode root);

        //https://stackoverflow.com/questions/642293/how-do-i-read-and-parse-an-xml-file-in-c
        public abstract void loadData(XmlDocument doc, XmlNode root);

        private ModuleInstanceResult exitCode = ModuleInstanceResult.NONE; 

        private AsyncManualResetEvent exitEvent = new AsyncManualResetEvent();

        public AsyncManualResetEvent asyncFinished = new AsyncManualResetEvent();
        public void exit(ModuleInstanceResult code){
            exitCode = code;
            exitEvent.Set();
        }
    }
}