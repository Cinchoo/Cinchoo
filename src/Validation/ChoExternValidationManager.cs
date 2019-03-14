namespace eSquare.Core.Validation
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Interfaces;
    using eSquare.Core.Attributes;
    using eSquare.Core.Configuration;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Formatters;

    #endregion NameSpaces

    public static class ChoExternValidationManager
    {
        private static IChoValidationManager _externValidationManager = ChoType.CreateInstance("eSquare.Core.Validations.MVAB.MVAB3ValidationManager, eSquare.Core.Validations.MVAB") as IChoValidationManager;
        internal static bool IsAvailable = true;

        public static void PreValidate(object target, MemberInfo memberInfo, object memberValue)
        {
            if (!IsAvailable) return;

            foreach (IChoValidationManager externValidationManager in ChoValidationManagerSettings.Me.ValidationManagers)
            {
                if (externValidationManager == null) continue;
                externValidationManager.PreValidate(target, memberInfo, memberValue);
            }
        }

        public static void PostValidate(object target, MemberInfo memberInfo, object memberValue)
        {
            if (!IsAvailable) return;

            foreach (IChoValidationManager externValidationManager in ChoValidationManagerSettings.Me.ValidationManagers)
            {
                if (externValidationManager == null) continue;
                externValidationManager.PostValidate(target, memberInfo, memberValue);
            }
        }

        public static ChoValidationResults Validate<T>(T target)
        {
            if (IsAvailable)
            {
                foreach (IChoValidationManager externValidationManager in ChoValidationManagerSettings.Me.ValidationManagers)
                {
                    if (externValidationManager == null) continue;
                    return externValidationManager.Validate(target);
                }
            }
            return new ChoValidationResults();
        }
    }
}
