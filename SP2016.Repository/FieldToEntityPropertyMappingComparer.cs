using SP2016.Repository.Mapping;
using System.Collections.Generic;

namespace SP2016.Repository
{
    public class FieldToEntityPropertyMappingComparer : IEqualityComparer<FieldToEntityPropertyMapping>
    {
        public bool Equals(FieldToEntityPropertyMapping x, FieldToEntityPropertyMapping y)
        {
            return x.FieldName == y.FieldName;
        }

        public int GetHashCode(FieldToEntityPropertyMapping obj)
        {
            if (obj == null) return 0;
            return obj.FieldName.GetHashCode() ^ obj.FieldName.GetHashCode();
        }
    }
}
