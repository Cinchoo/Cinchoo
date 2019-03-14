namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;

    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    public class ChoIgnoreInvalidArrayItemsConvertor : ChoBaseConverter
    {
        #region Constructors

        public ChoIgnoreInvalidArrayItemsConvertor(string format) : base(format)
        {
        }

        #endregion Constructors

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType.IsArray)
                return true;
            else
                return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null) return null;

            ChoNotNullableArrayList arrayList = new ChoNotNullableArrayList();
            object notNullItem = null;
            if (value.GetType().IsArray)
            {
                foreach (object item in (Array)value)
                {
                    if (item == null) continue;
                    notNullItem = item;

                    ChoValidationResults validationResults = ChoValidation.Validate(item);
                    if (validationResults != null && validationResults.Count > 0)
                        continue;

                    arrayList.Add(item);
                }
                return notNullItem == null ? null : arrayList.ToArray(notNullItem.GetType());
            }
            else
                return value;
        }
    }
}
