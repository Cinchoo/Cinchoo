namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using Cinchoo.Core.Common;
    using Cinchoo.Core;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.Collections;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Collections.Generic;

    #endregion NameSpaces

    public class ChoDictionaryPropertyReplacer : IChoKeyValuePropertyReplacer
    {
        #region Instance Data Members (Private)

        private readonly string _name;
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();
        private readonly IDictionary<string, string> _propertyDescriptions = new Dictionary<string, string>();

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoDictionaryPropertyReplacer(IDictionary<string, object> properties)
            : this(String.Format("DictionaryPropertyReplacer_{0}", ChoRandom.NextRandom().ToString()), properties)
        {
        }

        public ChoDictionaryPropertyReplacer(string name, IDictionary<string, object> properties)
            : this(name, properties, null)
        {
        }

        public ChoDictionaryPropertyReplacer(string name, IDictionary<string, object> properties, IDictionary<string, string> propertyDescriptions)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(properties, "properties");

            _name = name;
            _properties = properties;
            _propertyDescriptions = propertyDescriptions;
        }

        #endregion Constructors

        #region IChoPropertyReplacer Members

        public string ReplaceProperty(string propertyName, string format, object context)
        {
            if (String.IsNullOrEmpty(propertyName)) return propertyName;

            return ChoObject.Format(propertyName, format);
        }

        public bool ContainsProperty(string propertyName, object context)
        {
            return _properties.ContainsKey(propertyName);
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
                foreach (string propertyName in _properties.Keys)
                    yield return new KeyValuePair<string, string>(propertyName, null);
            }
        }

        #endregion

        #region IChoKeyValuePropertyReplacer Members


        public string GetPropertyDescription(string propertyName)
        {
            if (_propertyDescriptions != null && _propertyDescriptions.ContainsKey(propertyName))
                return _propertyDescriptions[propertyName];
            else
                return null;
        }

        #endregion
    }
}
