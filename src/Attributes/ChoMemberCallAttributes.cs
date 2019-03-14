namespace eSquare.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Configuration;
    using System.Collections.Generic;

    using eSquare.Core.Resources;
    using eSquare.Core.Reflection;

    #endregion NameSpaces

    public interface IChoBeforeMemberCallAttribute
    {
        //void Validate(object value, bool silent);
    }

    public interface IChoAfterMemberCallAttribute
    {
        //void Validate(object value, bool silent);
    }
}
