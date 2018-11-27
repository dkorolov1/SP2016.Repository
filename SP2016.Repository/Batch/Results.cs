using System;
using System.Xml.Serialization;

namespace SP2016.Repository.Batch
{
    [Serializable]
    public class Results
    {
        [XmlElement]
        public ResultEntry[] Result { get; set; }
    }
}
