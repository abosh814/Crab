using System.Threading.Tasks;
using System.Xml;
using System;

namespace Crab
{
    public class BasicInstance : ModuleInstance
    {
        public static string saveTest = "";

        public override async Task startAsync(){
            await Task.CompletedTask;
        }
        public override void shutdown(){}

        public new void loadData(XmlReader reader)
        {
            Console.WriteLine($"checking{reader.Name}");
            while(reader.Read())
            {
                Console.WriteLine($"checking{reader.Name}");
                if(reader.Name == "saveTest")
                    saveTest = reader.Value;
            }
        }

        public new void saveData(XmlWriter writer)
        {
            writer.WriteStartElement("test");
            writer.WriteStartAttribute("saveTest");
            writer.WriteValue(saveTest);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
        }
    }
}