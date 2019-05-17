using System.Xml.Serialization;

namespace WebApplication.Models
{
    [XmlRoot(ElementName = "QueueMessage")]
    public class QueueMessage
    {
        [XmlElement("Queue")]
        public string Queue { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        [XmlElement("Measurement")]
        public string Measurement { get; set; }

        [XmlElement("TimestampEpoch")]
        public long TimestampEpoch { get; set; }
    }
}