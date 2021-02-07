namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Common;
using System.Linq;
using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [ChoDictionaryConfigurationSection("sequenceGeneratorSettings")]
    public class ChoSequenceGeneratorSettings : ChoConfigurableObject
    {
        [ChoPropertyInfo("seed", DefaultValue = "1")]
        public long Seed;

        [ChoPropertyInfo("step", DefaultValue = "1")]
        public int Step;

        protected override void OnAfterConfigurationObjectLoaded()
        {
            if (Step == 0)
                Step = 1;

            _next = Seed;
        }

        private readonly object _padLock = new object();
        private long _next;

        public long Next()
        {
            lock (_padLock)
            {
                long next = _next;
                _next = _next + Step;
                return next;
            }
        }
    }

	[Serializable]
    public class ChoGlobalDictionaryPropertyReplacer : IChoKeyValuePropertyReplacer
    {
        #region Instance Data Members (Private)

        private readonly Dictionary<string, string> _availPropeties = new Dictionary<string,string>()
            {
                { "APPLICATION_NAME", "Current application name." },
                { "PROCESS_ID", "Current process identifier." },
                { "THREAD_ID", "Current thread identifier." },
                { "THREAD_NAME", "Current thread name." },
                { "RANDOM_NO", "Random number." },
                { "TODAY", "Today's date." },
                { "NOW", "Current time." },
                { "SEQ_NO", "Next sequence number." }
            };

        private readonly Lazy<ChoSequenceGeneratorSettings> _sequenceGeneratorSettings =
            new Lazy<ChoSequenceGeneratorSettings>(() => new ChoSequenceGeneratorSettings());

        #endregion Instance Data Members (Private)

        #region IChoPropertyReplacer Members

        public bool ContainsProperty(string propertyName, object context)
        {
            return _availPropeties.ContainsKey(propertyName);
        }

        public string ReplaceProperty(string propertyName, string format, object context)
        {
            if (String.IsNullOrEmpty(propertyName)) return propertyName;

            switch (propertyName)
            {
                case "APPLICATION_NAME":
                    return ChoObject.Format(ChoGlobalApplicationSettings.Me.ApplicationName, format);
                case "PROCESS_ID":
                    return ChoObject.Format(ChoApplication.ProcessId, format);
                case "THREAD_ID":
					return ChoObject.Format(ChoApplication.GetThreadId(), format);
                case "THREAD_NAME":
					return ChoObject.Format(ChoApplication.GetThreadName(), format);
                case "RANDOM_NO":
                    return ChoObject.Format(ChoRandom.NextRandom(), format);
                case "TODAY":
                    if (String.IsNullOrEmpty(format))
                        return System.DateTime.Today.ToShortDateString();
                    else
                        return ChoObject.Format(System.DateTime.Today, format);
                case "NOW":
                    if (String.IsNullOrEmpty(format))
                        return System.DateTime.Now.ToShortTimeString();
                    else
                        return ChoObject.Format(System.DateTime.Now, format);
                case "SEQ_NO":
                    return ChoObject.Format(_sequenceGeneratorSettings.Value.Next(), format);
                default:
                    return ChoObject.Format(propertyName, format);
            }
        }

        #endregion

        #region IChoPropertyReplacer Members

        public string Name
        {
            get { return this.GetType().FullName; }
        }

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get
            {
                foreach (KeyValuePair<string, string> keyValue in _availPropeties)
                    yield return keyValue;
            }
        }

        public string GetPropertyDescription(string propertyName)
        {
            if (_availPropeties.ContainsKey(propertyName))
                return _availPropeties[propertyName];
            else
                return null;
        }

        #endregion
    }
}
