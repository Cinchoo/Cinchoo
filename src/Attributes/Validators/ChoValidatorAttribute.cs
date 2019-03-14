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

        private object[] _args;
        public object[] Args
        {
            get { return _args; }
            set { _args = value; }
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

        public ChoValidatorAttribute(bool negated, Type validator) : this(negated)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");
            if (!typeof(ChoValidator).IsAssignableFrom(validator))
                throw new ArgumentException("Validator object should be type of ChoValidatorBase.");
            
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
                ChoValidator validatorBase;
                if (_args == null || _args.Length == 0)
                    validatorBase = (ChoValidator)ChoType.CreateInstanceWithReflectionPermission(_validator);
                else
                    validatorBase = (ChoValidator)ChoType.CreateInstanceWithReflectionPermission(_validator, Args);

                validatorBase.Negated = Negated;
                //validatorBase.ForceValidate = ForceValidate;

                return validatorBase;
            }
        }

        #endregion
    }
}
