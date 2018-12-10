using Microsoft.SharePoint;
using SP2016.Repository.Converters;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPBatchMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseEntity
    {
        public SPBatchMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            var batchConverters = new (Type, IConverter)[] {
                (typeof(DateTime), new XmlDateTimeFieldValueConverter()),
                (typeof(SPContentTypeId), new SPContentTypeIdValueConverter()),
                (typeof(SPFieldUserValueCollection), new SPFieldUserValueCollectionConverter()),
                (typeof(string), new BatchStringValueConverter())
            };

            RegisterUniqueConverters(batchConverters);
        }

        public void Map(SPWeb web, SPList list, StringBuilder to, BaseEntity from)
        {
            var fieldMappingsToFieldValues = FieldMappingsToFieldValues(web, list, from);

            foreach (var fieldMappingsToFieldValue in fieldMappingsToFieldValues)
            {
                (FieldToPropertyMapping fieldMapping, object fieldValue) = fieldMappingsToFieldValue;
                to.Append($"<SetVar Name=\"urn:schemas-microsoft-com:office:office#{fieldMapping.FieldName}\">{fieldValue}</SetVar>");
            }
        }
    }
}
