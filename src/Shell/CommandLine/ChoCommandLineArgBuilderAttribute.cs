namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Runtime.Remoting.Proxies;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Runtime.Remoting;
    using Cinchoo.Core.Services;
    using System.Reflection;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoCommandLineArgBuilderAttribute : ChoCommandLineArgObjectAttribute
    {
        #region Instance Data Members (Private)

        private ChoCommandLineArgBuilderDirector _loggableElement;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCommandLineArgBuilderAttribute() : base()
        {
        }

        internal ChoCommandLineArgBuilderAttribute(bool usageAvail = true)
            : base(usageAvail)
        {
        }

        #endregion

        #region Instance Members (Public)

        #region ChoConfigurationElementAttribute Overrides

        public override MarshalByRefObject CreateInstance(Type cmdLineObjType)
        {
            if (_dictService.ContainsKey(cmdLineObjType))
                return (MarshalByRefObject)_dictService.GetValue(cmdLineObjType).GetTransparentProxy();

            lock (_dictService.SyncRoot)
            {
                if (_dictService.ContainsKey(cmdLineObjType))
                    return (MarshalByRefObject)_dictService.GetValue(cmdLineObjType).GetTransparentProxy();
                else
                {
                    RealProxy proxy = new ChoCommandLineArgsBuilderProxy(base.CreateInstance(cmdLineObjType), cmdLineObjType);
                    _dictService.SetValue(cmdLineObjType, proxy);
                    PrintHeader();
                    return (MarshalByRefObject)proxy.GetTransparentProxy();
                }
            }
        }

        #endregion ChoConfigurationElementAttribute Overrides

        internal new ChoCommandLineArgBuilderDirector GetMe(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");

            if (_loggableElement == null)
            {
                lock (SyncRoot)
                {
                    if (_loggableElement == null)
                    {
                        _loggableElement = new ChoCommandLineArgBuilderDirector();
                    }
                }
            }

            return _loggableElement;
        }

        #endregion Instance Members (Public)
    }
}
