namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;

	using System.Text;

    #endregion NameSpaces

    public class ChoStringToStringArrayConverter : ChoParameterizedTypeConverter
    {
        #region Instance Data Members (Private)

        private char _separator = ';';

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoStringToStringArrayConverter() : base(null)
        {
        }

        public ChoStringToStringArrayConverter(object[] parameters) : base(parameters)
        {
        }

        #endregion Constructors

        #region ChoParameterizedTypeConverter Overrides

        protected override void Validate()
        {
            base.Validate();

            if (Parameters != null)
            {
                if (Parameters.Length > 0 && Parameters[0] is char)
                {
                    _separator = (char)Parameters[0];
                }
            }
        }

        #endregion ChoParameterizedTypeConverter Overrides

        #region TypeConverter Overrides

        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
			if (sourceType == typeof(string))
				return true;
			else
				return false;
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null) return null;
            if (value.GetType() == typeof(string))
            {
                string valueString = ((string)value).Trim();
                if (string.IsNullOrEmpty(valueString)) return null;
                return valueString.SplitNTrim(_separator);
            }
			return null; //base.ConvertFrom(context, culture, value);
        }

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			else
				return false;
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				StringBuilder msg = new StringBuilder();
				if (value is string[])
				{
					foreach (string arrItem in (string[])value)
					{
						if (msg.Length == 0)
							msg.AppendFormat("{0}".FormatString(arrItem.ToString()));
						else
                            msg.AppendFormat("{1} {0}".FormatString(arrItem.ToString(), _separator));
					}
				}
				return msg.ToString();
			}
			else
				return null;
		}

        public override string Help()
        {
            return "Parameters = new object[] {','})";
        }

        #endregion
    }
}
