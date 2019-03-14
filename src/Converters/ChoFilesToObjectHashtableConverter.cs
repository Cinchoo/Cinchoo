namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.IO;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public class ChoFilesToObjectHashtableConverter : ChoParameterizedTypeConverter
    {
        public const string InternalHashKey = "$$HashKey$$";

        #region Constructors

        public ChoFilesToObjectHashtableConverter(object[] parameters)
            : base(parameters)
        {
        }

        #endregion Constructors

        #region TypeConverter Overrides

        protected override void Validate()
        {
            if (Parameters == null)
                throw new NullReferenceException("Missing parameters.");

            if (Parameters.Length != 3)
                throw new ChoApplicationException("Requires 3 parameters [DepenedentMemberName(String), MemberElementType(Type), KeyName(String)].");

            if (!(Parameters[0] is string))
                throw new ChoApplicationException("First parameter should be of string type [DepenedentMemberName(String)].");

            if (!(Parameters[1] is Type))
                throw new ChoApplicationException("Second parameter should be of Type type [MemberElementType(Type)].");

            if (!(Parameters[2] is string))
                throw new ChoApplicationException("Second parameter should be of Type type [KeyName(String)].");
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
                        Hashtable convertedObjects = new Hashtable();
                        foreach (string fileName in (string[])value)
                        {
                            object element = GetObject(fileName, outerProfile);
                            if (KeyName == InternalHashKey)
                                convertedObjects.Add(element.GetHashCode().ToString(), element);
                            else
                                convertedObjects.Add(ChoType.GetMemberValue(element, KeyName).ToString(), element);
                        }

                        convertedObject = convertedObjects;
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
                //if (ChoConfigurationManager.GetConfigEntryByConfigSectionObject(Target) != null)
                //    ChoConfigurationManager.GetConfigEntryByConfigSectionObject(Target).IncludeFileList.Add(fileName);
            }

            return convertedObject;
        }

        private string DependentMemberName
        {
            get { return Parameters[0] as string; }
        }

        private Type ObjElementType
        {
            get { return Parameters[1] as Type; }
        }

        private string KeyName
        {
            get { return Parameters[2] as string; }
        }

        #endregion Instance Properties (Private)
    }
}
