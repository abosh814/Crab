using System.Threading.Tasks;
using System.Xml;

namespace Crab
{
    public class BasicInstance : ModuleInstance
    {
        public static string saveTest = "";

        public override async Task startAsync(){
            await Task.CompletedTask;
        }
        public override void shutdown(){}

        public void loadData(XmlReader reader)
        {
            while(reader.Read())
            {
                if(reader.Name == "saveTest")
                    saveTest = reader.Value;
            }
        }

        public void saveData(XmlWriter writer)
        {
            writer.WriteStartElement("test");
            writer.WriteStartAttribute("saveTest");
            writer.WriteValue(saveTest);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
        }
    }
}