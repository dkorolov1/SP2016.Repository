using Microsoft.SharePoint;
using System;

namespace SP2016.Repository.Utils
{
    /// <summary>
    /// Disabled item events scope
    /// </summary>
    /// <see cref="http://adrianhenke.wordpress.com/2010/01/29/disable-item-events-firing-during-item-update/"/>
    public class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        bool oldValue;

        public DisabledItemEventsScope()
        {
            oldValue = EventFiringEnabled;
            EventFiringEnabled = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            EventFiringEnabled = oldValue;
        }

        #endregion
    }
}
