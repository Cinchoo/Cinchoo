namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.ComponentModel;
    using System.Text;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ChoMemberFormatterAttribute : ChoObjectNameableAttribute
    {
        #region Instance Properties

        private IChoMemberFormatter _formatter;
        public Type Formatter
        {
            get { throw new NotImplementedException(); }
            set 
            {
                if (value != null)
                {
                    if (typeof(IChoMemberFormatter).IsAssignableFrom(value))
                        _formatter = ChoObjectManagementFactory.CreateInstance(value) as IChoMemberFormatter;
                }
            }
        }

        private TypeConverter _typeConverter;
        public Type TypeConverter
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (value != null)
                {
                    if (typeof(TypeConverter).IsAssignableFrom(value))
                        _typeConverter = ChoObjectManagementFactory.CreateInstance(value) as TypeConverter;
                }
            }
        }

        private IFormatProvider _formaterProvider;
        public Type FormatProvider
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (value != null)
                {
                    if (typeof(IFormatProvider).IsAssignableFrom(value))
                        _formaterProvider = ChoObjectManagementFactory.CreateInstance <IFormatProvider>(value);
                }
            }
        }

        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private int _noOfNewLinesBefore = 0;
        public int NoOfNewLinesBefore
        {
            get { return _noOfNewLinesBefore; }
            set
            {
                if (value >= 0)
                    _noOfNewLinesBefore = value;
            }
        }

        private int _noOfNewLinesAfter = 0;
        public int NoOfNewLinesAfter
        {
            get { return _noOfNewLinesAfter; }
            set
            {
                if (value >= 0)
                    _noOfNewLinesAfter = value;
            }
        }

        private int _noOfTabs = 0;
        public int NoOfTabs
        {
            get { return _noOfTabs; }
            set 
            { 
                if (value >= 0)
                    _noOfTabs = value; 
            }
        }

        private bool _forceFormat = false;
        public bool ForceFormat
        {
            get { return _forceFormat; }
            set { _forceFormat = value; }
        }

        #endregion

        #region Constructors

        public ChoMemberFormatterAttribute()
        {
        }

        public ChoMemberFormatterAttribute(int noOfTabs)
            : this(null, noOfTabs)
        {
        }

        public ChoMemberFormatterAttribute(string name) : base(name)
        {
        }

        public ChoMemberFormatterAttribute(string name, int noOfTabs) : base(name)
        {
            NoOfTabs = noOfTabs;
        }

        #endregion

        #region Instance Members (Public)
        
        internal string PostFormat(string name, string value)
        {
            if (Name != null && Name.Length > 0)
                name = Name;
            else if (name != null && name.Length > 0)
            {
            }
            else
                name = "key";

            StringBuilder msg = new StringBuilder();

            for (int index = 0; index < NoOfNewLinesBefore; index++)
                msg.AppendLine();

			if (name == ChoNull.NullString)
				msg.AppendFormat("{0}{1}", ChoString.ToString(value), Environment.NewLine);
			else
			{
				string valueText = ChoString.ToString(value);

				if (!valueText.ContainsHeader())
					msg.AppendFormat("{0}: {1}{2}", name, valueText, Environment.NewLine);
				else
					msg.AppendFormat("{0}{1}", valueText, Environment.NewLine);
			}

            for (int index = 0; index < NoOfNewLinesAfter; index++)
                msg.AppendLine();

            return msg.ToString().Indent(NoOfTabs);
        }

        internal bool CanFormat()
        {
            return _typeConverter != null || _formatter != null || _formaterProvider != null || !_format.IsNullOrEmpty();
        }

        internal string FormatObject(object value)
        {
            string msg = null;
            if (value != null)
            {
                if (_typeConverter != null && _typeConverter.CanConvertTo(typeof(string)))
                    msg = (string)_typeConverter.ConvertTo(value, typeof(string));
                else if (_formatter != null && (ForceFormat || _formatter.CanFormat(value.GetType())))
                    msg = _formatter.Format(value, false);
                else if (_formaterProvider != null)
                    msg = String.Format(_formaterProvider, String.Format("{{0:{0}}}", _format), value);
                else if (!_format.IsNullOrEmpty())
                    msg = String.Format(String.Format("{{0:{0}}}", _format), value);
            }

            return msg;
        }

        #endregion Instance Members (Public)
    }
}
