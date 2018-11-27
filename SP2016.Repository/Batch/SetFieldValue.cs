using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SP2016.Repository.Batch
{
    [Serializable]
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public class SetFieldValue
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlText]
        public string Value { get; set; }

        public SetFieldValue() { }

        public SetFieldValue(string name, object value)
        {
            Name = "urn:schemas-microsoft-com:office:office#" + name;
            Value = value != null ? value.ToString() : null;
        }

        public static SetFieldValue ForID(int id)
        {
            return new SetFieldValue { Name = "ID", Value = (id == 0 ? "New" : id.ToString()) };
        }

        public static SetFieldValue ForCmd(Command command)
        {
            return new SetFieldValue { Name = "Cmd", Value = command.ToString() };
        }

        public static SetFieldValue ForFolder(SPList list, string folderPath)
        {
            string folderServerRelativeUrl = SPUtility.ConcatUrls(list.RootFolder.ServerRelativeUrl, folderPath);
            return new SetFieldValue { Name = "RootFolder", Value = folderServerRelativeUrl };
        }
    }

    [Serializable]
    public enum Command { Save, Delete }
}
