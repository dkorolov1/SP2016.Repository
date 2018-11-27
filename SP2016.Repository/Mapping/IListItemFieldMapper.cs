using System.Collections.Generic;

namespace SP2016.Repository.Mapping
{
    public interface IListItemFieldMapper
    {
        IReadOnlyCollection<FieldToEntityPropertyMapping> Mappings { get; }
    }
}
