namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	#endregion NameSpaces

	public static class ChoHashtable
	{
		public static bool IsEquals(this Hashtable d1, Hashtable d2)
		{
			if (d1.Count != d2.Count)
				return false;

			foreach (DictionaryEntry pair in d1)
			{
				if (!d2.ContainsKey(pair.Key))
					return false;

				if (!Equals(d2[pair.Key], pair.Value))
					return false;
			}

			return true;
		}

        public static string ToXml(this IDictionary dict, string rootElementName)
        {
            IDictionary nameValues = dict as IDictionary;
            if (nameValues == null)
                return null;

            StringBuilder xmlString = new StringBuilder(String.Format("<{0}>", rootElementName));
            foreach (string key in nameValues.Keys)
            {
                if (nameValues[key] == null)
                    xmlString.AppendFormat("<add key=\"{0}\" value=\"\" />", key);
                else
                {
                    string value = nameValues[key].ToString();
                    if (value.IsNullOrWhiteSpace())
                        xmlString.AppendFormat("<add key=\"{0}\" value=\"\" />", key);
                    else if (value.ContainsXml())
                        xmlString.AppendFormat("<add key=\"{0}\"><value>{1}</value></add>", key, value);
                    else
                        xmlString.AppendFormat("<add key=\"{0}\" value=\"{1}\" />", key, value);
                }
            }
            xmlString.Append(String.Format("</{0}>", rootElementName));

            return xmlString.ToString();
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this Hashtable table)
        {
            return table
              .Cast<DictionaryEntry>()
              .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
        }
    }
}
