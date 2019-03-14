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
            _validators.AddRange(validators);
        }

        #endregion Constructors

        #region Instance Properties (Protected)

        protected IEnumerable<IChoSurrogateValidator> Validators
        {
            get { return _validators; }
        }

        public void Add(params IChoSurrogateValidator[] validators)
        {
            _validators.AddRange(validators);
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
