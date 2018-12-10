using Microsoft.SharePoint;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPListItemVersionMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseEntity
    {
        public SPListItemVersionMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            var itemVersionDateTimeConverter = (typeof(DateTime), new ItemVersionDateTimeConverter());
            RegisterUniqueConverters(itemVersionDateTimeConverter);
        }

        public void Map(SPWeb web, object to, SPListItemVersion from)
        {
            var listItemMapper = new SPListItemMapper<TEntity>(FieldMappings);
            listItemMapper.Map(web, to, from.ListItem);
        }
    }
}
