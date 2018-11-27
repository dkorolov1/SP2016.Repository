using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace SP2016.Repository.Caml
{
    public class ListsReference
    {
        public SPListTemplateType? ListsServerTemplate;
        public string ListsBaseType;
        public int MaxListsLimit;
        public bool SearchInHiddenLists;
        public List<Guid> ListsIds;

        public ListsReference()
        {
            ListsIds = new List<Guid>();
        }
    }
}
