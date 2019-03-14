namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ValidationScope { Before, After }
    public enum ValidatorSource { Attribute, Configuration }

    public interface IChoValidationManager
    {
        bool IsValid(Attribute attribute);
        IChoSurrogateValidator CreateValidator(Attribute attribute, ValidationScope validationScope, ValidatorSource validatorSource);
    }
}
