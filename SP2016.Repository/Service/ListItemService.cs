using Microsoft.SharePoint;
using System;
using System.Diagnostics;

namespace SP2016.Repository.Service
{
    public class ListItemService
    {
        public virtual void Delete(SPListItem item)
        {
            Debug.Assert(item != null);
            var listName = item.ParentList.Title;
            var webUrl = item.Web.Url;
            try
            {
                item.Web.AllowUnsafeUpdates = true;
                item.Delete();
            }
            catch (Exception ex)
            {
                var message = $"Ошибка при удалении элемента списка {item.ParentList.Title} узла {item.Web.Url} с ID={item.ID}";
                InvalidOperationException exception = new InvalidOperationException(message, ex);
                throw exception;
            }
        }

        public virtual void Delete(SPList list, int itemId)
        {
            Delete(list.GetItemById(itemId));
        }
    }
}
