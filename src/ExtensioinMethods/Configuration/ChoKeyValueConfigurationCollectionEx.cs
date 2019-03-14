namespace System.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.Specialized;

    #endregion NameSpaces

    public static class ChoKeyValueConfigurationCollectionEx
    {
        public static NameValueCollection ToNameValueCollection(this KeyValueConfigurationCollection keyValueCollection)
        {
            if (keyValueCollection == null) return null;

            NameValueCollection nameValueCollection = new NameValueCollection();
            foreach (string key in keyValueCollection.AllKeys)
            {
                if (keyValueCollection[key] != null)
                    nameValueCollection.Add(key, keyValueCollection[key].Value);
            }

            return nameValueCollection;
        }
    }
}
