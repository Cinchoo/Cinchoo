namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;

    using Cinchoo.Core;

    #endregion NameSpaces

    public class ChoCustomKeyValuePropertyReplacer : IChoKeyValuePropertyReplacer
    {
        #region Instance Data Members (Private)

        private readonly string _name;
        private readonly IChoKeyValuePropertyReplacer _keyValuePropertyReplacer;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCustomKeyValuePropertyReplacer(IChoKeyValuePropertyReplacer keyValuePropertyReplacer)
            : this(String.Format("CustomKeyValuePropertyReplacer_{0}", ChoRandom.NextRandom().ToString()), keyValuePropertyReplacer)
        {
        }

        public ChoCustomKeyValuePropertyReplacer(string name, IChoKeyValuePropertyReplacer keyValuePropertyReplacer)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(keyValuePropertyReplacer, "CustomKeyValuePropertyReplacer");

            _name = name;
            _keyValuePropertyReplacer = keyValuePropertyReplacer;
        }

        #endregion Constructors

        #region IChoKeyValuePropertyReplacer Members

        public bool ContainsProperty(string propertyName, object context)
        {
            ChoGuard.ArgumentNotNull(_keyValuePropertyReplacer, "KeyValuePropertyReplacer");

            return _keyValuePropertyReplacer.ContainsProperty(propertyName, context);
        }

        public string ReplaceProperty(string propertyName, string format, object context)
        {
            ChoGuard.ArgumentNotNull(_keyValuePropertyReplacer, "KeyValuePropertyReplacer");

            return _keyValuePropertyReplacer.ReplaceProperty(propertyName, format);
        }

        #endregion

        #region IChoPropertyReplacer Members

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get
            {
                return null;
            }
        }

        public string GetPropertyDescription(string propertyName)
        {
            return _keyValuePropertyReplacer.GetPropertyDescription(propertyName);
        }

        #endregion
    }
}
