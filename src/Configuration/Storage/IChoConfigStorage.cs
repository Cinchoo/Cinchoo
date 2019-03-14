namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Xml;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public interface IChoConfigStorage : IDisposable
    {
        bool IsConfigSectionDefined { get; }
        object Load(ChoBaseConfigurationElement configElement, XmlNode node);
        void Persist(object data, ChoDictionaryService<string, object> stateInfo);
        bool CanPersist(object data, ChoDictionaryService<string, object> stateInfo);
        IChoConfigurationChangeWatcher ConfigurationChangeWatcher
        {
            get;
        }

        object PersistableState { get; }

        void PostLoad(ChoBaseConfigurationElement configElement);
    }
}
