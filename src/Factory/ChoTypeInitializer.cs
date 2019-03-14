namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    public static class ChoTypeInitializer
    {
        #region Shared Data Members (Private)

        private static List<WeakReference> _initializedTypeCache = new List<WeakReference>();
        private static object _initializedTypeCacheLock = new object();

        #endregion Shared Data Members (Private)
        
        #region Shared Members (Public)

        public static void Initialize(Type type)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            if (IsInitialized(type)) return;

            DoPostTypeMemberConversion(type);

            foreach (FieldInfo fieldInfo in ChoType.GetFields(type))
            {
                if (!fieldInfo.IsStatic) continue;

                object fieldValue = ChoType.GetStaticFieldValue(type, fieldInfo.Name);
                if (fieldValue == null) continue;

                if (!(fieldValue is string) && fieldValue is IEnumerable)
                {
                    foreach (object fieldItemValue in (IEnumerable)fieldValue)
                    {
                        if (!(fieldItemValue is IChoObjectInitializable))
                            break;

                        ChoObjectInitializer.Initialize(fieldItemValue);
                    }
                }
                else if (fieldValue.GetType().IsPrimitive || fieldValue is string)
                {
                }
                else
                    ChoObjectInitializer.Initialize(fieldValue);
            }
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void DoPostTypeMemberConversion(Type type)
        {
            bool hasError = false;
            foreach (MemberInfo memberInfo in ChoType.GetMemberInfos(type, typeof(ChoTypeConverterAttribute)))
            {
                object memberValue = null;
                try
                {
                    memberValue = ChoType.GetMemberValue(null, memberInfo.Name);
                    ChoType.SetMemberValue(null, memberInfo, ChoObject.ConvertValueToObjectMemberType(type, memberInfo, memberValue) /* ChoConvert.ChangeType(configObject, memberInfo) */);
                }
                catch (TargetInvocationException)
                {
                    throw;
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    hasError = true;

                    ChoConfigurationObjectErrorManagerService.SetObjectMemberError(type, memberInfo.Name, String.Format("[Value: {0}] - {1}", ChoString.ToString(memberValue), ex.Message));
                    ChoType.SetMemberDefaultValue(null, memberInfo.Name);
                }
            }

            if (hasError)
                ChoConfigurationObjectErrorManagerService.SetObjectError(type, "Object has some validation errors.");
        }

        private static bool IsInitialized(Type type)
        {
            lock (_initializedTypeCacheLock)
            {
                for (int index = _initializedTypeCache.Count - 1; index >= 0; index--)
                {
                    if (!_initializedTypeCache[index].IsAlive) _initializedTypeCache.RemoveAt(index);
                }
                foreach (WeakReference weakRef in _initializedTypeCache)
                {
                    if (!weakRef.IsAlive) continue;
                    if ((Type)weakRef.Target == type) return true;
                }
            }

            return false;
        }

        #endregion Shared Members (Private)
    }
}
