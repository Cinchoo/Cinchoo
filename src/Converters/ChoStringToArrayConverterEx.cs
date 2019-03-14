//namespace Cinchoo.Core.Converters
//{
//    #region NameSpaces

//    using System;
//    using System.Text;
//    using System.ComponentModel;

//    using Cinchoo.Core.Types;
//    using System.IO;
//    using Cinchoo.Core.Diagnostics;
//    using System.Collections;
//    using Cinchoo.Core.Configuration;
//    using Cinchoo.Core.Configuration.Sections;
//    using System.Xml;
//    using Cinchoo.Core.Xml.Serialization;
//    using Cinchoo.Core.IO;
//    using Cinchoo.Core.Exceptions;
//    using Cinchoo.Core.Collections;

//    #endregion NameSpaces

//    public class ChoStringToArrayConverter : ChoTypeConverter
//    {
//        #region Constructors

//        public ChoStringToArrayConverter(object[] parameters)
//            : base(parameters)
//        {
//        }

//        #endregion Constructors

//        #region TypeConverter Overrides

//        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
//        {
//            return true;
//        }

//        // Returns a StandardValuesCollection of standard value objects.
//        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
//        {
//            return null;
//        }

//        public override void Validate()
//        {
//            if (_parameters == null)
//                throw new NullReferenceException("Missing parameters.");

//            if (_parameters.Length != 1)
//                throw new ChoApplicationException("Requires one parameter [ArrayItemType(Type)].");

//            if (!(_parameters[0] is Type))
//                throw new ChoApplicationException("First parameter should be of 'Type' type [ArrayItemType(Type)].");
//        }

//        // Returns true for a sourceType of string to indicate that 
//        // conversions from string to integer are supported. (The 
//        // GetStandardValues method requires a string to native type 
//        // conversion because the items in the drop-down list are 
//        // translated to string.)
//        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
//        {
//            sourceType = ChoType.GetMemberType(Target.GetType(), DependentMemberName);
//            if (sourceType == typeof(string) || sourceType == typeof(string[]))
//                return true;
//            else
//                return false;
//        }

//        // If the type of the value to convert is string, parses the string 
//        // and returns the integer to set the value of the property to. 
//        // This example first extends the integer array that supplies the 
//        // standard values collection if the user-entered value is not 
//        // already in the array.
//        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
//        {
//            value = ChoType.GetMemberValue(Target, DependentMemberName);

//            object convertedObject = null;

//            using (ChoBufferProfileEx outerProfile = new ChoBufferProfileEx(LogFileName, "Converting file to objects...."))
//            {
//                try
//                {
//                    if (value.GetType() == typeof(string))
//                    {
//                        convertedObject = GetObject(value as string, outerProfile);
//                    }
//                    else if (value.GetType() == typeof(string[]))
//                    {
//                        ChoNotNullableArrayList convertedObjects = new ChoNotNullableArrayList();
//                        foreach (string fileName in (string[])value)
//                            convertedObjects.Add(GetObject(fileName, outerProfile));

//                        convertedObject = convertedObjects.ToArray(ObjElementType);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    outerProfile.Append(ex);
//                }
//            }

//            return convertedObject;
//        }

//        #endregion

//        #region Instance Properties (Private)

//        private Type ArrayItemType
//        {
//            get
//            {
//                if (_parameters == null || _parameters.Length != 1 || !(_parameters[0] is Type))
//                    throw new NullReferenceException("Missing Array Item type.");

//                return _parameters[0] as Type;
//            }
//        }

//        #endregion Instance Properties (Private)
//    }
//}
