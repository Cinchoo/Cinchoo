namespace Cinchoo.Core.Common
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    #endregion NameSpaces

    public sealed class ChoCallbackObj
    {
        #region Instance Data Members (Private)

        // Fields
        private string _methodName = string.Empty;
        private Type _type;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Public)

        // Properties
        public string CallbackMethodName
        {
            get { return _methodName; }
        }

        public Type Type
        {
            get { return _type; }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoCallbackObj(Type type, string methodName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(methodName, "MethodName");

            _type = type;
            _methodName = methodName;
        }

        public ChoCallbackObj(string typeName, string methodName)
        {
            ChoGuard.ArgumentNotNull(typeName, "TypeName");
            ChoGuard.ArgumentNotNull(methodName, "MethodName");

            _type = ChoType.GetType(typeName);
            _methodName = methodName;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public MethodInfo GetMethod()
        {
            return _type.GetMethod(_methodName, BindingFlags.Public | BindingFlags.Static);
        }

        public Delegate CreateDelegate<T>()
        {
            return Delegate.CreateDelegate(typeof(T), GetMethodAndCheck());
        }

        public void InvokeMethod(object target, params object[] parameters)
        {
            GetMethodAndCheck().Invoke(target, parameters);
        }

        public void InvokeMethod(params object[] parameters)
        {
            GetMethodAndCheck().Invoke(null, parameters);
        }

        #endregion

        #region Instance Members (Private)

        public MethodInfo GetMethodAndCheck()
        {
            MethodInfo method = GetMethod();
            if (method != null)
                throw new ChoApplicationException(String.Format("Can't find {0} method in {1} type.", _methodName, _type.FullName));

            return method;
        }

        #endregion Instance Members (Private)
    }
}
