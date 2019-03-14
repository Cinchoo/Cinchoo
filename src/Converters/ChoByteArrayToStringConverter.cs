namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoByteArrayToStringConverter : IChoValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is string))
                return value;

            if (targetType == typeof(byte[]))
            {
                List<byte> byteArr = new List<byte>();
                foreach (string token in ((string)value).SplitNTrim())
                {
                    byteArr.Add(System.Convert.ToByte(token));
                }

                return byteArr.ToArray();
            }
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is byte[]))
                return value;

            if (targetType == typeof(String))
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte token in (byte[])value)
                {
                    if (sb.Length == 0)
                        sb.Append(token);
                    else
                        sb.AppendFormat(",{0}", token);
                }

                return sb.ToString();
            }
            else
                return value;
        }
    }
}
