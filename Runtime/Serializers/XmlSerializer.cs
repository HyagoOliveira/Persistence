using System.IO;
using SystemSerialization = System.Xml.Serialization;

namespace ActionCode.Persistence
{
    /// <summary>
    /// XML Serializer classes must have an empty Constructor.
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        public string Extension => "xml";

        public string SerializePretty<T>(T data) => Serialize(data);

        public string Serialize<T>(T data)
        {
            var serializer = new SystemSerialization.XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            serializer.Serialize(writer, data);
            return writer.ToString();
        }

        public T Deserialize<T>(string value)
        {
            var serializer = new SystemSerialization.XmlSerializer(typeof(T));
            using TextReader reader = new StringReader(value);
            return (T)serializer.Deserialize(reader);
        }
    }
}
