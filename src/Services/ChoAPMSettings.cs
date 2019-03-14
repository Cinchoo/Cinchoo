namespace Cinchoo.Core.Services
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;
    using Cinchoo.Core.Text;
    using System.IO;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Configuration;

    #endregion NameSpaces

    [XmlRoot("APMSettings")]
    public class ChoAPMSettings : IChoInitializable
    {
        #region Shared Data Members (Private)

        private static readonly object _padLock = new object();
        private static ChoAPMSettings _instance;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Public)

        [XmlAttribute("maxNoOfRetry")]
        public int MaxNoOfRetry;

        [XmlAttribute("sleepBetweenRetry")]
        public int SleepBetweenRetry;

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            ChoStringMsgBuilder msg = new ChoStringMsgBuilder("APM Settings");

            msg.AppendFormatLine("MaxNoOfRetry: {0}", MaxNoOfRetry);
            msg.AppendFormatLine("SleepBetweenRetry: {0}", SleepBetweenRetry);

            return msg.ToString();
        }

        #endregion Object Overrides

        #region Factory Methods

        public static ChoAPMSettings Me
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_padLock)
                {
                    if (_instance == null)
                        _instance = ChoCoreFrxConfigurationManager.Register<ChoAPMSettings>();
                }

                return _instance;
            }
        }

        #endregion Factory Methods

        #region IChoInitializable Members

        public void Initialize()
        {
            if (MaxNoOfRetry < 0)
                MaxNoOfRetry = 0;
            if (SleepBetweenRetry <= 0)
                SleepBetweenRetry = 30;
        }

        #endregion
    }
}
