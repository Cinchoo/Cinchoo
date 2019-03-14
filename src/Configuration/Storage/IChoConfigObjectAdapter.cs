namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using System.Collections.Specialized;

    #endregion

    public interface IChoConfigObjectAdapter
    {
        void Init(ChoBaseConfigurationElement configElement, IEnumerable<Tuple<string, string>> parameters);
        DateTime LastUpdateDateTime
        {
            get;
        }
    }

    public interface IChoDictionaryConfigObjectAdapter : IChoConfigObjectAdapter
    {
        IDictionary GetData();
        void Save(IDictionary dict);
    }

    //public interface IChoNameValueConfigObjectAdapter : IChoConfigObjectAdapter
    //{
    //    NameValueCollection GetData();
    //    void Save(NameValueCollection dict);
    //}
}
