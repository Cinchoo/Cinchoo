namespace Cinchoo.Core.Configuration
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;
    using System.Diagnostics;
    using Cinchoo.Core.Services;

    #endregion

    public static class ChoSingletonFactoryService
    {
        #region Instance Data Members (Private)

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static object _padLock = new object();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static ChoDictionaryService<Type, Func<object>> _singletonFactoryDictService = new ChoDictionaryService<Type, Func<object>>("SingletonFactoryDictionaryService");
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static ChoDictionaryService<Type, bool> _singletonStateDictService = new ChoDictionaryService<Type, bool>("SingletonStateDictionaryService");
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly static ChoDictionaryService<Type, object> _singletonDictService = new ChoDictionaryService<Type, object>("SingletonDictionaryService");

        #endregion Instance Data Members (Private)

        public static void Register(Type type, Func<object> factory = null)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            lock (_padLock)
            {
                _singletonFactoryDictService.SetValue(type, factory);
            }
        }

        public static object GetInstance(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            if (_singletonStateDictService.GetValue(type))
                return _singletonDictService.GetValue(type);

            lock (_padLock)
            {
                if (_singletonStateDictService.GetValue(type))
                    return _singletonDictService.GetValue(type);

                Func<object> factory = _singletonFactoryDictService[type];
                if (factory == null)
                    _singletonDictService.SetValue(type, ChoActivator.CreateInstance(type));
                else
                    _singletonDictService.SetValue(type, factory());

                _singletonStateDictService.SetValue(type, true);
                return _singletonDictService.GetValue(type);
            }
        }
    }

    [XmlRoot("configurationChangeWatcherSettings")]
    public class ChoConfigurationChangeWatcherSettings : IChoInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("pollDelayInMilliseconds")]
        public int PollDelayInMilliseconds = 100;

        #endregion Instance Data Members (Public)

        static ChoConfigurationChangeWatcherSettings()
        {
            ChoSingletonFactoryService.Register(typeof(ChoConfigurationChangeWatcherSettings), () => ChoCoreFrxConfigurationManager.Register<ChoConfigurationChangeWatcherSettings>());
        }

        #region Object Overrides

        public override string ToString()
        {
            return ChoObject.ToString(this);
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoConfigurationChangeWatcherSettings Me
        {
            get
            {
                return ChoSingletonFactoryService.GetInstance(typeof(ChoConfigurationChangeWatcherSettings)) as ChoConfigurationChangeWatcherSettings;
            }
        }

        #endregion Factory Methods

        public void Initialize()
        {
            if (PollDelayInMilliseconds <= 0)
                PollDelayInMilliseconds = 100;
        }
    }
}
