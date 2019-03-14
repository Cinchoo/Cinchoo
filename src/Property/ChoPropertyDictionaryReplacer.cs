namespace eSquare.Core.Property
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Types;
    using eSquare.Core.Common;
    using eSquare.Core.Formatters;
    using eSquare.Core.Interfaces;
    using eSquare.Core.Attributes;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Collections;
    using eSquare.Core.Configuration;
    using eSquare.Core.Collections.Generic;

    #endregion NameSpaces

    [Serializable]
    public class ChoPropertyNameValue
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("value")]
        public string Value;
    }

    [Serializable]
    public class ChoPropertyDictionaryReplacer : ChoObjConfigurable, IChoPropertyReplacer, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlElement("nameValue", typeof(ChoPropertyNameValue))]
        public ChoPropertyNameValue[] ConfigNameValues;

        #endregion Instance Data Members (Public)

        #region Instance Data Members (Private)

        private IDictionary<string, object> _properties = new Dictionary<string, object>();
        public IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoPropertyDictionaryReplacer()
        {
        }

        internal ChoPropertyDictionaryReplacer(IDictionary<string, object> properties)
        {
            Name = String.Format("PropertyDictionaryReplacer_{0}", ChoRandom.NextRandom().ToString());
            Priority = 0;
            _properties = properties;
        }

        public ChoPropertyDictionaryReplacer(string name, int priority, IDictionary<string, object> properties)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(properties, "properties");

            Name = name;
            Priority = priority;
            _properties = properties;
        }

        #endregion Constructors

        #region IChoPropertyReplacer Members

        public string Replace(string token, string format)
        {
            return token;
        }

        public bool Contains(string token)
        {
            return false;
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeFieldInit)
        {
            if (ConfigNameValues == null || ConfigNameValues.Length == 0) return;
            if (String.IsNullOrEmpty(Name))
            {
                ChoProfile.WriteLine("Missing Name");
                return;
            }

            using (ChoBufferProfile profile = ChoBufferProfile.DelayedAutoStart(new ChoBufferProfile(true, Name, "Loading property dictionary...")))
            {
                foreach (ChoPropertyNameValue configNameValue in ConfigNameValues)
                {
                    if (configNameValue == null) continue;
                    if (String.IsNullOrEmpty(configNameValue.Name)) continue;

                    try
                    {
                        _properties.Add(configNameValue.Name, ChoString.ToObject(configNameValue.Value));
                    }
                    catch (Exception ex)
                    {
                        profile.AppendLine(String.Format("{0}: {1}", configNameValue.Name, ex.ToString()));
                    }
                }
            }
            ConfigNameValues = null;
        }

        #endregion
    }
}
