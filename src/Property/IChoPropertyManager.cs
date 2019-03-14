namespace eSquare.Core.Property
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoPropertyManager
    {
        string Replace(string inString);
    }
}
