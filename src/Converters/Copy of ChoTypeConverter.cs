namespace Cinchoo.Core.Converters
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using Cinchoo.Core.Diagnostics;
    using System.IO;
    using Cinchoo.Core.IO;
    using System.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoTypeConverter : TypeConverter
    {
        #region Instance Data Members (Private)

        protected object[] _parameters;
        protected object _target;
        protected string[] _dependentMemberNames;
        protected string _logFileName;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoTypeConverter(object[] parameters)
        {
            ParseNLoadParameters(parameters);
            Validate();
        }

        #endregion Constructors

        #region TypeConverter Overrides

        public virtual void Validate()
        {
        }

        public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
        {
            return true;
        }

        // Returns a StandardValuesCollection of standard value objects.
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            return null;
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
                return base.CanConvertFrom(context, sourceType);
        }

        #endregion

        #region Instance Properties (Public)

        public object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string LogFileName
        {
            get 
            { 
                if (_logFileName.IsNullOrEmpty())
                    _logFileName = Path.Combine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(Target.GetType().FullName, ChoReservedFileExt.Log));

                return _logFileName;
            }
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Private)

        private void ParseNLoadParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length == 0) return;

            List<object> newParameters = new List<object>();
            List<string> dependentMemberNames = new List<string>();
            foreach (object parameter in parameters)
            {
                if (parameter == null) continue;

                if (parameter is String
                    && !((string)parameter).IsNullOrEmpty())
                {
                    if (((string)parameter).StartsWith("{this.")
                        && ((string)parameter).EndsWith("}"))
                        dependentMemberNames.Add(((string)parameter));
                    else if (((string)parameter).StartsWith("{LogFileName=")
                        && ((string)parameter).EndsWith("}"))
                        _logFileName = ((string)parameter).Replace("{LogFileName=", String.Empty).Replace("}", String.Empty);
                    else
                        newParameters.Add(parameter);
                }
                else
                    newParameters.Add(parameter);
            }

            _dependentMemberNames = dependentMemberNames.ToArray();
            _parameters = newParameters.ToArray();
        }

        #endregion Instance Members (Private)

    }
}
