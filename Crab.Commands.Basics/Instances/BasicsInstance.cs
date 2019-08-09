using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Crab
{
    [HasDataFileAttribute("basics")]
    public class BasicInstance : ModuleInstance
    {
        public static string memorytest = "";

        public override async Task startAsync(){
            await Task.CompletedTask;
        }
        public override void shutdown(){}

        public override JObject get_jobject(){
            JObject obj = new JObject();
            obj["memorytest"] = memorytest;
            return obj;
        }

        public override void load_jobject(JObject obj){
            memorytest = (string)obj["memorytest"];
        }
    }
}