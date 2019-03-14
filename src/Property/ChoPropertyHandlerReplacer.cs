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

    public delegate string ChoPropertyReplaceHandler(string token, string format);

    #endregion Delegates

    [Serializable]
    [ChoBufferProfile("Loading property handler replacers....", FileNameFromTypeName = typeof(ChoPropertyHandlerReplacer), Mode = ChoProfileMode.Truncate)]
    public class ChoPropertyHandlerReplacer : ChoObjConfigurable, IChoPropertyReplacer, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("method")]
        public string Method;

        #endregion Instance Data Members (Public)

        #region Instance Data Members (Private)

        private ChoPropertyReplaceHandler _propertyReplaceHandler;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoPropertyHandlerReplacer()
        {
        }

        internal ChoPropertyHandlerReplacer(ChoPropertyReplaceHandler propertyReplaceHandler)
        {
            Name = String.Format("PropertyHandlerReplacer_{0}", ChoRandom.NextRandom().ToString());
            Priority = 0;
            _propertyReplaceHandler = propertyReplaceHandler;
        }

        public ChoPropertyHandlerReplacer(string name, int priority, ChoPropertyReplaceHandler propertyReplaceHandler)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(propertyReplaceHandler, "PropertyReplaceHandler");

            Name = name;
            Priority = priority;
            _propertyReplaceHandler = propertyReplaceHandler;
        }

        #endregion Constructors

        #region IChoPropertyReplacer Members

        public string Replace(string token, string format)
        {
            if (_propertyReplaceHandler == null) return token;

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

            ChoProfile.WriteLine("Missing Name");
            if (String.IsNullOrEmpty(Name))
            {
                ChoProfile.WriteLine("Missing Name");
                return;
            }

            using (ChoBufferProfile profile = ChoBufferProfile.DelayedAutoStart(new ChoBufferProfile(true, Name, "Loading property handler...")))
            {
                try
                {
                    _propertyReplaceHandler = new ChoCallbackObj(Type, Method).CreateDelegate<ChoPropertyReplaceHandler>() as ChoPropertyReplaceHandler;
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
