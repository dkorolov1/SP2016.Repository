using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SP2016.Repository.Batch
{
    [Serializable]
    [DebuggerDisplay("Scope = {Scope}, ListId = {ListId}")]
    public class SetList
    {
        [XmlAttribute]
        public SetListScope Scope { get; set; }
        [XmlText]
        public string ListId { get; set; }
    }
    [Serializable]
    public enum SetListScope { Request }
}
