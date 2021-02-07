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
        public readonly static ChoFormatProvider Instance;
        private readonly object _padLock = new object();
        private Dictionary<Type, ICustomFormatter> _formatProviders = new Dictionary<Type, ICustomFormatter>();

        static ChoFormatProvider()
        {
            Instance = new ChoFormatProvider();
        }

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            lock (_padLock)
            {
                foreach (Type key in _formatProviders.Keys)
                {
                    if (key.IsAssignableFrom(formatType))
                        return _formatProviders[key];
                }

                return null;
            }
        }

        public void Add(Type formatType, ICustomFormatter formatter)
        {
            ChoGuard.ArgumentNotNull(formatType, "FormatType");

            lock (_padLock)
            {
                _formatProviders.AddOrUpdate(formatType, formatter);
            }
        }

        public void Remove(Type formatType)
        {
            ChoGuard.ArgumentNotNull(formatType, "FormatType");

            lock (_padLock)
            {
                _formatProviders.Delete(formatType);
            }
        }

        #endregion
    }
}
