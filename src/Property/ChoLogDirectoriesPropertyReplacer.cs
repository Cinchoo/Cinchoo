namespace eSquare.Core.Property
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using eSquare.Core.Diagnostics;
    using Cinchoo.Core;

    #endregion NameSpaces

    public sealed class ChoLogDirectoriesPropertyReplacer : IChoKeyValuePropertyReplacer
    {
        #region IChoKeyValuePropertyReplacer Members

        public bool Contains(string propertyName)
        {
            return ChoType.HasField(typeof(ChoLogDirectories), propertyName);
        }

        public string Replace(string propertyName, string format)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                return ChoObject.Format(ChoType.GetFieldValue(null, propertyName), format);
            }
             
            return propertyName;
        }

        #endregion
    }
}
