namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public class ChoObjectMemberResolver : ChoParameterizedTypeConverter
    {
        #region Constructors

        public ChoObjectMemberResolver(object[] parameters)
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
                throw new ChoApplicationException("Requires 1 parameters [MemberName(String)].");

            if (!(Parameters[0] is string))
                throw new ChoApplicationException("First parameter should be of string type [MemberName(String)].");
        }

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return true;
        }

        // If the type of the value to convert is string, parses the string 
        // and returns the integer to set the value of the property to. 
        // This example first extends the integer array that supplies the 
        // standard values collection if the user-entered value is not 
        // already in the array.
        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return ChoObject.Evaluate(Target, (string)Parameters[0]);
        }

        #endregion
    }
}
