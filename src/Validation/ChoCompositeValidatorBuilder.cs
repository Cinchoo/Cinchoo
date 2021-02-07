namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Linq;
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
            string validatorName = null;
            List<IChoSurrogateValidator> validators = new List<IChoSurrogateValidator>();
            Dictionary<string, IChoSurrogateValidator> namedValidators = new Dictionary<string, IChoSurrogateValidator>();
            List<ChoCompositeValidatorAttribute> compositeValidatorsAttrs = new List<ChoCompositeValidatorAttribute>();

            ChoCompositeValidator topCompositeValidator = new ChoAndCompositeValidator();
            ChoCompositeValidator compositeValidator = topCompositeValidator;

            Attribute[] attrs = ChoType.GetMemberAttributesByBaseType(memberInfo, typeof(Attribute));

            //Lookup all non composite validators, build and cache them
            foreach (Attribute memberCallAttribute in attrs)
            {
                if (!(memberCallAttribute is ChoCompositeValidatorAttribute))
                {
                    foreach (IChoValidationManager validationManager in ChoValidationManagerSettings.Me.ValidationManagers)
                    {
                        if (validationManager.IsValid(memberCallAttribute, out validatorName))
                        {
                            if (!validatorName.IsNullOrWhiteSpace() && namedValidators.ContainsKey(validatorName)) continue;

                            IChoSurrogateValidator validator = validationManager.CreateValidator(memberCallAttribute, ValidationScope.Before, ValidatorSource.Attribute);
                            if (validator != null)
                            {
                                if (validatorName.IsNullOrWhiteSpace())
                                    validators.Add(validator);
                                else
                                    namedValidators.Add(validatorName, validator);
                            }

                            break;
                        }
                    }
                }
                else if (memberCallAttribute is ChoCompositeValidatorAttribute)
                {
                    compositeValidatorsAttrs.Add(memberCallAttribute as ChoCompositeValidatorAttribute);
                }
            }

            //Build and cache all the composite validators
            foreach (ChoCompositeValidatorAttribute memberCallAttribute in compositeValidatorsAttrs)
            {
                if (memberCallAttribute.Name.IsNullOrWhiteSpace())
                {
                    IChoSurrogateValidator validator = BuildCompositeValidator(memberCallAttribute as ChoCompositeValidatorAttribute, namedValidators, compositeValidatorsAttrs.ToArray());
                    if (validator != null)
                        validators.Add(validator);
                }
            }

            _objectMemberValidatorCache.Add(memberInfo, new ChoAndCompositeValidator(validators.ToArray()));
        }

        private static IChoSurrogateValidator BuildCompositeValidator(ChoCompositeValidatorAttribute compositeValidatorAttribute, Dictionary<string, IChoSurrogateValidator> namedValidators,
            ChoCompositeValidatorAttribute[] attrs, List<ChoCompositeValidator> parentValidators = null)
        {
            ChoCompositeValidator compositeValidator = null;

            if (compositeValidatorAttribute.CompositionType == ChoCompositionType.Or)
                compositeValidator = new ChoOrCompositeValidator();
            else
                compositeValidator = new ChoAndCompositeValidator();

            compositeValidator.Name = compositeValidatorAttribute.Name;
            if (parentValidators == null)
                parentValidators = new List<ChoCompositeValidator>();

            parentValidators.Add(compositeValidator);

            foreach (string name in compositeValidatorAttribute.ValidatorNames)
            {
                if (name.IsNullOrWhiteSpace()) continue;
                if (namedValidators.ContainsKey(name))
                {
                    if (!(namedValidators[name] is ChoCompositeValidator))
                        compositeValidator.Add(namedValidators[name]);
                    else if (!IsCircularReferenceFound(parentValidators, name))
                    {
                        compositeValidator.Add(namedValidators[name]);
                        parentValidators.Add(namedValidators[name] as ChoCompositeValidator);
                    }

                    continue;
                }
                else
                {
                    foreach (Attribute memberCallAttribute in attrs)
                    {
                        if (!(memberCallAttribute is ChoCompositeValidatorAttribute)) continue;
                        if (((ChoCompositeValidatorAttribute)memberCallAttribute).Name != name) continue;
                        if (IsCircularReferenceFound(parentValidators, name)) continue;

                        IChoSurrogateValidator childCompositeValidator = null;

                        childCompositeValidator = BuildCompositeValidator(memberCallAttribute as ChoCompositeValidatorAttribute,
                            namedValidators, attrs, parentValidators);

                        namedValidators.Add(name, childCompositeValidator);

                        compositeValidator.Add(childCompositeValidator);
                        parentValidators.Add(namedValidators[name] as ChoCompositeValidator);
                    }
                }
            }

            return compositeValidator;
        }

        private static bool IsCircularReferenceFound(List<ChoCompositeValidator> parentValidators, string name)
        {
            if (parentValidators == null) return false;

            return parentValidators.SingleOrDefault((x) => x.Name == name) != null;
        }

        #endregion Shared Members (Private)
    }
}
