using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OsrsColorBot
{
    public class UserConfigOptionsModel
    {
        public string ResourceLocation { get; set; }
        public string TestingFlag { get; set; }

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static UserConfigOptionsModel LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(UserConfigOptionsModel));
            return serializer.Deserialize(stringReader) as UserConfigOptionsModel;
        }
    }
}
