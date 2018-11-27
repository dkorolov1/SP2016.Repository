using System;
using System.Xml.Serialization;

namespace SP2016.Repository.Batch
{
    [Serializable]
    public class ResultEntry
    {
        [XmlIgnore]
        public int? MethodID { get; set; }

        [XmlAttribute("ID")]
        public string MethodIDStr
        {
            get { return MethodID.HasValue ? MethodID.ToString() : null; }
            set { MethodID = !string.IsNullOrEmpty(value) ? int.Parse(value) : default(int?); }
        }

        [XmlAttribute]
        public int Code { get; set; }
        [XmlElement("ID")]
        public int? ItemID { get; set; }
        [XmlElement]
        public string ErrorText { get; set; }
    }
}
