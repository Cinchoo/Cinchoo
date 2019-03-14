using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Services;

namespace Cinchoo.Core
{
    public class ChoTypeEvent
    {
        public event EventHandler ObjectChanged;

        public void OnObjectChanged(object target, EventArgs e)
        {
            ObjectChanged.Raise(target, e);
        }
    }

    public static class ChoEventManager
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static readonly ChoDictionaryService<Type, ChoTypeEvent> _eventsCache = new ChoDictionaryService<Type, ChoTypeEvent>("ChoCoreFrxConfigurationObjectEvents");

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static ChoTypeEvent GetValue(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            ChoTypeEvent typeEvent = _eventsCache[type];
            if (typeEvent == null)
            {
                lock (_padLock)
                {
                    if (typeEvent == null)
                    {
                        _eventsCache[type] = new ChoTypeEvent();
                    }
                }
            }

            return _eventsCache[type];
        }

        public static ChoTypeEvent GetValue<T>()
        {
            return GetValue(typeof(T));
        }

        #endregion Shared Members (Public)
    }
}
