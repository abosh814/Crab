using System.Threading.Tasks;
using System.Xml;

namespace Crab
{
    [HasDataFileAttribute("basics")]
    public class BasicInstance : ModuleInstance
    {
        public static string saveTest = "";

        public override async Task startAsync(){
            await Task.CompletedTask;
        }
        public override void shutdown(){}

        public override void loadData(XmlDocument doc, XmlNode root)
        {
            saveTest = doc.SelectSingleNode("/root/test")?.Attributes.GetNamedItem("value")?.Value;
        }

        public override void saveData(ref XmlDocument doc, ref XmlNode root)
        {
            XmlNode node = doc.CreateElement("test");
            XmlAttribute att = doc.CreateAttribute("value");
            att.Value = saveTest;
            node.Attributes.Append(att);
            root.AppendChild(node);
        }
    }
}