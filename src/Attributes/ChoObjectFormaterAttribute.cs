namespace eSquare.Core.Attributes
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    using eSquare.Core.Text;
    using eSquare.Core.Reflection;
    using eSquare.Core.Formatters;
    using eSquare.Core.Diagnostics;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoObjectFormaterAttribute : Attribute
    {
        #region Instance Properties

        private readonly string _header;
        public string Header
        {
            get { return _header; }
        }

        #endregion Instance Properties

        #region Constructors

        public ChoObjectFormaterAttribute(string header)
        {
            _header = header;
        }

        #endregion Constructors
    }
}
