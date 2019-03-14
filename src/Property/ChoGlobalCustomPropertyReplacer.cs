namespace Cinchoo.Core.Property
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Cinchoo.Core.Common;
    using Cinchoo.Core.Types;

    #endregion NameSpaces

	[Serializable]
    public class ChoGlobalCustomPropertyReplacer : IChoCustomPropertyReplacer
    {
        #region IChoCustomPropertyReplacer Members

        public string Format(object target, string msg)
        {
            try
            {
                return msg;
            }
            catch (Exception ex)
            {
                return ChoPropertyManager.FormatException(msg, ex);
            }
        }

        #endregion
    }
}
