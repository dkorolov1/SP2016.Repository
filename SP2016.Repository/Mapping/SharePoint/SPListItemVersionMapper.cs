using Microsoft.SharePoint;
using SP2016.Repository.Converters.SharePoint;
using SP2016.Repository.Entities;
using System;
using System.Collections.Generic;

namespace SP2016.Repository.Mapping.SharePoint
{
    public class SPListItemVersionMapper<TEntity> : SPFieldMapper<TEntity> where TEntity : BaseSPEntity
    {
        public SPListItemVersionMapper(IEnumerable<FieldToPropertyMapping> mappings) 
            : base(mappings)
        {
            RegisterConverters(
                (typeof(DateTime), new ItemVersionDateTimeConverter()));
        }

        public void Map(SPWeb web, object to, SPListItemVersion from)
        {
            var listItemMapper = new SPListItemMapper<TEntity>(FieldMappings);
            listItemMapper.Map(web, to, from.ListItem);
        }
    }
}
