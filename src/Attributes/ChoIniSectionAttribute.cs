using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Cinchoo.Core.Attributes
{
    public class ChoIniSectionAttribute1 : Attribute
    {
        #region Instance Data Members (Private)

        private string _iniSectionName;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Public)

        public string IniSectionName
        {
            get { return _iniSectionName; }
        }

		public string IniFilePath
		{
			get;
			set;
		}

        #endregion Instance Properties (Private)

        #region Constructors

        public ChoIniSectionAttribute1(string iniSectionName)
        {
            ChoGuard.ArgumentNotNullOrEmpty(iniSectionName, "IniSectionName");

            _iniSectionName = iniSectionName;
        }

        #endregion Constructors

        #region Shared Members (Public)

		public static Type GetType(string iniSectionName, out ChoIniSectionAttribute1 iniSectionAttribute)
        {
			iniSectionAttribute = null;

            Type[] types = ChoType.GetTypes(typeof(ChoIniSectionAttribute1));
            if (types == null || types.Length == 0) return null;

            foreach (Type type in types)
            {
                if (type == null) continue;

                ChoIniSectionAttribute1 iniSectionNameAttribute = ChoType.GetAttribute(type, typeof(ChoIniSectionAttribute1)) as ChoIniSectionAttribute1;
                if (iniSectionNameAttribute == null) continue;

				if (String.Compare(iniSectionNameAttribute.IniSectionName, iniSectionName, true) == 0)
				{
					iniSectionAttribute = iniSectionNameAttribute;
					return type;
				}
            }

            return null;
        }

        #endregion Shared Members (Public)
    }
}
