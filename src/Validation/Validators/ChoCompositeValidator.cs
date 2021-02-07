using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Cinchoo.Core
{
    public abstract class ChoCompositeValidator : IChoSurrogateValidator
    {
        #region Instance Data Members (Protected)

        private List<IChoSurrogateValidator> _validators = new List<IChoSurrogateValidator>();

        #endregion Instance Data Members (Private)
    
        #region Constructors

        public ChoCompositeValidator(params IChoSurrogateValidator[] validators)
        {
            Add(validators);
        }

        #endregion Constructors

        #region Instance Properties (Protected)

        internal string Name
        {
            get;
            set;
        }

        internal ChoCompositeValidator Parent
        {
            get;
            set;
        }

        protected IEnumerable<IChoSurrogateValidator> Validators
        {
            get { return _validators; }
        }

        public void Add(params IChoSurrogateValidator[] validators)
        {
            if (validators == null) return;

            foreach (IChoSurrogateValidator val in validators)
            {
                if (val is ChoCompositeValidator)
                {
                    if (IsCircularReferenceFound(val as ChoCompositeValidator)) continue;
                    ((ChoCompositeValidator)val).Parent = this;
                }

                _validators.AddRange(validators);
            }
        }

        private bool IsCircularReferenceFound(ChoCompositeValidator val)
        {
            ChoCompositeValidator parent = val.Parent;
            while (parent != null)
            {
                if (parent == val)
                    return true;
                parent = val.Parent;
            }

            return false;
        }

        public void Clear()
        {
            _validators.Clear();
        }

        #endregion Instance Properties (Protected)

        #region IChoSurrogateValidator Members

        public abstract void Validate(MemberInfo memberInfo, object value);

        public void Validate(MemberInfo memberInfo, object value, ChoValidationResults validationResults)
        {
            try
            {
                Validate(memberInfo, value);
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                validationResults.AddResult(new ChoValidationResult(ex.Message));
            }
        }

        #endregion
    }
}
