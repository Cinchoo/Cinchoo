namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Services;

    #endregion NameSpaces

    public class ChoFormatProvider : IFormatProvider
    {
        private static ChoFormatProvider _instance;
        private ChoDictionaryService<Type, ICustomFormatter> _formatProviders = new ChoDictionaryService<Type, ICustomFormatter>("FormatProviders");

        static ChoFormatProvider()
        {
            _instance = new ChoFormatProvider();
        }

        private ChoFormatProvider()
        {
        }

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            return _formatProviders.GetValue(formatType);
        }

        public void Add(Type formatType, ICustomFormatter formatter)
        {
            ChoGuard.ArgumentNotNull(formatType, "FormatType");

            _formatProviders.SetValue(formatType, formatter);
        }

        public void Remove(Type formatType)
        {
            ChoGuard.ArgumentNotNull(formatType, "FormatType");

            _formatProviders.RemoveValue(formatType);
        }

        #endregion

        #region Shared Properties

        public static ChoFormatProvider Instance
        {
            get { return _instance; }
        }

        #endregion Shared Properties
    }
}
