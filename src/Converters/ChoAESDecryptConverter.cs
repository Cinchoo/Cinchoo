using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Cinchoo.Core.Security.Cryptography;

namespace Cinchoo.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ChoAESDecryptConverterAttribute : ChoTypeConverterAttribute
    {
        public ChoAESDecryptConverterAttribute()
            : base(typeof(ChoAESDecryptConverter))
        {
        }
    }

    public class ChoAESDecryptConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string key = null, vector = null;
                if (!ParseParams(parameter, ref key, ref vector))
                    return value;

                using (ChoAESCryptography crypt = new ChoAESCryptography(key, vector))
                    return crypt.Decrypt(value as string);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string key = null, vector = null;
                if (!ParseParams(parameter, ref key, ref vector))
                    return value;

                using (ChoAESCryptography crypt = new ChoAESCryptography(key, vector))
                    return crypt.Encrypt(value as string);
            }
            return value;
        }

        private bool ParseParams(object parameter, ref string key, ref string vector)
        {
            object[] parameters = parameter as object[];
            if (parameters == null || parameters.Length != 2)
                return false;

            if (!(parameters[0] is string))
                return false;

            if (!(parameters[1] is string))
                return false;

            key = parameters[0] as string;
            vector = parameters[1] as string;

            return true;
        }
    }
}
