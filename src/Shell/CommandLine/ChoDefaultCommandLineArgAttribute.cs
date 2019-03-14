namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ChoDefaultCommandLineArgAttribute : Attribute
    {
        #region Public Instance Properties

        public bool IsRequired
        {
            get;
            set;
        }
        public object DefaultValue
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string ShortName
        {
            get;
            set;
        }

        private int _descriptionFormatLineSize = 60;
        public int DescriptionFormatLineSize
        {
            get { return _descriptionFormatLineSize; }
            set
            {
                if (value > 0)
                    _descriptionFormatLineSize = value;
            }
        }

        private char _descriptionFormatLineBreakChar = ' ';
        public char DescriptionFormatLineBreakChar
        {
            get { return _descriptionFormatLineBreakChar; }
            set { _descriptionFormatLineBreakChar = value; }
        }

        private int _descriptionFormatLineNoOfTabs = 1;
        public int DescriptionFormatLineNoOfTabs
        {
            get { return _descriptionFormatLineNoOfTabs; }
            set { _descriptionFormatLineNoOfTabs = value; }
        }

        private string _switchValueSeperator = "\t";
        public string SwitchValueSeperator
        {
            get { return _switchValueSeperator; }
            set { _switchValueSeperator = value; }
        }

        #endregion Public Instance Properties

        public object FallbackValue { get; set; }
    }
}
