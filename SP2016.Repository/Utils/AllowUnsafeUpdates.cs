using Microsoft.SharePoint;
using System;

namespace SP2016.Repository.Utils
{
    public class AllowUnsafeUpdates : IDisposable
    {
        private SPWeb _web;
        private bool _oldAllowUnsafeUpdates;

        private void SetAllowUnsafeUpdates(SPWeb web)
        {
            if (!web.AllowUnsafeUpdates)
            {
                web.AllowUnsafeUpdates = true;
                _oldAllowUnsafeUpdates = true;
            }
        }
        
        private void UnSetAllowUnsafeUpdates(SPWeb web)
        {
            if (_oldAllowUnsafeUpdates)
            {
                web.AllowUnsafeUpdates = false;
            }
        }

        public AllowUnsafeUpdates(SPWeb web)
        {
            _web = web;
            SetAllowUnsafeUpdates(web);
        }

        public void Dispose()
        {
            UnSetAllowUnsafeUpdates(_web);
        }
    }
}
