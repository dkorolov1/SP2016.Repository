using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SP2016.Repository.Constants;
using System;
using System.Diagnostics;
using System.Linq;

namespace SP2016.Repository.Service
{
    public class ListService
    {
        public bool IsLibrary { get; set; }

        public virtual SPList GetByInternalName(SPWeb web, string name)
        {
            Debug.Assert(web != null);
            Debug.Assert(!string.IsNullOrEmpty(name));

            var relativeUrl = SPConstants.LIST.URL.PREFIX + name;

            return GetByRelativeUrl(web, relativeUrl);
        }     

        public virtual SPList GetByRelativeUrl(SPWeb web, string relativeUrl)
        {
            Debug.Assert(web != null);
            Debug.Assert(!string.IsNullOrEmpty(relativeUrl));

            var url = SPUtility.ConcatUrls(web.ServerRelativeUrl, relativeUrl);
            var fullUrl = web.Site.MakeFullUrl(url);

            return web.GetList(fullUrl);
        }

        public virtual SPList Get(SPWeb web, Guid listId)
        {
            Debug.Assert(web != null);
            Debug.Assert(listId != null);

            return web.Lists[listId];
        }

        public virtual bool IsExist(SPWeb web, string relativeUrl)
        {
            Debug.Assert(web != null);
            Debug.Assert(!string.IsNullOrEmpty(relativeUrl));

            bool isExist = false;

            try
            {
                var list = GetByRelativeUrl(web, relativeUrl);
                if (list != null)
                {
                    var folder = GetFolderByRelativeUrl(web, relativeUrl);
                    if (folder != null)
                        isExist = true;
                }
            }
            catch { }

            return isExist;
        }

        public SPFolder GetFolderByRelativeUrl(SPWeb web, string relativeUrl)
        {
            Debug.Assert(web != null);
            Debug.Assert(!string.IsNullOrEmpty(relativeUrl));

            var url = SPUtility.ConcatUrls(web.ServerRelativeUrl, relativeUrl);
            var fullUrl = web.Site.MakeFullUrl(url);

            return web.GetFolder(fullUrl);
        }

        public SPList GetList(SPWeb web, string internalName)
        {
            Debug.Assert(web != null);
            Debug.Assert(!string.IsNullOrEmpty(internalName));

            SPList list = this.IsLibrary
                ? GetByRelativeUrl(web, internalName)
                : GetByInternalName(web, internalName);

            return list;
        }

        private void SetListProperty(SPWeb web, string internalName, Action<SPList> setPropertyFunc)
        {
            var list = GetList(web, internalName);
            setPropertyFunc(list);
            Update(web, list);
        }

        private void Update(SPWeb web, SPList list)
        {
            bool oldAllowUnsafeUpdates = web.AllowUnsafeUpdates;
            web.AllowUnsafeUpdates = true;
            list.Update();
            web.AllowUnsafeUpdates = oldAllowUnsafeUpdates;
        }

        public bool HasEventReceiver(SPList taskList, SPEventReceiverType type, string assembly, string className)
        {
            string assemblyLow = assembly.ToLowerInvariant();
            string classNameLow = className.ToLowerInvariant();

            Func<SPEventReceiverDefinition, bool> predicate = er => er.Assembly.ToLowerInvariant() == assemblyLow
                                                                 && er.Class.ToLowerInvariant()    == classNameLow
                                                                 && er.Type                        == type;

            return taskList.EventReceivers.Cast<SPEventReceiverDefinition>().Any(predicate);
        }
    }
}
