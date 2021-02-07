namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Configuration;
    using System.Collections.Generic;

    using Cinchoo.Core.Resources;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ChoValidatorAttribute : ChoMemberInfoAttribute
    {
        #region Instance Properties (Public)

        private bool _negated = false;
        public bool Negated
        {
            get { return _negated; }
            set { _negated = value; }
        }

        private Type _validator;
        public Type ValidatorType
        {
            get { return _validator; }
        }

        private object[] _constructorArgs;
        public object[] ConstructorArgs
        {
            get { return _constructorArgs; }
            set { _constructorArgs = value; }
        }

        public string ConstructorArgsText
        {
            get { throw new NotSupportedException(); }
            set
            {
                if (value.IsNullOrWhiteSpace()) return;

                List<object> args = new List<object>();
                foreach (string arg in value.SplitNTrim())
                {
                    if (arg.IsNullOrEmpty())
                        args.Add(null);
                    else if (arg.IsNullOrWhiteSpace())
                        args.Add(arg);
                    else
                        args.Add(arg.Evaluate());
                }

                ConstructorArgs = args.ToArray();
            }
        }

        public string KeyValuePropertiesText
        {
            get;
            set;
        }

        //private bool _forceValidate = false;
        //public bool ForceValidate
        //{
        //    get { return _forceValidate; }
        //    set { _forceValidate = value; }
        //}

        #endregion Instance Properties (Public)

        #region Constructors

        protected ChoValidatorAttribute() : base()
        {
        }

        protected ChoValidatorAttribute(bool negated)
        {
            _negated = negated;
        }

        public ChoValidatorAttribute(Type validator, bool negated = false)
            : this(negated)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");
            if (!typeof(ChoValidator).IsAssignableFrom(validator) &&
                !typeof(ConfigurationValidatorBase).IsAssignableFrom(validator))
                throw new ArgumentException("Validator object should be type of ChoValidatorBase/ConfigurationValidatorBase.");
            
            _validator = validator;
        }

        #endregion Constructors

        #region IChoBeforeMemberCallAttribute Members

        // Summary:
        //     Gets the validator attribute instance.
        //
        // Returns:
        //     The current System.Configuration.ConfigurationValidatorBase.
        public virtual object ValidatorInstance 
        { 
            get 
            {
                object validatorBase;
                if (_constructorArgs == null || _constructorArgs.Length == 0)
                    validatorBase = ChoType.CreateInstanceWithReflectionPermission(_validator);
                else
                    validatorBase = ChoType.CreateInstanceWithReflectionPermission(_validator, ConstructorArgs);

                if (validatorBase is ChoValidator)
                    ((ChoValidator)validatorBase).Negated = Negated;
                //validatorBase.ForceValidate = ForceValidate;

                SetPropertyValues(validatorBase);

                return validatorBase;
            }
        }

        private void SetPropertyValues(object validator)
        {
            if (KeyValuePropertiesText.IsNullOrWhiteSpace()) return;

            foreach (Tuple<string, string> tuple in KeyValuePropertiesText.ToKeyValuePairs())
            {
                if (tuple.Item2.IsNullOrEmpty()) continue;

                if (tuple.Item2.IsNullOrWhiteSpace())
                    ChoType.SetMemberValue(validator, tuple.Item1, tuple.Item2);
                else
                    ChoType.SetMemberValue(validator, tuple.Item1, tuple.Item2.Evaluate());
            }
        }

        #endregion
    }
}
