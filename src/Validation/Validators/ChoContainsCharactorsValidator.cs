namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;

    #endregion NameSpaces

    public sealed class ChoContainsCharactersValidator : ChoValidatorGeneric<string>
    {
        private string _characterSet;
        private bool _containsAnyCharacters = true;

        public ChoContainsCharactersValidator(string characterSet)
            : this(false, characterSet, true)
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoContainsCharactersValidator(bool negated, string characterSet, bool containsAnyCharacters)
            : base(negated)
        {
            _characterSet = characterSet;
            _containsAnyCharacters = containsAnyCharacters;
        }

        // Summary:
        //     Determines whether an object can be validated based on type.
        //
        // Parameters:
        //   type:
        //     The object type.
        //
        // Returns:
        //     true if the type parameter value matches the expected type; otherwise, false.
        public override bool CanValidate(Type type)
        {
            if (type == typeof(string))
                return true;
            else
                return false;
        }

        protected override string GetErrMsg()
        {
            return String.Format("The value must{0}contains {1} of the '{2}' characters.",
                Negated ? " NOT " : " ", _containsAnyCharacters ? "ANY": "ALL", _characterSet);
        }

        //
        // Summary:
        //     Determines whether the value of an object is valid.
        //
        // Parameters:
        //   value:
        //     The object value.
        public override void Validate(string objectToValidate)
        {
            if (objectToValidate != null)
            {
                if (_containsAnyCharacters)
                {
                    List<char> characterSetArray = new List<char>(_characterSet);
                    bool containsCharacterFromSet = false;
                    foreach (char ch in objectToValidate)
                    {
                        if (characterSetArray.Contains(ch))
                        {
                            containsCharacterFromSet = true;
                            break;
                        }

                    }

                    if ((containsCharacterFromSet && Negated) 
                        || (!containsCharacterFromSet && !Negated))
                        throw new ChoValidationException(GetErrMsg());
                }
                else
                {
                    List<char> objectToValidateArray = new List<char>(objectToValidate);
                    bool containsAllCharactersFromSet = true;
                    foreach (char ch in _characterSet)
                    {
                        if (!objectToValidateArray.Contains(ch))
                        {
                            containsAllCharactersFromSet = false;
                        }
                    }

                    if ((containsAllCharactersFromSet && Negated)
                        || (!containsAllCharactersFromSet && !Negated))
                        throw new ChoValidationException(GetErrMsg());
                }
            }
        }
    }
}
