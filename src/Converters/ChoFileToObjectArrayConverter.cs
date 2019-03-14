namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public class ChoFilesToObjectArrayConverter : ChoParameterizedTypeConverter
    {
        #region Constructors

        public ChoFilesToObjectArrayConverter(object[] parameters) : base(parameters)
        {
        }

        #endregion Constructors

        #region TypeConverter Overrides

        protected override void Validate()
        {
            if (Parameters == null)
                throw new NullReferenceException("Missing parameters.");

            if (Parameters.Length != 2)
                throw new ChoApplicationException("Requires two parameters [DepenedentMemberName(String), MemberElementType(Type)].");
 
            if (!(Parameters[0] is string))
                throw new ChoApplicationException("First parameter should be of string type [DepenedentMemberName(String)].");
 
            if (!(Parameters[1] is Type))
                throw new ChoApplicationException("Second parameter should be of Type type [MemberElementType(Type)].");
        }

        // Returns true for a sourceType of string to indicate that 
        // conversions from string to integer are supported. (The 
        // GetStandardValues method requires a string to native type 
        // conversion because the items in the drop-down list are 
        // translated to string.)
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            sourceType = ChoType.GetMemberType(Target.GetType(), DependentMemberName);
            if (sourceType == typeof(string) || sourceType == typeof(string[]))
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
            value = ChoType.GetMemberValue(Target, DependentMemberName);

            object convertedObject = null;

            using (ChoBufferProfileEx outerProfile = new ChoBufferProfileEx(LogFileName, "Converting file to objects...."))
            {
                try
                {
                    if (value.GetType() == typeof(string))
                    {
                        convertedObject = GetObject(value as string, outerProfile);
                    }
                    else if (value.GetType() == typeof(string[]))
                    {
                        ChoNotNullableArrayList convertedObjects = new ChoNotNullableArrayList();
                        foreach (string fileName in (string[])value)
                            convertedObjects.Add(GetObject(fileName, outerProfile));

                        convertedObject = convertedObjects.ToArray(ObjElementType);
                    }
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    outerProfile.Append(ex);
                }
            }

            return convertedObject;
        }

        #endregion

        #region Instance Properties (Private)

        private object GetObject(string fileName, ChoBufferProfileEx outerProfile)
        {
            object convertedObject = null;
            using (ChoBufferProfileEx fileProfile = new ChoBufferProfileEx("Converting {0} file to objects...", fileName, outerProfile))
            {
                if (!File.Exists(fileName))
                    outerProfile.AppendLine("{0} file not exists.", fileName);
                
                try
                {
                    object element = ChoFile.GetObject(fileName);
                    if (element != null)
                    {
                        if (element.GetType() != ObjElementType)
                            throw new ChoApplicationException(String.Format("Unexpected member element. Expected: {0}. Actual: {1}", ObjElementType.FullName,
                                element.GetType().FullName));

                        convertedObject = element;
                    }
                }
                catch (Exception innerEx)
                {
                    fileProfile.Append(innerEx);
                }

                //REDO:
                //string configSectionName = ChoConfigurationManager.GetConfigSectionName(Target.GetType());
                //if (!String.IsNullOrEmpty(configSectionName) && ChoConfigurationManager.GetConfigEntryByConfigSectionName(configSectionName) != null)
                //    ChoConfigurationManager.GetConfigEntryByConfigSectionName(configSectionName).IncludeFileList.Add(fileName);
            }

            return convertedObject;
        }

        private string DependentMemberName
        {
            get
            {
                if (Parameters == null || Parameters.Length != 2 || !(Parameters[0] is string))
                    throw new NullReferenceException("Missing dependent member name.");

                return Parameters[0] as string;
            }
        }

        private Type ObjElementType
        {
            get
            {
                if (Parameters == null || Parameters.Length != 2 || !(Parameters[1] is Type))
                    throw new NullReferenceException("Missing object element type.");

                return Parameters[1] as Type;
            }
        }

        #endregion Instance Properties (Private)
    }
}
