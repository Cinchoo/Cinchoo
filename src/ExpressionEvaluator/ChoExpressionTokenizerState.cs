namespace eSquare.Core.ExpressionEvaluator
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoExpressionTokenizerState
    {
        #region Instance Data Members (Private)

        private eSquare.Core.ExpresssionEvaluator.ChoExpressionTokenizer.ChoTokenType _currentToken;
        private string _tokenText;
        private eSquare.Core.ExpresssionEvaluator.ChoExpressionTokenizer.ChoPosition _currentPosition;

        #endregion Instance Data Members (Private)

        #region Constructors

        internal ChoExpressionTokenizerState(eSquare.Core.ExpresssionEvaluator.ChoExpressionTokenizer.ChoTokenType currentToken, string tokenText, ChoPosition currentPosition)
        {
            _currentToken = currentToken;
            _tokenText = tokenText;
            _currentPosition = currentPosition;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public eSquare.Core.ExpresssionEvaluator.ChoExpressionTokenizer.ChoTokenType CurrentToken
        {
            get { return _currentToken; }
        }

        public string TokenText
        {
            get { return _tokenText; }
        }

        public eSquare.Core.ExpresssionEvaluator.ChoExpressionTokenizer.ChoPosition CurrentPosition
        {
            get { return _currentPosition; }
        }

        #endregion Instance Properties (Public)
    }
}
