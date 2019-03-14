namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Xml;

    #endregion NameSpaces

    #region IConfigXmlNode Interface

    interface IConfigXmlNode
    {
        string Filename { get; }
        int LineNumber { get; }
    }

    #endregion

    [Serializable]
    public class ChoConfigurationException : ChoApplicationException
    {
        #region Instance Data Members (Private)

        // Fields		
        private string _bareMessage;
        private string _filename;
        private int _line;

        #endregion

        //
        // Constructors
        //
        public ChoConfigurationException()
            : base("There is an error in a configuration setting.")
        {
            _filename = null;
            _bareMessage = "There is an error in a configuration setting.";
            _line = 0;
        }

        public ChoConfigurationException(string message)
            : base(message)
        {
            _bareMessage = message;
        }

        protected ChoConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _filename = info.GetString("filename");
            _line = info.GetInt32("line");
        }

        public ChoConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
            _bareMessage = message;
        }

        public ChoConfigurationException(string message, XmlNode node)
            : base(message)
        {
            _filename = GetXmlNodeFilename(node);
            _line = GetXmlNodeLineNumber(node);
            _bareMessage = message;
        }

        public ChoConfigurationException(string message, Exception inner, XmlNode node)
            : base(message, inner)
        {
            _filename = GetXmlNodeFilename(node);
            _line = GetXmlNodeLineNumber(node);
            _bareMessage = message;
        }

        public ChoConfigurationException(string message, string _filename, int _line)
            : base(message)
        {
            _bareMessage = message;
            this._filename = _filename;
            this._line = _line;
        }

        public ChoConfigurationException(string message, Exception inner, string _filename, int _line)
            : base(message)
        {
            _bareMessage = message;
            this._filename = _filename;
            this._line = _line;
        }
        //
        // Properties
        //
        public string BareMessage
        {
            get { return _bareMessage; }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public int Line
        {
            get { return _line; }
        }

        public override string Message
        {
            get
            {
                string baseMsg = base.Message;
                string f = (_filename == null) ? String.Empty : _filename;
                string l = (_line == 0) ? String.Empty : (" line " + _line);

                return baseMsg + " (" + f + l + ")";
            }
        }

        //
        // Methods
        //

        public static string GetXmlNodeFilename(XmlNode node)
        {
            if (!(node is IConfigXmlNode))
                return String.Empty;

            return ((IConfigXmlNode)node).Filename;
        }

        public static int GetXmlNodeLineNumber(XmlNode node)
        {
            if (!(node is IConfigXmlNode))
                return 0;

            return ((IConfigXmlNode)node).LineNumber;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("filename", _filename);
            info.AddValue("line", _line);
        }
    }
}
