using SP2016.Repository.Mapping;
using System.Collections.Generic;

namespace SP2016.Repository
{
    public class FieldToEntityPropertyMappingComparer : IEqualityComparer<FieldToPropertyMapping>
    {
        public bool Equals(FieldToPropertyMapping x, FieldToPropertyMapping y)
        {
            return x.FieldName == y.FieldName;
        }

        public int GetHashCode(FieldToPropertyMapping obj)
        {
            if (obj == null) return 0;
            return obj.FieldName.GetHashCode() ^ obj.FieldName.GetHashCode();
        }
    }
}
