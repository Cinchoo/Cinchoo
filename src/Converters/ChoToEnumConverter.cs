namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public class ChoToEnumConverter : ChoParameterizedTypeConverter
    {
        #region Instance Data Members (Private)

        private Type _enumType;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoToEnumConverter(object[] parameters)
            : base(parameters)
        {
        }

        #endregion Constructors

        #region TypeConverter Overrides

        protected override void Validate()
        {
            if (Parameters == null)
                throw new NullReferenceException("Missing parameters.");

            if (Parameters.Length != 1)
                throw new ChoApplicationException("Requires 1 parameters [EnumType(Type)].");

            if (!(Parameters[0] is Type))
                throw new ChoApplicationException("First parameter should be of Type type [EnumType(Type)].");

            _enumType = Parameters[0] as Type;
        }

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return false;
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value != null && value.GetType() == typeof(string))
                return Enum.Parse(_enumType, value as string);

            return base.ConvertFrom(context, culture, value);
        }

        #endregion
    }
}
