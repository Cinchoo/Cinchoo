using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.IO
{
    #region Enum

    public static partial class ChoReservedFileExt
    {
        #region Constants (Public)

        //TODO: If you add new ext, please visit IsValidExtension method, add the correponding ext there.
        public const string Err = "err";
        public const string Log = "log";
        public const string Config = "config";
        public const string Expn = "expn";
        public const string Ignore = "ignore";
		public const string MetaData = "meta";
		public const string Xml = "xml";
        public const string Txt= "txt";
        public const string Perf = "perf";
        public const string Cho = "cho";
        public const string ETL = "etl";

        #endregion Constants (Public)

        #region Shared Members (Public)

        public static bool IsValidExtension(string extension)
        {
            ChoGuard.ArgumentNotNullOrEmpty(extension, "Extension");
            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            switch (extension)
            {
                case Err:
                case Log:
                case Config:
                case Expn:
                case Ignore:
                case MetaData:
                case Xml:
                case Txt:
                case Perf:
                    return true;
                default:
                    return false;
            }
        }

        #endregion Shared Members (Public)
    }

    public partial class ChoReservedDirectoryName
    {
        #region Constants

        public const string Caches = "Caches";
        public const string Settings = "Settings";
        public const string Others = "Others";
        public const string Errors = "Errors";
        public const string Logs = "Logs";
        public const string Meta = "Meta";
        public const string Config = "Config";

        #endregion Constants
    }

    public partial class ChoReservedFileName
    {
        #region Constants

        public const string AllFailedCacheLookup = "AllFailedCacheLookup";
        public const string Bcp = "Bcp";
        public const string SerializationIssues = "SerializationIssues";
        public const string ConfigurationErrors = "ConfigurationErrors";
        public const string CoreFrxConfigFileName = "ChoCoreFrx.xml";

        #endregion Constants
    }

    #endregion
}
