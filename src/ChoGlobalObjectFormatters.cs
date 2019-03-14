namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    public class ChoObjectFormatHandlerAttribute : Attribute
    {
        internal const string DEFAULT_HANDLER_NAME = "#DEFAULT_HANDLER#";

        private readonly Type _targetType;
        public Type TargetType
        {
            get { return _targetType; }
        }

        public string FormatName
        {
            get;
            set;
        }

        public ChoObjectFormatHandlerAttribute(Type targetType)
        {
            _targetType = targetType;
        }
    }

    public static class ChoGlobalObjectFormatters
    {
        #region Instance Data Members (Private)

        [ChoHiddenMember]
        private static readonly Dictionary<Type, Dictionary<string, Func<object, string>>> _objectFormatHandlers = new Dictionary<Type, Dictionary<string, Func<object, string>>>();
        private static readonly object _padLock = new object();

        #endregion Instance Data Members (Private)

        #region Constructors

        [ChoHiddenMember]
        static ChoGlobalObjectFormatters()
        {
            _objectFormatHandlers.Add(typeof(Object), new Dictionary<string, Func<object, string>>());
            _objectFormatHandlers[typeof(Object)].Add(ChoFormattableObject.DefaultFormatName, ChoFormattableObject.ToString);
            _objectFormatHandlers[typeof(Object)].Add(ChoFormattableObject.ExtendedFormatName, ChoFormattableObject.ToExtendedString);
        }

        #endregion Constructors

        #region Shared Members (Public)

        public static void Register<T>(T target)
        {
            if (target == null)
                return;

            Register(target.GetType());
        }
        
        public static void Register<T>()
        {
            Register(typeof(T));
        }

        public static void Register(Type type)
        {
            if (type == null)
                return;

            foreach (MethodInfo methodInfo in type.GetMethods())
            {
                ChoObjectFormatHandlerAttribute objectFormatterAttribute = methodInfo.GetCustomAttribute<ChoObjectFormatHandlerAttribute>();
                if (objectFormatterAttribute == null)
                    continue;

                try
                {
                    AddOrReplace(objectFormatterAttribute.TargetType,
                        objectFormatterAttribute.FormatName.IsNullOrEmpty() ? ChoObjectFormatHandlerAttribute.DEFAULT_HANDLER_NAME : objectFormatterAttribute.FormatName,
                        methodInfo.CreateDelegate<Func<object, string>>());
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ChoTrace.Error(ex);
                }
            }
        }

        public static Func<object, string> GetObjectFormatHandler(string formatName)
        {
            return GetObjectFormatHandler(null, formatName);
        }

        public static Func<object, string> GetObjectFormatHandler(Type type, string formatName)
        {
            if (type == null)
                type = typeof(object);

            if (formatName.IsNullOrWhiteSpace())
                return null;

            lock (_padLock)
            {
                if (_objectFormatHandlers.ContainsKey(type))
                {
                    if (_objectFormatHandlers[type] != null && !_objectFormatHandlers[type].ContainsKey(formatName))
                        return _objectFormatHandlers[type][formatName];
                    else
                        return null;
                }
                else if (_objectFormatHandlers[typeof(object)].ContainsKey(formatName))
                    return _objectFormatHandlers[typeof(object)][formatName];
                else
                    return null;
            }
        }

        public static bool AddOrReplace(Type type, string formatName, Func<object, string> handler)
        {
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");
            ChoGuard.ArgumentNotNullOrEmpty(formatName, "FormatName");
            ChoGuard.ArgumentNotNullOrEmpty(handler, "Handler");

            lock (_padLock)
            {
                if (!_objectFormatHandlers.ContainsKey(type))
                    _objectFormatHandlers.Add(type, new Dictionary<string, Func<object, string>>());

                Dictionary<string, Func<object, string>> handlersDict = _objectFormatHandlers[type];
                if (handlersDict.ContainsKey(formatName))
                {
                    handlersDict[formatName] = handler;
                    return false;
                }
                else
                {
                    handlersDict.Add(formatName, handler);
                    return true;
                }
            }
        }
        
        public static bool Contains(string formatName)
        {
            return Contains(null, formatName);
        }

        public static bool Contains(Type type, string formatName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(formatName, "FormatName");
            if (type == null)
                type = typeof(object);
            
            lock (_padLock)
            {
                return _objectFormatHandlers.ContainsKey(type) && _objectFormatHandlers[type] != null && _objectFormatHandlers[type].ContainsKey(formatName);
            }
        }

        public static void Remove(Type type, string formatName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");
            ChoGuard.ArgumentNotNullOrEmpty(formatName, "FormatName");

            if (!_objectFormatHandlers.ContainsKey(type)
                || !_objectFormatHandlers[type].ContainsKey(formatName))
                return;

            lock (_padLock)
            {
                if (!_objectFormatHandlers.ContainsKey(type)
                    || !_objectFormatHandlers[type].ContainsKey(formatName))
                    return;

                _objectFormatHandlers[type].Remove(formatName);
            }
        }
        #endregion Shared Members (Public)
    }
}
