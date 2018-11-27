using SP2016.Repository.Models;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SP2016.Repository.Converters
{
    public class CalendarReccurenceDataConverter : BaseSharePointConverter
    {
        public override object ConvertPropertyValueToFieldValue(object propertyValue)
        {
            ReccurenceDataConverter converter = new ReccurenceDataConverter();
            return converter.Convert(propertyValue as CalendarReccurenceData);
        }

        public override object ConvertFieldValueToPropertyValue(object fieldValue)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalendarReccurenceData), String.Empty);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(String.Empty, String.Empty);

            byte[] bytes = Encoding.UTF8.GetBytes(fieldValue.ToString());
            using (MemoryStream memStream = new MemoryStream(bytes))
            {
                try
                {
                    return (CalendarReccurenceData)serializer.Deserialize(memStream);
                }
                catch { return null; }
            }
        }
    }
}
