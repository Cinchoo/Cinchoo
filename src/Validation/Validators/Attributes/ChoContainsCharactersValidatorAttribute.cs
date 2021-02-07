namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false, AllowMultiple = true)]
    public sealed class ChoContainsCharactersValidatorAttribute : ChoValidatorAttribute
    {
        #region Instance Properties

        private string _characterSet;
        public string CharacterSet
        {
            get { return _characterSet; }
        }

        private bool _containsAnyCharacters;
        public bool ContainsAnyCharacters
        {
            get { return _containsAnyCharacters; }
            set { _containsAnyCharacters = value; }
        }

        #endregion Instance Properties

        #region Constructors

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="NotNullValidator"/>.</para>
        /// </summary>
        public ChoContainsCharactersValidatorAttribute(string characterSet) : this(false, characterSet)
        {
        }

        public ChoContainsCharactersValidatorAttribute(bool negated, string characterSet)
            : base(negated)
        {
            _characterSet = characterSet;
        }

        #endregion Constructors

        // Properties
        public override object ValidatorInstance
        {
            get { return new ChoContainsCharactersValidator(Negated, CharacterSet, ContainsAnyCharacters); }
        }
    }
}
