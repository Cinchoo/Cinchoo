namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;
    using Cinchoo.Core.Factory;

    #endregion NameSpaces

    public static class ChoValidation
    {
        #region Shared Members (Public)

        public static ChoValidationResults Validate<T>(T target)
        {
            ChoValidationResults validationResults = new ChoValidationResults();
            if (target != null) DoValidate(target, validationResults);

            return validationResults;
        }

        public static void Validate(MemberInfo memberInfo, object memberValue)
        {
            IChoSurrogateValidator validator = ChoCompositeValidatorBuilder.GetValidator(memberInfo);
            if (validator == null) return;

            validator.Validate(memberInfo, memberValue);
        }

        #endregion Shared Members (Public)

        #region Shared Members (Private)

        private static void DoValidate<T>(T target, ChoValidationResults validationResults)
        {
            bool validationRoutineFound = false;
            bool canContinue = true;
            foreach (MethodInfo methodInfo in GetValidationRoutines(target.GetType()))
            {
                canContinue = false;
                validationRoutineFound = true;
                canContinue = (bool)ChoType.InvokeMethod(target, methodInfo.Name, new object[] { validationResults });
                if (!canContinue) break;
            }

            //Do built-in attribute validations
            if (!validationRoutineFound)
            {
                MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType());
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    IChoSurrogateValidator validator = ChoCompositeValidatorBuilder.GetValidator(memberInfo);
                    if (validator == null) continue;

                    try
                    {
                        validator.Validate(memberInfo, ChoType.GetMemberValue(target, memberInfo.Name));
                    }
                    catch (ChoFatalApplicationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        validationResults.AddResult(new ChoValidationResult(ex.Message));
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

        #endregion Shared Members (Private)
    }
}
