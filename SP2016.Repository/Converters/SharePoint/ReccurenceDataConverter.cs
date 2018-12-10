using SP2016.Repository.Models;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SP2016.Repository.Converters.SharePoint
{
    public class ReccurenceDataConverter
    {
        public string Convert(CalendarReccurenceData reccurenceData) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalendarReccurenceData), "");
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (MemoryStream memStream = new MemoryStream())
            {
                XmlWriter writer = XmlWriter.Create(memStream, new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true
                });

                serializer.Serialize(writer, reccurenceData, ns);

                writer.Flush();
                memStream.Position = 0;

                using (StreamReader reader = new StreamReader(memStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
