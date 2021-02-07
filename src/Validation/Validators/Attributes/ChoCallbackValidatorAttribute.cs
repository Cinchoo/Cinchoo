namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ChoCallbackValidatorAttribute : ChoValidatorAttribute
    {
        // Fields
        private ValidatorCallback _callbackMethod;
        private string _callbackMethodName = string.Empty;
        private Type _type;

        // Properties
        public string CallbackMethodName
        {
            get { return _callbackMethodName; }
            set
            {
                _callbackMethodName = value;
                _callbackMethod = null;
            }
        }

        public Type Type
        {
            get { return _type; }
            set
            {
                _type = value;
                _callbackMethod = null;
            }
        }

        public override object ValidatorInstance
        {
            get
            {
                if (_callbackMethod == null)
                {
                    if (_type == null)
                    {
                        throw new ArgumentNullException("Type");
                    }
                    if (!string.IsNullOrEmpty(_callbackMethodName))
                    {
                        MethodInfo method = _type.GetMethod(_callbackMethodName, BindingFlags.Public | BindingFlags.Static);
                        if (method != null)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            if ((parameters.Length == 1) && (parameters[0].ParameterType == typeof(object)))
                            {
                                _callbackMethod = (ValidatorCallback)Delegate.CreateDelegate(typeof(ValidatorCallback), method);
                            }
                        }
                    }
                }
                if (_callbackMethod == null)
                {
                    throw new ArgumentException(ConfigResources.GetString("Validator_method_not_found", new object[] { _callbackMethodName }));
                }
                return new CallbackValidator(_type, _callbackMethod);
            }
        }
    }
}
