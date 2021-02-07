namespace Cinchoo.Core.Shell
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ChoDefaultCommandLineArgAttribute : Attribute
    {
        #region Public Instance Properties

        internal bool IsDefaultValueSpecified { get; set; }
        internal bool IsFallbackValueSpecified { get; set; }

        public int Order
        {
            get;
            set;
        }

        public virtual bool IsRequired
        {
            get;
            set;
        }

        private string _defaultValue;
        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                IsDefaultValueSpecified = true;
                _defaultValue = value;
            }
        }

        private string _fallbackValue;
        public string FallbackValue
        {
            get { return _fallbackValue; }
            set
            {
                IsFallbackValueSpecified = true;
                _fallbackValue = value;
            }
        }

        public virtual Type SourceType { get; set; }

        public virtual string Description
        {
            get;
            set;
        }

        public virtual string ShortName
        {
            get;
            set;
        }

        private int _descriptionFormatLineSize = 60;
        public virtual int DescriptionFormatLineSize
        {
            get { return _descriptionFormatLineSize; }
            set
            {
                if (value > 0)
                    _descriptionFormatLineSize = value;
            }
        }

        private char _descriptionFormatLineBreakChar = ' ';
        public virtual char DescriptionFormatLineBreakChar
        {
            get { return _descriptionFormatLineBreakChar; }
            set { if (_descriptionFormatLineBreakChar != ChoChar.NUL) _descriptionFormatLineBreakChar = value; }
        }

        private int _noOfTabsSwitchDescFormatSeparator = 1;
        public virtual int NoOfTabsSwitchDescFormatSeparator
        {
            get { return _noOfTabsSwitchDescFormatSeparator; }
            set { if (value > 0) _noOfTabsSwitchDescFormatSeparator = value; }
        }

        #endregion Public Instance Properties
    }
}
