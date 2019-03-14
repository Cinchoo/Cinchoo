namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Cinchoo.Core.Collections.Generic;

    #endregion NameSpaces

    internal class ChoCompositeValidatorBuilder
    {
        #region Shared Data Members (Private)

        private static ChoDictionary<MemberInfo, IChoSurrogateValidator> _objectMemberValidatorCache = new ChoDictionary<MemberInfo, IChoSurrogateValidator>();

        #endregion Shared Data Members (Private)

        #region Shared Members (Public)

        public static IChoSurrogateValidator GetValidator(MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(memberInfo, "memberInfo");
            if (!ChoType.IsValidObjectMember(memberInfo))
                return null;

            //If the cache doesn't have, build it
            if (!_objectMemberValidatorCache.ContainsKey(memberInfo))
            {
                lock (_objectMemberValidatorCache.SyncRoot)
                {
                    if (!_objectMemberValidatorCache.ContainsKey(memberInfo))
                        Build(memberInfo);
                }
            }

            return _objectMemberValidatorCache[memberInfo];
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void Build(MemberInfo memberInfo)
        {
            List<IChoSurrogateValidator> validators = new List<IChoSurrogateValidator>();

            ChoCompositeValidator compositeValidator = new ChoAndCompositeValidator();
            foreach (Attribute memberCallAttribute in ChoType.GetMemberAttributesByBaseType(memberInfo, typeof(Attribute)))
            {
                foreach (IChoValidationManager validationManager in ChoValidationManagerSettings.Me.ValidationManagers)
                {
                    if (memberCallAttribute is ChoCompositeValidatorAttribute)
                    {
                        if (((ChoCompositeValidatorAttribute)memberCallAttribute).CompositionType == ChoCompositionType.Or)
                        {
                            if (validators.Count > 0)
                            {
                                compositeValidator.Add(validators.ToArray());
                                validators.Add(compositeValidator);
                                validators.Clear();
                            }
                            compositeValidator = new ChoOrCompositeValidator();
                        }
                    }
                    else if (validationManager.IsValid(memberCallAttribute))
                    {
                        IChoSurrogateValidator validator = validationManager.CreateValidator(memberCallAttribute, ValidationScope.Before, ValidatorSource.Attribute);
                        if (validator != null)
                            validators.Add(validator);
                    }
                }
            }
            if (validators.Count > 0)
            {
                compositeValidator.Add(validators.ToArray());
                validators.Add(compositeValidator);
            }

            _objectMemberValidatorCache.Add(memberInfo, new ChoAndCompositeValidator(validators.ToArray()));
        }

        #endregion Shared Members (Private)

        //    private static void Build(object target)
    //    {
    //        if (target == null)
    //            return;

    //        if (_objectValidatorCache.ContainsKey(target.GetType().FullName))
    //            return;

    //        ChoDictionary<string, IChoSurrogateValidator> memberValidators = new ChoDictionary<string, IChoSurrogateValidator>();

    //        foreach (MemberInfo memberInfo in ChoType.GetMembers(target.GetType()))
    //        {
    //            ChoCompositeValidator compositeValidator = new ChoAndCompositeValidator();
    //            List<IChoSurrogateValidator> validators = new List<IChoSurrogateValidator>();
    //            foreach (Attribute memberCallAttribute in ChoType.GetMemberAttributesByBaseType(memberInfo, typeof(Attribute)))
    //            {
    //                foreach (IChoValidationManager validationManager in ChoValidationManagerSettings.Me.ValidationManagers)
    //                {
    //                    if (memberCallAttribute is ChoCompositeValidatorAttribute)
    //                    {
    //                        if (((ChoCompositeValidatorAttribute)memberCallAttribute).CompositionType == ChoCompositionType.Or)
    //                            compositeValidator = new ChoOrCompositeValidator();
    //                    }
    //                    else if (validationManager.IsValid(memberCallAttribute))
    //                    {
    //                        IChoSurrogateValidator validator = validationManager.CreateValidator(target, memberCallAttribute, ValidationScope.Before, ValidatorSource.Attribute);
    //                        if (validator != null)
    //                            validators.Add(validator);
    //                    }
    //                }
    //            }
    //            if (validators.Count > 0)
    //            {
    //                compositeValidator.Add(validators.ToArray());
    //                memberValidators.Add(memberInfo.Name, compositeValidator);
    //            }
    //        }

    //        if (_objectValidatorCache.ContainsKey(target.GetType().FullName))
    //            _objectValidatorCache[target.GetType().FullName] = memberValidators;
    //        else
    //            _objectValidatorCache.Add(target.GetType().FullName, memberValidators);
    //    }

    //    #endregion Shared Members (Private)
    }
}
