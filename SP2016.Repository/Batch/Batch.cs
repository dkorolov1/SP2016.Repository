using System;
using System.Xml.Serialization;

namespace SP2016.Repository.Batch
{
    [Serializable]
    public class Batch
    {
        [XmlAttribute]
        public BatchErrorHandling OnError { get; set; }
        [XmlElement]
        public Operation[] Method { get; set; }
    }

    [Serializable]
    public enum BatchErrorHandling
    {
        Continue
    }
}
