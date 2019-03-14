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
    using eSquare.Core.Interfaces;
    using eSquare.Core.Attributes;
    using eSquare.Core.Configuration;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Formatters;
    using eSquare.Core.Collections;
    using eSquare.Core.Collections.Generic;
    using eSquare.Core.Common;
    using eSquare.Core.Diagnostics.Attributes;

    #endregion NameSpaces

    #region Delegates

    public delegate string ChoCustomPropertyReplaceHandler(object format, string msg);

    #endregion Delegates

    [Serializable]
    [ChoBufferProfile("Loading custom property handler replacers....", FileNameFromTypeFullName = typeof(ChoCustomPropertyHandlerReplacer), Mode = ChoProfileMode.Truncate)]
    public class ChoCustomPropertyHandlerReplacer : ChoObjConfigurable, IChoPropertyReplacer, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("method")]
        public string Method;

        #endregion Instance Data Members (Public)

        #region Instance Data Members (Private)

        private ChoCustomPropertyReplaceHandler _customPropertyReplaceHandler;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCustomPropertyHandlerReplacer()
        {
        }

        internal ChoCustomPropertyHandlerReplacer(ChoCustomPropertyReplaceHandler customPropertyReplaceHandler)
        {
            Name = String.Format("CustomPropertyHandlerReplacer_{0}", ChoRandom.NextRandom().ToString());
            Priority = 0;
            _customPropertyReplaceHandler = customPropertyReplaceHandler;
        }

        public ChoCustomPropertyHandlerReplacer(string name, int priority, ChoCustomPropertyReplaceHandler customPropertyReplaceHandler)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(customPropertyReplaceHandler, "CustomPropertyReplaceHandler");

            Name = name;
            Priority = priority;
            _customPropertyReplaceHandler = customPropertyReplaceHandler;
        }

        #endregion Constructors

        #region IChoPropertyReplacer Members

        public string Replace(string token, string format)
        {
            if (_customPropertyReplaceHandler == null) return token;

            return token;
        }

        public bool Contains(string token)
        {
            return false;
        }

        #endregion

        #region IChoObjectInitializable Members

        private bool _initialized = false;
        public void Initialize(bool beforeFieldInit)
        {
            if (_initialized) return;
            _initialized = true;

            if (String.IsNullOrEmpty(Name))
            {
                ChoProfile.WriteLine("Missing Name");
                return;
            }

            using (ChoBufferProfile profile = ChoBufferProfile.DelayedAutoStart(new ChoBufferProfile(true, Name, "Loading property handler...")))
            {
                try
                {
                    _customPropertyReplaceHandler = new ChoCallbackObj(Type, Method).CreateDelegate<ChoCustomPropertyReplaceHandler>() as ChoCustomPropertyReplaceHandler;
                }
                catch (Exception ex)
                {
                    profile.AppendLine(String.Format("{0}: {1}", Name, ex.ToString()));
                }
            }
        }

        #endregion
    }
}
