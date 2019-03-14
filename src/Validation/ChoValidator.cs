namespace eSquare.Core.Validation
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    using eSquare.Core.Attributes;
    using eSquare.Core.Attributes.Validators;

    #endregion NameSpaces

    public static class ChoValidator
    {
        public static ChoValidationResults Validate(object target)
        {
            ChoValidationResults validationResults = new ChoValidationResults();
            if (target != null) DoValidate(target, validationResults);

            return validationResults;
        }

        private static void DoValidate(object target, ChoValidationResults validationResults)
        {
            bool canContinue = true;
            foreach (MethodInfo methodInfo in GetValidationRoutines(target.GetType()))
            {
                canContinue = false;

                canContinue = (bool)ChoType.InvokeMethod(target, methodInfo.Name, validationResults);
                if (!canContinue) break;
            }

            //Do built-in attribute validations
            if (canContinue)
            {
                MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType());
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    foreach (ChoMemberAttribute memberAttribute in ChoType.GetMemberAttributesByBaseType(memberInfo,
                        typeof(ChoMemberAttribute)))
                    {
                        try
                        {
                            memberAttribute.Validate(ChoType.GetMemberValue(target, memberInfo.Name), false);
                        }
                        catch (Exception ex)
                        {
                            validationResults.AddResult(ex.Message);
                        }
                    }
                }
            }
        }

        private static IEnumerable<MethodInfo> GetValidationRoutines(Type type)
        {
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                bool hasBoolReturnType = methodInfo.ReturnType == typeof(bool);
                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (hasBoolReturnType && parameters.Length == 1 && parameters[0].ParameterType == typeof(ChoValidationResults))
                    yield return methodInfo;
            }
        }
    }
}
