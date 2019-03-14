namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    using Cinchoo.Core.Text;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoTypeFormatterAttribute : Attribute
    {
        #region Instance Properties

        private readonly string _header;
        public string Header
        {
            get { return _header; }
        }

        private IFormatProvider _formaterProvider;
        public Type FormatProviderType
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (value != null)
                {
                    if (typeof(IFormatProvider).IsAssignableFrom(value))
                        _formaterProvider = ChoObjectManagementFactory.CreateInstance<IFormatProvider>(value);
                }
            }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoTypeFormatterAttribute(string header)
        {
            _header = header;
        }

        #endregion Constructors

        #region Instance Members (Internal)

        internal bool HasFormatSpecified
        {
            get { return _formaterProvider != null || !Format.IsNullOrEmpty(); }
        }

        internal string FormatObject(object target)
        {
            string msg = null;
            if (target != null)
            {
                if (_formaterProvider != null)
                    msg = String.Format(_formaterProvider, String.Format("{{0:{0}}}", _format), target);
                else if (!_format.IsNullOrEmpty())
                    msg = String.Format(String.Format("{{0:{0}}}", _format), target);
            }

            return msg;
        }

        #endregion Instance Members (Internal)
    }
}
