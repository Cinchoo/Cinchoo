namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;

    using Cinchoo.Core.IO;
    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    public class ChoUniqueArray : TypeConverter
    {
        #region TypeConverter Overrides

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            if (typeof(ICollection).IsAssignableFrom(sourceType))
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
            Type sourceType = typeof(object);
            if (value != null && value is ICollection)
            {
                ArrayList nonDuplicateList = new ArrayList();
                foreach (object element in (ICollection)value)
                {
                    sourceType = element.GetType();
                    break;
                }

                foreach (object element in (ICollection)value)
                {
                    if (IsExists(nonDuplicateList, element)) continue;
                    nonDuplicateList.Add(element);
                }
                return ChoArray.ConvertTo(nonDuplicateList, sourceType);
            }

            return value;
        }

        #endregion

        #region Instance Members (Private)

        private bool IsExists(ArrayList list, object element)
        {
            foreach (object item in list)
            {
                if (item.Equals(element))
                {
                    ChoStringMsgBuilder msg = new ChoStringMsgBuilder("Duplicate entry found");
                    msg.AppendFormatLine(ChoObject.ToString(element));

                    ChoTrace.Debug(msg.ToString());
                    return true;
                }
            }

            return false;
        }

        #endregion Instance Members (Private)
    }
}
