namespace Apixu
{
    public class QueueMessage
    {
        public string Queue { get; set; }

        public string Value { get; set; }

        public string Measurement { get; set; }

        public int TimestampEpoch { get; set; }
    }
}