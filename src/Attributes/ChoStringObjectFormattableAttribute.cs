namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoStringObjectFormattableAttribute : Attribute
    {
        public readonly Type SupportedType;
        
        public ChoStringObjectFormattableAttribute()
        {
        }

        public ChoStringObjectFormattableAttribute(Type supportedType)
        {
            ChoGuard.ArgumentNotNullOrEmpty(supportedType, "SupportedType");

            SupportedType = supportedType;
        }
    }
}