namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoExpressionParseException : ChoApplicationException
    {
        #region Instance Data Members (Private)

        private int _startPosition = -1;
        private int _endPosition = -1;

        #endregion Instance Data Members (Private)

        #region Instance Properties (Public)

        public int StartPosition
        {
            get { return _startPosition; }
        }

        public int EndPosition
        {
            get { return _endPosition; }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoExpressionParseException() : base() { }
        public ChoExpressionParseException(string message) : base(message, null) { }
        public ChoExpressionParseException(string message, Exception inner) : base(message, inner) { }

        public ChoExpressionParseException(string message, int startPosition)
            : base(message, null)
        {
            _startPosition = startPosition;
            _endPosition = -1;
        }

        public ChoExpressionParseException(string message, int startPosition, int endPosition)
            : base(message, null)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        }

        public ChoExpressionParseException(string message, int startPosition, int endPosition, Exception inner)
            : base(message, inner)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        }

        #endregion Constructors

        #region Serializable Members

        protected ChoExpressionParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _startPosition = (int)info.GetValue("startPosition", typeof(int));
            _endPosition = (int)info.GetValue("endPosition", typeof(int));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("startPosition", _startPosition);
            info.AddValue("endPosition", _endPosition);

            base.GetObjectData(info, context);
        }

        #endregion Serializable Members
    }
}
