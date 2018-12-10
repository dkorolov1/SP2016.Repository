using Microsoft.SharePoint;
using SP2016.Repository.Models;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace SP2016.Repository.Converters.SharePoint
{
    public class CalendarReccurenceDataConverter : SharePointConverter
    {
        public override object ConvertPropertyValueToFieldValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object propertyValue)
        {
            ReccurenceDataConverter converter = new ReccurenceDataConverter();
            return converter.Convert(propertyValue as CalendarReccurenceData);
        }

        public override object ConvertFieldValueToPropertyValue(SPWeb web, SPField field, PropertyInfo propertyInfo, object fieldValue)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalendarReccurenceData), string.Empty);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

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
