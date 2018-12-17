using SP2016.Repository.Entities;
using SP2016.Repository.Mapping.SharePoint;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SP2016.Repository.Mapping
{
    public class SPMappersFactory<TEntity> where TEntity : BaseSPEntity
    {
        private readonly IReadOnlyCollection<FieldToPropertyMapping> fieldMappings;

        public SPMappersFactory(IEnumerable<FieldToPropertyMapping> mappings)
        {
            fieldMappings = new ReadOnlyCollection<FieldToPropertyMapping>(mappings.ToArray());
        }

        public SPBatchMapper<TEntity> SPBatchMapper => 
            new SPBatchMapper<TEntity>(fieldMappings);

        public SPListItemMapper<TEntity> SPListItemMapper => 
            new SPListItemMapper<TEntity>(fieldMappings);

        public SPListItemVersionMapper<TEntity> SPListItemVersionMapper => 
            new SPListItemVersionMapper<TEntity>(fieldMappings);

        public SPAfterPropertiesMapper<TEntity> SPAfterPropertiesMapper =>
            new SPAfterPropertiesMapper<TEntity>(fieldMappings);
    }
}
