using Microsoft.SharePoint;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPBatchMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseSPEntity
    {
        public SPBatchMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            var batchConverters = new (Type, FieldConverter)[] {
                (typeof(DateTime), new XmlDateTimeFieldValueConverter()),
                (typeof(string), new BatchStringValueConverter())
            };

            RegisterConverters(batchConverters);
        }

        public void Map(SPWeb web, SPList list, StringBuilder to, BaseSPEntity from)
        {
            var fieldMappingsToFieldValues = FieldMappingsToFieldValues(web, list, from);

            foreach (var fieldMappingsToFieldValue in fieldMappingsToFieldValues)
            {
                (FieldToPropertyMapping fieldMapping, object fieldValue) = fieldMappingsToFieldValue;
                var commandText = $"<SetVar Name=\"urn:schemas-microsoft-com:office:office#{fieldMapping.FieldName}\">{fieldValue}</SetVar>";
                to.Append(commandText);
            }
        }
    }
}
