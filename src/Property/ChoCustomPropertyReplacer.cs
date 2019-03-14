namespace Cinchoo.Core.Property
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using Cinchoo.Core.Types;
    using Cinchoo.Core.Interfaces;
    using Cinchoo.Core.Attributes;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Formatters;
    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Common;
using Cinchoo.Core.Diagnostics.Attributes;

    #endregion NameSpaces

    [Serializable]
	[ChoBufferProfile("Loading custom property replacers....", NameFromTypeFullName = typeof(ChoCustomPropertyReplacer), StartActions = "Truncate")]
    public class ChoCustomPropertyReplacer : IChoCustomPropertyReplacer
    {
        #region Instance Data Members (Private)

        private readonly string _name;
        private readonly ChoCustomPropertyReplaceHandler _customPropertyReplaceHandler;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCustomPropertyReplacer()
        {
        }

        public ChoCustomPropertyReplacer(ChoCustomPropertyReplaceHandler customPropertyReplaceHandler)
            : this(String.Format("CustomPropertyReplacer_{0}", ChoRandom.NextRandom().ToString()), customPropertyReplaceHandler)
        {
        }

        public ChoCustomPropertyReplacer(string name, ChoCustomPropertyReplaceHandler customPropertyReplaceHandler)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(customPropertyReplaceHandler, "CustomPropertyReplaceHandler");

            _name = name;
            _customPropertyReplaceHandler = customPropertyReplaceHandler;
        }

        #endregion Constructors

        #region IChoCustomPropertyReplacer Members

        public string Format(object target, string msg)
        {
            if (_customPropertyReplaceHandler == null) return msg;
            return _customPropertyReplaceHandler(target, msg);
        }

        #endregion

        #region IChoPropertyReplacer Members

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties()
        {
            return null;
        }

        #endregion
    }
}
