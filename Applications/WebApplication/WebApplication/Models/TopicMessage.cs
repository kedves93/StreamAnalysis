using System.Xml.Serialization;

namespace WebApplication.Models
{
    [XmlRoot(ElementName = "TopicMessage")]
    public class TopicMessage
    {
        [XmlElement("Topic")]
        public string Topic { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("Measurement")]
        public string Measurement { get; set; }
    }
}