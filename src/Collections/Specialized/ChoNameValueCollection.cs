namespace Cinchoo.Core.Collections.Specialized
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using Cinchoo.Core.Ini;
    using Cinchoo.Core;
    using Cinchoo.Core.Configuration;
    using System.Text.RegularExpressions;

    #endregion NameSpaces

    public class ChoNameValueCollection : NameValueCollection
    {
        #region Instance Data Members (Private)

        private readonly ChoNameValueCollectionSettings _nameValueCollectionSettings;

        #endregion

        #region Constructors

        public ChoNameValueCollection()
        {
        }

        public ChoNameValueCollection(NameValueCollection c)
            : base(c)
        {
        }

        public ChoNameValueCollection(string keyValuePairs) : this(keyValuePairs, ChoNameValueCollectionSettings.Me)
        {
        }

        public ChoNameValueCollection(string keyValuePairs, ChoNameValueCollectionSettings nameValueCollectionSettings)
        {
            if (keyValuePairs == null || keyValuePairs.Length == 0)
                return;

            if (nameValueCollectionSettings == null)
                throw new ArgumentNullException("nameValueCollectionSettings");

            _nameValueCollectionSettings = nameValueCollectionSettings;

            ChoStringJoinerSettings stringJoinerSettings = new ChoStringJoinerSettings("|", @"\s*[", @"]\s*");
            string nameValuePairSeparator = new ChoStringJoiner(stringJoinerSettings).Join(_nameValueCollectionSettings.NameValuePairSeparator.ToCharArray());
            string nameValueSeparator = new ChoStringJoiner(stringJoinerSettings).Join(_nameValueCollectionSettings.NameValueSeparator.ToCharArray());

            string regExString = String.Format(@"(?<param>\w+){0}(?<value>[^{2}]*){1}|(?<param>\w+){0}(?<value>[^{2}]*)",
                nameValueSeparator, nameValuePairSeparator, _nameValueCollectionSettings.NameValueSeparator);

            Regex spliter = new Regex(regExString, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (Match match in spliter.Matches(keyValuePairs))
                Add(match.Groups["param"].ToString(), match.Groups["value"].ToString());
        }

        #endregion Constructors

        #region Instance Members (Public)

        public void MergeWith(ChoNameValueCollection nameValueCollection)
        {
            MergeWith(nameValueCollection as NameValueCollection);
        }

        public void MergeWith(NameValueCollection nameValueCollection)
        {
            if (nameValueCollection == null) return;

            foreach (string key in nameValueCollection.AllKeys)
                this[key] = nameValueCollection[key];
        }

        #endregion Instance Members (Public)

        #region Shared Members (Public)

        public static ChoNameValueCollection Merge(ChoNameValueCollection collection1, ChoNameValueCollection collection2)
        {
            ChoNameValueCollection nameValueCollection = new ChoNameValueCollection();
            nameValueCollection.MergeWith(collection1);
            nameValueCollection.MergeWith(collection2);

            return nameValueCollection;
        }

        public static NameValueCollection Merge(NameValueCollection collection1, NameValueCollection collection2)
        {
            ChoNameValueCollection nameValueCollection = new ChoNameValueCollection();
            nameValueCollection.MergeWith(collection1);
            nameValueCollection.MergeWith(collection2);

            return nameValueCollection as NameValueCollection;
        }

        public static NameValueCollection ToNameValueCollection(ChoIniSectionNode iniSectionNode)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
			if (iniSectionNode != null)
			{
				foreach (ChoIniNode iniNode in iniSectionNode)
				{
					if (!(iniNode is ChoIniNameValueNode))
						continue;
					nameValueCollection.Add(((ChoIniNameValueNode)iniNode).Name, ((ChoIniNameValueNode)iniNode).Value);
				}
			}

            return nameValueCollection;
        }

        #endregion Shared Members (Public)
    }
}
