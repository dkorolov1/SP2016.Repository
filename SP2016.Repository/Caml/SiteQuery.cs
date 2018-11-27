using SP2016.Repository.Enums;
using System.Collections.Generic;

namespace SP2016.Repository.Caml
{
    public class SiteQuery
    {
        public Query Query { get; set; }
        public ListsReference ListsReference { get; set; }
        public List<SiteQueryFieldReference> ViewFields { get; set; }
        public SiteQueryWebScope WebScope { get; set; }

        public SiteQuery(Query query, ListsReference listsReference, SiteQueryWebScope webScope, List<SiteQueryFieldReference> viewFields = null)
        {
            this.Query = query;
            this.ListsReference = listsReference;
            this.WebScope = webScope;
            this.ViewFields = viewFields ?? new List<SiteQueryFieldReference>();
        }
    }
}
