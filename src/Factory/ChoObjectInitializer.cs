namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Runtime.Remoting;

    #endregion NameSpaces

    public static class ChoObjectInitializer
    {
        #region Shared Data Members (Private)

        private static List<WeakReference> _initializedObjectCache = new List<WeakReference>();
        private static object _initializedObjectCacheLock = new object();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static void Initialize(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            Initialize(target, false, null);
        }

        public static void Initialize(object target, bool beforeFieldInit)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            Initialize(target, beforeFieldInit, null);
        }

        public static void Initialize(object target, bool beforeFieldInit, object state)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            DoInitialize(target, beforeFieldInit, state);
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static bool IsInitialized(object target)
        {
            lock (_initializedObjectCacheLock)
            {
                for (int index = _initializedObjectCache.Count - 1; index >= 0; index--)
                {
                    if (!_initializedObjectCache[index].IsAlive) _initializedObjectCache.RemoveAt(index);
                }
                foreach (WeakReference weakRef in _initializedObjectCache)
                {
                    if (!weakRef.IsAlive) continue;
                    if (weakRef.Target == target) return true;
                }
            }

            return false;
        }

        private static void DoInitialize(object target, bool beforeFieldInit, object state)
        {
            if (target == null) return;

            if (target is IChoObjectInitializable)
            {
                if (!beforeFieldInit)
                {
                    if (IsInitialized(target)) return;

                    lock (_initializedObjectCacheLock)
                    {
                        _initializedObjectCache.Add(new WeakReference(target));
                    }
                }
            }

            if (!beforeFieldInit)
                DoObjectMemberConversion(target, target);

            if (target != state)
            {
                if (DoObjectMemberInitialization(target, beforeFieldInit, state))
                {
                    if (!beforeFieldInit)
                    {
                        DoObjectMemberConversion(target, target);
                        DoObjectMemberValidation(target);
                    }
                }
            }
            else
                DoObjectMemberValidation(target);
        }

        private static bool DoObjectMemberInitialization(object target, bool beforeFieldInit, object state)
        {
            if (target.GetType().IsPrimitive
                || target is string || target is Enum
                )
            {
            }
            else
            {
                //Call the initialize to all members
                foreach (FieldInfo fieldInfo in ChoType.GetFields(target.GetType()))
                {
                    object fieldValue = ChoType.GetFieldValue(target, fieldInfo.Name);
                    if (fieldValue == null) continue;

                    if (!(fieldValue is string) && fieldValue is IEnumerable)
                    {
                        foreach (object fieldItemValue in (IEnumerable)fieldValue)
                            DoInitialize(fieldItemValue, beforeFieldInit, state);

                        DoInitialize(fieldValue, beforeFieldInit, state);
                    }
                    else
                        DoInitialize(fieldValue, beforeFieldInit, state);
                }
            }

            if (target is IChoObjectInitializable)
                return ((IChoObjectInitializable)target).Initialize(beforeFieldInit, state);
            else
                return false;
        }

        private static void DoObjectMemberConversion(object ultimateParent, object target)
        {
            if (target == null) return;

            if (target.GetType().IsPrimitive
                || target is string || target is Enum
                )
            {
            }
            else
            {
                DoPostObjectMemberConversion(ultimateParent, target);

                //Call the initialize to all members
                foreach (FieldInfo fieldInfo in ChoType.GetFields(target.GetType()))
                {
                    object fieldValue = ChoType.GetFieldValue(target, fieldInfo.Name);
                    if (fieldValue == null) continue;

                    if (!(fieldValue is string) && fieldValue is IEnumerable)
                    {
                        foreach (object fieldItemValue in (IEnumerable)fieldValue)
                            DoObjectMemberConversion(ultimateParent, fieldItemValue);

                        ChoType.SetFieldValue(target, fieldInfo.Name, fieldValue);
                    }
                    else
                        DoObjectMemberConversion(ultimateParent, fieldValue);
                }
            }
        }

        private static void DoPostObjectMemberConversion(object ultimateParent, object configObject)
        {
            bool hasError = false;
            foreach (MemberInfo memberInfo in ChoType.GetMemberInfos(configObject.GetType(), typeof(ChoTypeConverterAttribute)))
            {
                object memberValue = null;
                try
                {
                    memberValue = ChoType.GetMemberValue(configObject, memberInfo.Name);
                    ChoType.SetMemberValue(configObject, memberInfo, ChoObject.ConvertValueToObjectMemberType(configObject, memberInfo, memberValue) /* ChoConvert.ChangeType(configObject, memberInfo) */);
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

                    ChoConfigurationObjectErrorManagerService.SetObjectError(ultimateParent, "Object has some validation errors.");
                    ChoConfigurationObjectErrorManagerService.SetObjectMemberError(configObject, memberInfo.Name, String.Format("[Value: {0}] - {1}", ChoString.ToString(memberValue), ex.Message));
                    ChoType.SetMemberDefaultValue(configObject, memberInfo.Name);
                }
            }

            if (hasError)
                ChoConfigurationObjectErrorManagerService.SetObjectError(configObject, "Object has some validation errors.");
        }

        private static void DoObjectMemberValidation(object target)
        {
            if (target == null) return;

            if (target.GetType().IsPrimitive
                || target is string || target is Enum
                )
            {
            }
            else
            {
                //Call the initialize to all members
                foreach (FieldInfo fieldInfo in ChoType.GetFields(target.GetType()))
                {
                    object fieldValue = ChoType.GetFieldValue(target, fieldInfo.Name);
                    if (fieldValue == null) continue;

                    if (!(fieldValue is string) && fieldValue is IEnumerable)
                    {
                        foreach (object fieldItemValue in (IEnumerable)fieldValue)
                            DoObjectMemberValidation(fieldItemValue);
                    }
                    else
                        DoObjectMemberValidation(fieldValue);
                }
            }

            if (target != null && !(target is ChoRealProxy))
            {
                //Do Validate
                ChoDoObjectValidationAfterInitializationAttribute doObjectValidationAfterInitializationAttribute =
                    ChoType.GetAttribute<ChoDoObjectValidationAfterInitializationAttribute>(target.GetType());

                if (doObjectValidationAfterInitializationAttribute != null
                    && doObjectValidationAfterInitializationAttribute.DoObjectValidation)
                {
                    ChoValidationResults validationResults = ChoValidation.Validate(target);
                    if (validationResults != null && validationResults.Count > 0)
                    {
                        throw new ChoValidationException("Failed to validate object.", validationResults);
                    }
                }
            }
        }

        #endregion Shared Members (Private)
    }
}
