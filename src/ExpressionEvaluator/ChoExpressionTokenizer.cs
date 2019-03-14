namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Globalization;
    using System.ComponentModel;

    #endregion NameSpaces

    /// <summary>
    /// Splits an input string into a sequence of tokens used during parsing.
    /// </summary>
    public class ChoExpressionTokenizer : IEnumerable
    {
        #region ChoPositon Struct

        public struct ChoPosition
        {
            private int _charIndex;

            public ChoPosition(int charIndex)
            {
                _charIndex = charIndex;
            }

            public int CharIndex
            {
                get { return _charIndex; }
            }
        }

        #endregion ChoPosition Struct

        #region Enums

        /// <summary>
        /// Available tokens
        /// </summary>
        public enum ChoTokenType
        {
            [Description("BOF")]
            BOF,
            [Description("EOF")]
            EOF,
            [Description("Number")]
            Number,
            [Description("String")]
            String,
            [Description("Keyword")]
            Keyword,
            [Description("==")]
            EQ,
            [Description("!=")]
            NE,
            [Description("<")]
            LT,
            [Description(">")]
            GT,
            [Description("<=")]
            LE,
            [Description(">=")]
            GE,
            [Description("+")]
            Plus,
            [Description("-")]
            Minus,
            [Description("*")]
            Mul,
            [Description("/")]
            Div,
            [Description("%")]
            Mod,
            [Description("(")]
            LeftParen,
            [Description(")")]
            RightParen,
            [Description("{")]
            LeftCurlyBrace,
            [Description("}")]
            RightCurlyBrace,
            [Description("!")]
            Not,
            [Description("Punctuation")]
            Punctuation,
            [Description("Whitespace")]
            Whitespace,
            [Description("$")]
            Dollar,
            [Description(",")]
            Comma,
            [Description(".")]
            Dot,
            [Description("::")]
            DoubleColon,
            [Description(":")]
            SingleColon,
            [Description("'")]
            SingleQuote,
            [Description("\"")]
            DoubleQuote,
        }

        #endregion Enums

        #region Private Instance Fields

        private string _text = null;
        private int _position;
        private ChoPosition _tokenStartPosition;
        private ChoTokenType _tokenType;
        private string _tokenText;
        private bool _ignoreWhiteSpace = true;
        private bool _singleCharacterMode = false;

        #endregion Private Instance Fields

        #region Private Static Fields

        private static ChoCharToTokenType[] _charToTokenType = {
            new ChoCharToTokenType('+', ChoTokenType.Plus),
            new ChoCharToTokenType('-', ChoTokenType.Minus),
            new ChoCharToTokenType('*', ChoTokenType.Mul),
            new ChoCharToTokenType('/', ChoTokenType.Div),
            new ChoCharToTokenType('%', ChoTokenType.Mod),
            new ChoCharToTokenType('<', ChoTokenType.LT),
            new ChoCharToTokenType('>', ChoTokenType.GT),
            new ChoCharToTokenType('(', ChoTokenType.LeftParen),
            new ChoCharToTokenType(')', ChoTokenType.RightParen),
            new ChoCharToTokenType('{', ChoTokenType.LeftCurlyBrace),
            new ChoCharToTokenType('}', ChoTokenType.RightCurlyBrace),
            new ChoCharToTokenType('!', ChoTokenType.Not),
            new ChoCharToTokenType('$', ChoTokenType.Dollar),
            new ChoCharToTokenType(',', ChoTokenType.Comma),
            new ChoCharToTokenType('.', ChoTokenType.Dot),
        };

        private static ChoTokenType[] charIndexToTokenType = new ChoTokenType[128];

        #endregion Private Static Fields

        #region Static Constructor

        static ChoExpressionTokenizer()
        {
            for (int index = 0; index < 128; ++index)
            {
                charIndexToTokenType[index] = ChoTokenType.Punctuation;
            };

            foreach (ChoCharToTokenType cht in _charToTokenType)
            {
                charIndexToTokenType[(int)cht.ch] = cht.tokenType;
            }
        }

        #endregion Static Constructor

        #region Constructors

        public ChoExpressionTokenizer(string exprString)
        {
            _text = exprString;
            Initialize();
        }

        #endregion Constructors

        #region Public Instance Properties

        public bool IgnoreWhitespace
        {
            get { return _ignoreWhiteSpace; }
            set { _ignoreWhiteSpace = value; }
        }

        public bool SingleCharacterMode
        {
            get { return _singleCharacterMode; }
            set { _singleCharacterMode = value; }
        }

        public ChoTokenType CurrentToken
        {
            get { return _tokenType; }
        }

        public string TokenText
        {
            get { return _tokenText; }
        }

        public ChoPosition CurrentPosition
        {
            get { return _tokenStartPosition; }
        }

        #endregion Public Instance Properties

        #region Public Instance Methods

        public void Initialize()
        {
            //_text = exprString;
            _position = 0;
            _tokenType = ChoTokenType.BOF;

            //GetNextToken();
        }

        public void GetNextToken()
        {
            if (_tokenType == ChoTokenType.EOF)
                throw new ChoExpressionParseException("Cannot read past end of stream.");

            if (IgnoreWhitespace)
                SkipWhitespace();

            _tokenStartPosition = new ChoPosition(_position);

            int nextChar = PeekChar();
            if (nextChar == -1)
            {
                _tokenType = ChoTokenType.EOF;
                return;
            }

            char ch = (char)nextChar;

            if (!SingleCharacterMode)
            {
                #region Handle WhiteSpace chars

                if (!IgnoreWhitespace && Char.IsWhiteSpace(ch))
                {
                    StringBuilder sb = new StringBuilder();
                    int ch2;

                    while ((ch2 = PeekChar()) != -1)
                    {
                        if (!Char.IsWhiteSpace((char)ch2))
                        {
                            break;
                        }

                        sb.Append((char)ch2);
                        ReadChar();
                    };

                    _tokenType = ChoTokenType.Whitespace;
                    _tokenText = sb.ToString();
                    return;
                }

                #endregion Handle WhiteSpace chars

                #region Handle Digits

                if (Char.IsDigit(ch))
                {
                    _tokenType = ChoTokenType.Number;
                    
                    StringBuilder sb = new StringBuilder();
                    //sb.Append(ch);

                    while ((nextChar = PeekChar()) != -1)
                    {
                        ch = (char)nextChar;

                        if (!Char.IsDigit(ch))
                            break;

                        sb.Append(ch);
                        ReadChar();
                    };

                    _tokenText = sb.ToString();
                    return;
                }

                #endregion Handle Digits

                #region Handle String in quotes

                if (ch == '\'')
                {
                    _tokenType = ChoTokenType.String;

                    StringBuilder sb = new StringBuilder();
                    ReadChar();
                    while ((nextChar = ReadChar()) != -1)
                    {
                        ch = (char)nextChar;

                        if (ch == '\'')
                        {
                            if (PeekChar() == (int)'\'')
                                ReadChar();
                            else
                                break;
                        }
                        sb.Append(ch);
                    };

                    _tokenText = sb.ToString();
                    return;
                }

                #endregion Handle String in quotes

                #region Handle Keywords

                if (ch == '_' || Char.IsLetter(ch))
                {
                    _tokenType = ChoTokenType.Keyword;

                    StringBuilder sb = new StringBuilder();

                    sb.Append((char)ch);

                    ReadChar();

                    while ((nextChar = PeekChar()) != -1)
                    {
                        if ((char)nextChar == '_' || (char)nextChar == '-' || Char.IsLetterOrDigit((char)nextChar))
                            sb.Append((char)ReadChar());
                        else
                            break;
                    };

                    _tokenText = sb.ToString();
                    if (_tokenText.EndsWith("-"))
                        throw new ChoExpressionParseException(String.Format(CultureInfo.InvariantCulture,
                            "Identifier cannot end with a dash: {0}", _tokenText), CurrentPosition.CharIndex);
                    return;
                }

                #endregion Handle Keywords

                ReadChar();

                if (ch == ':' && PeekChar() == (int)':')
                {
                    _tokenType = ChoTokenType.DoubleColon;
                    _tokenText = "::";
                    ReadChar();
                    return;
                }

                if (ch == '!' && PeekChar() == (int)'=')
                {
                    _tokenType = ChoTokenType.NE;
                    _tokenText = "!=";
                    ReadChar();
                    return;
                }

                if (ch == '=' && PeekChar() == (int)'=')
                {
                    _tokenType = ChoTokenType.EQ;
                    _tokenText = "==";
                    ReadChar();
                    return;
                }

                if (ch == '<' && PeekChar() == (int)'=')
                {
                    _tokenType = ChoTokenType.LE;
                    _tokenText = "<=";
                    ReadChar();
                    return;
                }

                if (ch == '>' && PeekChar() == (int)'=')
                {
                    _tokenType = ChoTokenType.GE;
                    _tokenText = ">=";
                    ReadChar();
                    return;
                }
            }
            else
            {
                ReadChar();
            }
            _tokenText = new String(ch, 1);
            _tokenType = ChoTokenType.Punctuation;
            if (ch >= 32 && ch < 128)
            {
                _tokenType = charIndexToTokenType[ch];
            }
        }

        public bool IsKeyword(string keyword)
        {
            return (_tokenType == ChoTokenType.Keyword) && (_tokenText == keyword);
        }

        #endregion Public Instance Methods

        #region Private Instance Methods

        private int ReadChar()
        {
            return _position < _text.Length ? _text[_position++] : -1;
        }

        private int PeekChar()
        {
            return _position < _text.Length ? _text[_position] : -1;
        }

        private void SkipWhitespace()
        {
            int ch;

            while ((ch = PeekChar()) != -1)
            {
                if (!Char.IsWhiteSpace((char)ch))
                    break;
                ReadChar();
            };
        }

        #endregion Private Instance Methods

        #region ChoCharToTokenType Struct

        private struct ChoCharToTokenType
        {
            public readonly char ch;
            public readonly ChoTokenType tokenType;

            public ChoCharToTokenType(char ch, ChoTokenType tokenType)
            {
                this.ch = ch;
                this.tokenType = tokenType;
            }
        }

        #endregion ChoCharToTokenType Struct

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new ChoExpressionTokenizerEnumerator(this);
        }

        #endregion

        public class ChoExpressionTokenizerState
        {
            #region Instance Data Members (Private)

            private ChoTokenType _currentToken;
            private string _tokenText;
            private ChoPosition _currentPosition;

            #endregion Instance Data Members (Private)

            #region Constructors

            public ChoExpressionTokenizerState(ChoTokenType currentToken, string tokenText, ChoPosition currentPosition)
            {
                _currentToken = currentToken;
                _tokenText = tokenText;
                _currentPosition = currentPosition;
            }

            #endregion Constructors

            #region Instance Properties (Public)

            public ChoTokenType CurrentToken
            {
                get { return _currentToken; }
            }

            public string TokenText
            {
                get { return _tokenText; }
            }

            public ChoPosition CurrentPosition
            {
                get { return _currentPosition; }
            }

            #endregion Instance Properties (Public)
        }

        #region ChoExpressionTokenizerEnumerator Class

        [Serializable]
        private class ChoExpressionTokenizerEnumerator : IEnumerator, ICloneable
        {
            // Fields
            private ChoExpressionTokenizer _expressionTokenizer;
            private ChoExpressionTokenizerState _currentElement;

            // Methods
            internal ChoExpressionTokenizerEnumerator(ChoExpressionTokenizer expressionTokenizer)
            {
                _expressionTokenizer = expressionTokenizer;
                _expressionTokenizer.Initialize();
            }

            public object Clone()
            {
                return base.MemberwiseClone();
            }

            public virtual bool MoveNext()
            {
                _expressionTokenizer.GetNextToken();
                if (_expressionTokenizer.CurrentToken != ChoTokenType.EOF)
                {
                    _currentElement = new ChoExpressionTokenizerState(_expressionTokenizer.CurrentToken,
                        _expressionTokenizer.TokenText, _expressionTokenizer.CurrentPosition);
                    return true;
                }
                else
                {
                    _currentElement = null;
                    return false;
                }
            }

            public virtual void Reset()
            {
                _expressionTokenizer.Initialize();
                _currentElement = null;
            }

            // Properties
            public virtual object Current
            {
                get
                {
                    if (_currentElement != null)
                        return _currentElement;

                    throw new InvalidOperationException();
                }
            }
        }

        #endregion ChoExpressionTokenizerEnumerator Class
    }
}
