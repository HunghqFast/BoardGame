using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FastMobile.FXamarin.Core
{
    public class FObjectSerializer
    {
        public static T DeserializeObject<T>(string returningData)
        {
            using (var stringReader = new StringReader(returningData))
            {
                using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(xmlReader);
                }
            }
        }

        public static string SerializeObject<T>(T returningData)
        {
            using (var stringWriter = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stringWriter, returningData);
                return stringWriter.ToString();
            }
        }
    }
}