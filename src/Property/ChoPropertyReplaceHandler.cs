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

    #endregion NameSpaces

    [Serializable]
    public class ChoPropertyReplaceHandler1 : ChoObjConfigurable, IChoPropertyManager, IChoObjectInitializable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("type")]
        public string TypeName;

        [XmlAttribute("method")]
        public string Method;

        #endregion Instance Data Members (Public)

        #region Instance Data Members (Private)

        private ChoPropertyReplaceHandler _propertyReplaceHandler;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoPropertyReplaceHandler1()
        {
        }

        public ChoPropertyReplaceHandler1(string name, int priority, ChoPropertyReplaceHandler propertyReplaceHandler)
        {
            ChoGuard.ArgumentNotNull(name, "name");
            ChoGuard.ArgumentNotNull(propertyReplaceHandler, "PropertyReplaceHandler");

            Name = name;
            Priority = priority;
            _propertyReplaceHandler = propertyReplaceHandler;
        }

        #endregion Constructors

        #region IChoPropertyManager Members

        public string Replace(string inString)
        {
            if (_propertyReplaceHandler == null) return inString;

            return ChoString.Replace(inString, _propertyReplaceHandler);
        }

        #endregion

        #region IChoObjectInitializable Members

        public void Initialize(bool beforeInit)
        {
            if (String.IsNullOrEmpty(Name))
            {
                ChoProfile.WriteLine("Missing Name");
                return;
            }

            using (ChoBufferProfile profile = ChoBufferProfile.DelayedAutoStart(new ChoBufferProfile(true, Name, "Loading property handler...")))
            {
                try
                {
                    _propertyReplaceHandler = new ChoCallbackObj(TypeName, Method).CreateDelegate<ChoPropertyReplaceHandler>() as ChoPropertyReplaceHandler;
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
