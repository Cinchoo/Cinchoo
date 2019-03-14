namespace Cinchoo.Core.Property
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Types;

    #endregion NameSpaces

    public class ChoDynamicPropertyReplacer : IChoCustomPropertyReplacer
    {
        #region Constructors

        public ChoDynamicPropertyReplacer()
        {

        }

        #endregion Constructors

        #region IChoCustomPropertyReplacer Members

        public string Format(object target, string msg)
        {
            try
            {
                return ChoString.ExpandProperties(target, msg, '{', '}', '^', new ChoCustomKeyValuePropertyReplacer(target));
            }
            catch (Exception ex)
            {
                return ChoPropertyManager.FormatException(msg, ex);
            }
        }

        #endregion

        #region IChoPropertyReplacer Members

        public string Name
        {
            get { return GetType().FullName; }
        }

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get { ; }
        }

        #endregion
    }
}
