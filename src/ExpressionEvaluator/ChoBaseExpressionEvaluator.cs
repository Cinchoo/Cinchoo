namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;

    #endregion NameSpaces

    public abstract class ChoBaseExpressionEvaluator
    {
        #region Enums

        enum ChoEvaluatorMode
        {
            Evaluate,
            ParseOnly
        }

        #endregion Enums

        #region Instance Members (Private)

        private ChoEvaluatorMode _evaluatorMode = ChoEvaluatorMode.Evaluate;
        private ChoExpressionTokenizer _tokenizer = null;

        #endregion Instance Members (Private)

        #region Instance Members (Public)

        public object Evaluate(ChoExpressionTokenizer tokenizer)
        {
            _evaluatorMode = ChoEvaluatorMode.Evaluate;
            _tokenizer = tokenizer;
            return ParseExpression();
        }

        public object Evaluate(string expr)
        {
            _tokenizer = new ChoExpressionTokenizer(expr);
            _evaluatorMode = ChoEvaluatorMode.Evaluate;
            _tokenizer.GetNextToken();

            object val = ParseExpression();
            if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.EOF)
                throw BuildParseError("Unexpected token at the end of expression", _tokenizer.CurrentPosition);

            return val;
        }

        public void CheckSyntax(string expr)
        {
            _tokenizer = new ChoExpressionTokenizer(expr);
            _evaluatorMode = ChoEvaluatorMode.ParseOnly;
            _tokenizer.GetNextToken();

            ParseExpression();
            if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.EOF)
                throw BuildParseError("Unexpected token at the end of expression", _tokenizer.CurrentPosition);
        }

        #endregion Instance Members (Public)

        #region Instance Members (Parser) (Private)

        private bool SyntaxCheckOnly()
        {
            return _evaluatorMode == ChoEvaluatorMode.ParseOnly;
        }

        private object ParseExpression()
        {
            return ParseBooleanOr();
        }

        private object ParseBooleanOr()
        {
            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object obj = ParseBooleanAnd();
            ChoEvaluatorMode oldEvaluatorMode = _evaluatorMode;
            try
            {
                while (_tokenizer.IsKeyword("or"))
                {
                    bool v1 = true;

                    if (!SyntaxCheckOnly())
                    {
                        v1 = (bool)SafeConvert(typeof(bool), obj, "the left hand side of the 'or' operator", p0, _tokenizer.CurrentPosition);

                        if (v1)
                        {
                            // we're lazy - don't evaluate anything from now, we know that the result is 'true'
                            _evaluatorMode = ChoEvaluatorMode.ParseOnly;
                        }
                    }

                    _tokenizer.GetNextToken();
                    ChoExpressionTokenizer.ChoPosition p2 = _tokenizer.CurrentPosition;
                    object o2 = ParseBooleanAnd();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        bool v2 = (bool)SafeConvert(typeof(bool), o2, "the right hand side of the 'or' operator", p2, p3);
                        obj = v1 || v2;
                    }
                }
                return obj;
            }
            finally
            {
                _evaluatorMode = oldEvaluatorMode;
            }
        }

        private object ParseBooleanAnd()
        {
            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object o = ParseRelationalExpression();
            ChoEvaluatorMode oldEvalMode = _evaluatorMode;
            try
            {
                while (_tokenizer.IsKeyword("and"))
                {
                    bool v1 = true;

                    if (!SyntaxCheckOnly())
                    {
                        v1 = (bool)SafeConvert(typeof(bool), o, "the left hand side of the 'and' operator", p0, _tokenizer.CurrentPosition);

                        if (!v1)
                        {
                            // we're lazy - don't evaluate anything from now, we know that the result is 'true'
                            _evaluatorMode = ChoEvaluatorMode.ParseOnly;
                        }
                    }

                    _tokenizer.GetNextToken();
                    ChoExpressionTokenizer.ChoPosition p2 = _tokenizer.CurrentPosition;
                    object o2 = ParseRelationalExpression();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;
                    if (!SyntaxCheckOnly())
                    {
                        bool v2 = (bool)SafeConvert(typeof(bool), o2, "the right hand side of the 'and' operator", p2, p3);

                        o = v1 && v2;
                    }
                }
                return o;
            }
            finally
            {
                _evaluatorMode = oldEvalMode;
            }
        }

        private object ParseRelationalExpression()
        {
            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object o = ParseAddSubtract();

            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.EQ
             || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.NE
             || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.LT
             || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.GT
             || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.LE
             || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.GE)
            {
                ChoExpressionTokenizer.ChoTokenType op = _tokenizer.CurrentToken;
                _tokenizer.GetNextToken();

                object o2 = ParseAddSubtract();
                ChoExpressionTokenizer.ChoPosition p2 = _tokenizer.CurrentPosition;

                if (SyntaxCheckOnly())
                {
                    return null;
                }

                switch (op)
                {
                    case ChoExpressionTokenizer.ChoTokenType.EQ:
                        if (o is string && o2 is string)
                            return o.Equals(o2);
                        else if (o is bool && o2 is bool)
                            return o.Equals(o2);
                        else if (o is int && o2 is int)
                            return o.Equals(o2);
                        else if (o is int && o2 is long)
                            return (Convert.ToInt64(o)).Equals(o2);
                        else if (o is int && o2 is double)
                            return (Convert.ToDouble(o)).Equals(o2);
                        else if (o is long && o2 is long)
                            return o.Equals(o2);
                        else if (o is long && o2 is int)
                            return (o.Equals(Convert.ToInt64(o2)));
                        else if (o is long && o2 is double)
                            return (Convert.ToDouble(o)).Equals(o2);
                        else if (o is double && o2 is double)
                            return o.Equals(o2);
                        else if (o is double && o2 is int)
                            return o.Equals(Convert.ToDouble(o2));
                        else if (o is double && o2 is long)
                            return o.Equals(Convert.ToDouble(o2));
                        else if (o is DateTime && o2 is DateTime)
                            return o.Equals(o2);
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return o.Equals(o2);
                        else if (o is Version && o2 is Version)
                            return o.Equals(o2);
                        else if (o.GetType().IsEnum)
                        {
                            if (o2 is string)
                                return o.Equals(Enum.Parse(o.GetType(), (string)o2, false));
                            else
                                return o.Equals(Enum.ToObject(o.GetType(), o2));
                        }
                        else if (o2.GetType().IsEnum)
                        {
                            if (o is string)
                                return o2.Equals(Enum.Parse(o2.GetType(), (string)o, false));
                            else
                                return o2.Equals(Enum.ToObject(o2.GetType(), o));
                        }

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '==' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);

                    case ChoExpressionTokenizer.ChoTokenType.NE:
                        if (o is string && o2 is string)
                            return !o.Equals(o2);
                        else if (o is bool && o2 is bool)
                            return !o.Equals(o2);
                        else if (o is int && o2 is int)
                            return !o.Equals(o2);
                        else if (o is int && o2 is long)
                            return !(Convert.ToInt64(o)).Equals(o2);
                        else if (o is int && o2 is double)
                            return !(Convert.ToDouble(o)).Equals(o2);
                        else if (o is long && o2 is long)
                            return !o.Equals(o2);
                        else if (o is long && o2 is int)
                            return !(o.Equals(Convert.ToInt64(o2)));
                        else if (o is long && o2 is double)
                            return !(Convert.ToDouble(o)).Equals(o2);
                        else if (o is double && o2 is double)
                            return !o.Equals(o2);
                        else if (o is double && o2 is int)
                            return !o.Equals(Convert.ToDouble(o2));
                        else if (o is double && o2 is long)
                            return !o.Equals(Convert.ToDouble(o2));
                        else if (o is DateTime && o2 is DateTime)
                            return !o.Equals(o2);
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return !o.Equals(o2);
                        else if (o is Version && o2 is Version)
                            return !o.Equals(o2);
                        else if (o.GetType().IsEnum)
                        {
                            if (o2 is string)
                                return !o.Equals(Enum.Parse(o.GetType(), (string)o2, false));
                            else
                                return !o.Equals(Enum.ToObject(o.GetType(), o2));
                        }
                        else if (o2.GetType().IsEnum)
                        {
                            if (o is string)
                                return !o2.Equals(Enum.Parse(o2.GetType(), (string)o, false));
                            else
                                return !o2.Equals(Enum.ToObject(o2.GetType(), o));
                        }

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '!=' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);

                    case ChoExpressionTokenizer.ChoTokenType.LT:
                        if (o is string && o2 is string)
                            return string.Compare((string)o, (string)o2, false,
                                CultureInfo.InvariantCulture) < 0;
                        else if (o is bool && o2 is bool)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is int && o2 is int)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is int && o2 is long)
                            return ((IComparable)Convert.ToInt64(o)).CompareTo(o2) < 0;
                        else if (o is int && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) < 0;
                        else if (o is long && o2 is long)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is long && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToInt64(o2)) < 0;
                        else if (o is long && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) < 0;
                        else if (o is double && o2 is double)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is double && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) < 0;
                        else if (o is double && o2 is long)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) < 0;
                        else if (o is DateTime && o2 is DateTime)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return ((IComparable)o).CompareTo(o2) < 0;
                        else if (o is Version && o2 is Version)
                            return ((IComparable)o).CompareTo(o2) < 0;

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '&lt;' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);

                    case ChoExpressionTokenizer.ChoTokenType.GT:
                        if (o is string && o2 is string)
                            return string.Compare((string)o, (string)o2, false,
                                CultureInfo.InvariantCulture) > 0;
                        else if (o is bool && o2 is bool)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is int && o2 is int)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is int && o2 is long)
                            return ((IComparable)Convert.ToInt64(o)).CompareTo(o2) > 0;
                        else if (o is int && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) > 0;
                        else if (o is long && o2 is long)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is long && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToInt64(o2)) > 0;
                        else if (o is long && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) > 0;
                        else if (o is double && o2 is double)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is double && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) > 0;
                        else if (o is double && o2 is long)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) > 0;
                        else if (o is DateTime && o2 is DateTime)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return ((IComparable)o).CompareTo(o2) > 0;
                        else if (o is Version && o2 is Version)
                            return ((IComparable)o).CompareTo(o2) > 0;

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '&gt;' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);

                    case ChoExpressionTokenizer.ChoTokenType.LE:
                        if (o is string && o2 is string)
                            return string.Compare((string)o, (string)o2, false,
                                CultureInfo.InvariantCulture) <= 0;
                        else if (o is bool && o2 is bool)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is int && o2 is int)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is int && o2 is long)
                            return ((IComparable)Convert.ToInt64(o)).CompareTo(o2) <= 0;
                        else if (o is int && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) <= 0;
                        else if (o is long && o2 is long)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is long && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToInt64(o2)) <= 0;
                        else if (o is long && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) <= 0;
                        else if (o is double && o2 is double)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is double && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) <= 0;
                        else if (o is double && o2 is long)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) <= 0;
                        else if (o is DateTime && o2 is DateTime)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return ((IComparable)o).CompareTo(o2) <= 0;
                        else if (o is Version && o2 is Version)
                            return ((IComparable)o).CompareTo(o2) <= 0;

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '&lt;=' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);

                    case ChoExpressionTokenizer.ChoTokenType.GE:
                        if (o is string && o2 is string)
                            return string.Compare((string)o, (string)o2, false,
                                CultureInfo.InvariantCulture) >= 0;
                        else if (o is bool && o2 is bool)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is int && o2 is int)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is int && o2 is long)
                            return ((IComparable)Convert.ToInt64(o)).CompareTo(o2) >= 0;
                        else if (o is int && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) >= 0;
                        else if (o is long && o2 is long)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is long && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToInt64(o2)) >= 0;
                        else if (o is long && o2 is double)
                            return ((IComparable)Convert.ToDouble(o)).CompareTo(o2) >= 0;
                        else if (o is double && o2 is double)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is double && o2 is int)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) >= 0;
                        else if (o is double && o2 is long)
                            return ((IComparable)o).CompareTo(Convert.ToDouble(o2)) >= 0;
                        else if (o is DateTime && o2 is DateTime)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            return ((IComparable)o).CompareTo(o2) >= 0;
                        else if (o is Version && o2 is Version)
                            return ((IComparable)o).CompareTo(o2) >= 0;

                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                            "Operator '&gt;=' cannot be applied to arguments of type '{0}' and '{1}'.",
                            ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p2);
                }
            }
            return o;
        }

        private object ParseAddSubtract()
        {
            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object o = ParseMulDiv();

            while (true)
            {
                if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Plus)
                {
                    _tokenizer.GetNextToken();
                    object o2 = ParseMulDiv();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        if (o is string && o2 is string)
                            o = (string)o + (string)o2;
                        else if (o is int && o2 is int)
                            o = (int)o + (int)o2;
                        else if (o is int && o2 is long)
                            o = (int)o + (long)o2;
                        else if (o is int && o2 is double)
                            o = (int)o + (double)o2;
                        else if (o is long && o2 is long)
                            o = (long)o + (long)o2;
                        else if (o is long && o2 is int)
                            o = (long)o + (int)o2;
                        else if (o is long && o2 is double)
                            o = (long)o + (double)o2;
                        else if (o is double && o2 is double)
                            o = (double)o + (double)o2;
                        else if (o is double && o2 is int)
                            o = (double)o + (int)o2;
                        else if (o is double && o2 is long)
                            o = (double)o + (long)o2;
                        else if (o is DateTime && o2 is TimeSpan)
                            o = (DateTime)o + (TimeSpan)o2;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            o = (TimeSpan)o + (TimeSpan)o2;
                        else
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                "Operator '+' cannot be applied to arguments of type '{0}' and '{1}'.",
                                ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p3);
                    }
                }
                else if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Minus)
                {
                    _tokenizer.GetNextToken();

                    object o2 = ParseMulDiv();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        if (o is int && o2 is int)
                            o = (int)o - (int)o2;
                        else if (o is int && o2 is long)
                            o = (int)o - (long)o2;
                        else if (o is int && o2 is double)
                            o = (int)o - (double)o2;
                        else if (o is long && o2 is long)
                            o = (long)o - (long)o2;
                        else if (o is long && o2 is int)
                            o = (long)o - (int)o2;
                        else if (o is long && o2 is double)
                            o = (long)o - (double)o2;
                        else if (o is double && o2 is double)
                            o = (double)o - (double)o2;
                        else if (o is double && o2 is int)
                            o = (double)o - (int)o2;
                        else if (o is double && o2 is long)
                            o = (double)o - (long)o2;
                        else if (o is DateTime && o2 is DateTime)
                            o = (DateTime)o - (DateTime)o2;
                        else if (o is DateTime && o2 is TimeSpan)
                            o = (DateTime)o - (TimeSpan)o2;
                        else if (o is TimeSpan && o2 is TimeSpan)
                            o = (TimeSpan)o - (TimeSpan)o2;
                        else
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                "Operator '-' cannot be applied to arguments of type '{0}' and '{1}'.",
                                ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p3);
                    }
                }
                else
                    break;
            }
            return o;
        }

        private object ParseMulDiv()
        {
            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object o = ParseValue();

            while (true)
            {
                if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Mul)
                {
                    _tokenizer.GetNextToken();

                    object o2 = ParseValue();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        if (o is int && o2 is int)
                            o = (int)o * (int)o2;
                        else if (o is int && o2 is long)
                            o = (int)o * (long)o2;
                        else if (o is int && o2 is double)
                            o = (int)o * (double)o2;
                        else if (o is long && o2 is long)
                            o = (long)o * (long)o2;
                        else if (o is long && o2 is int)
                            o = (long)o * (int)o2;
                        else if (o is long && o2 is double)
                            o = (long)o * (double)o2;
                        else if (o is double && o2 is double)
                            o = (double)o * (double)o2;
                        else if (o is double && o2 is int)
                            o = (double)o * (int)o2;
                        else if (o is double && o2 is long)
                            o = (double)o * (long)o2;
                        else
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                        "Operator '*' cannot be applied to arguments of type '{0}' and '{1}'.",
                                        ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p3);
                    }
                }
                else if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Div)
                {
                    _tokenizer.GetNextToken();

                    ChoExpressionTokenizer.ChoPosition p2 = _tokenizer.CurrentPosition;
                    object o2 = ParseValue();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        if (o is int && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o / (int)o2;
                        }
                        else if (o is int && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o / (long)o2;
                        }
                        else if (o is int && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o / (double)o2;
                        }
                        else if (o is long && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o / (long)o2;
                        }
                        else if (o is long && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o / (int)o2;
                        }
                        else if (o is long && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o / (double)o2;
                        }
                        else if (o is double && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o / (double)o2;
                        }
                        else if (o is double && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o / (int)o2;
                        }
                        else if (o is double && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o / (long)o2;
                        }
                        else
                        {
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                "Operator '/' cannot be applied to arguments of type '{0}' and '{1}'.",
                                ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p3);
                        }
                    }
                }
                else if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Mod)
                {
                    _tokenizer.GetNextToken();

                    ChoExpressionTokenizer.ChoPosition p2 = _tokenizer.CurrentPosition;
                    object o2 = ParseValue();
                    ChoExpressionTokenizer.ChoPosition p3 = _tokenizer.CurrentPosition;

                    if (!SyntaxCheckOnly())
                    {
                        if (o is int && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o % (int)o2;
                        }
                        else if (o is int && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o % (long)o2;
                        }
                        else if (o is int && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (int)o % (double)o2;
                        }
                        else if (o is long && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o % (long)o2;
                        }
                        else if (o is long && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o % (int)o2;
                        }
                        else if (o is long && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (long)o % (double)o2;
                        }
                        else if (o is double && o2 is double)
                        {
                            if ((double)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o % (double)o2;
                        }
                        else if (o is double && o2 is int)
                        {
                            if ((int)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o % (int)o2;
                        }
                        else if (o is double && o2 is long)
                        {
                            if ((long)o2 == 0)
                                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Attempt to divide by zero."), p2, p3);

                            o = (double)o % (long)o2;
                        }
                        else
                        {
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                "Operator '%' cannot be applied to arguments of type '{0}' and '{1}'.",
                                ChoType.GetTypeName(o), ChoType.GetTypeName(o2)), p0, p3);
                        }
                    }
                }
                else
                    break;
            }
            return o;
        }

        private object ParseConditional()
        {
            // we're on "if" token - skip it 
            _tokenizer.GetNextToken();
            if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.LeftParen)
                throw BuildParseError("'(' expected.", _tokenizer.CurrentPosition);

            _tokenizer.GetNextToken();


            ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
            object val = ParseExpression();
            ChoExpressionTokenizer.ChoPosition p1 = _tokenizer.CurrentPosition;

            bool cond = false;
            if (!SyntaxCheckOnly())
                cond = (bool)SafeConvert(typeof(bool), val, "the conditional expression", p0, p1);

            // skip comma between condition value and then
            if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.Comma)
                throw BuildParseError("',' expected.", _tokenizer.CurrentPosition);
            _tokenizer.GetNextToken();

            ChoEvaluatorMode oldEvaluatorMode = _evaluatorMode;

            try
            {
                if (!cond)
                {
                    // evaluate 'then' clause without executing functions
                    _evaluatorMode = ChoEvaluatorMode.ParseOnly;
                }
                else
                    _evaluatorMode = oldEvaluatorMode;

                object thenValue = ParseExpression();
                _evaluatorMode = oldEvaluatorMode;

                if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.Comma)
                    throw BuildParseError("',' expected.", _tokenizer.CurrentPosition);

                _tokenizer.GetNextToken(); // skip comma

                if (cond)
                {
                    // evaluate 'else' clause without executing functions
                    _evaluatorMode = ChoEvaluatorMode.ParseOnly;
                }
                else
                    _evaluatorMode = oldEvaluatorMode;

                object elseValue = ParseExpression();

                _evaluatorMode = oldEvaluatorMode;

                // skip closing ')'
                if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.RightParen)
                {
                    throw BuildParseError("')' expected.", _tokenizer.CurrentPosition);
                }
                _tokenizer.GetNextToken();

                return cond ? thenValue : elseValue;
            }
            finally
            {
                // restore evaluation mode - even on exceptions
                _evaluatorMode = oldEvaluatorMode;
            }
        }

        protected virtual object ParseValue()
        {
            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.String)
            {
                object v = _tokenizer.TokenText;
                _tokenizer.GetNextToken();
                string tokenText = ChoString.ExpandProperties(v as string);

                //if (tokenText == null || tokenText.Trim().Length == 0) 
                    return tokenText;
                //else
                //    return ChoString.ToObject(tokenText);
            }

            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Number)
            {
                string number = _tokenizer.TokenText;

                ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
                _tokenizer.GetNextToken();
                ChoExpressionTokenizer.ChoPosition p1 = new ChoExpressionTokenizer.ChoPosition(
                    _tokenizer.CurrentPosition.CharIndex - 1);

                if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Dot)
                {
                    number += ".";
                    _tokenizer.GetNextToken();
                    if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.Number)
                        throw BuildParseError("Fractional part expected.", _tokenizer.CurrentPosition);

                    number += _tokenizer.TokenText;

                    _tokenizer.GetNextToken();

                    p1 = _tokenizer.CurrentPosition;

                    try
                    {
                        return Double.Parse(number, CultureInfo.InvariantCulture);
                    }
                    catch (OverflowException)
                    {
                        throw BuildParseError("Value was either too large or too"
                            + " small for type 'double'.", p0, p1);
                    }
                }
                else
                {
                    try
                    {
                        return Int32.Parse(number, CultureInfo.InvariantCulture);
                    }
                    catch (OverflowException)
                    {
                        try
                        {
                            return long.Parse(number, CultureInfo.InvariantCulture);
                        }
                        catch (OverflowException)
                        {
                            throw BuildParseError("Value was either too large or too"
                                + " small for type 'long'.", p0, p1);
                        }
                    }
                }
            }

            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Minus)
            {
                _tokenizer.GetNextToken();

                // unary minus
                ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
                object v = ParseValue();
                ChoExpressionTokenizer.ChoPosition p1 = _tokenizer.CurrentPosition;
                if (!SyntaxCheckOnly())
                {
                    if (v is int)
                        return -((int)v);
                    if (v is long)
                        return -((long)v);
                    if (v is double)
                        return -((double)v);

                    throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                        "Unary minus not supported for arguments of type '{0}'.",
                        v.GetType().FullName), p0, p1);
                }
                return null;
            }

            if (_tokenizer.IsKeyword("not"))
            {
                _tokenizer.GetNextToken();

                // unary boolean not
                ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;
                object v = ParseValue();
                ChoExpressionTokenizer.ChoPosition p1 = _tokenizer.CurrentPosition;
                if (!SyntaxCheckOnly())
                {
                    bool value = (bool)SafeConvert(typeof(bool), v, "the argument of 'not' operator", p0, p1);
                    return !value;
                }
                return null;
            }

            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.LeftParen)
            {
                _tokenizer.GetNextToken();
                object v = ParseExpression();
                if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.RightParen)
                    throw BuildParseError("')' expected.", _tokenizer.CurrentPosition);

                _tokenizer.GetNextToken();
                return v;
            }

            if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Keyword)
            {
                ChoExpressionTokenizer.ChoPosition p0 = _tokenizer.CurrentPosition;

                string functionOrPropertyName = _tokenizer.TokenText;
                if (functionOrPropertyName == "if")
                    return ParseConditional();

                if (functionOrPropertyName == "true")
                {
                    _tokenizer.GetNextToken();
                    return true;
                }

                if (functionOrPropertyName == "false")
                {
                    _tokenizer.GetNextToken();
                    return false;
                }

                // don't ignore whitespace - properties shouldn't be written with spaces in them

                _tokenizer.IgnoreWhitespace = false;
                _tokenizer.GetNextToken();

                ArrayList args = new ArrayList();
                bool isFunction = false;

                // gather function or property name
                if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.DoubleColon)
                {
                    isFunction = true;
                    functionOrPropertyName += "::";
                    _tokenizer.GetNextToken();
                    if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.Keyword)
                        throw BuildParseError("Function name expected.", p0, _tokenizer.CurrentPosition);

                    functionOrPropertyName += _tokenizer.TokenText;
                    _tokenizer.GetNextToken();
                }
                else
                {
                    while (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Dot
                            || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Minus
                            || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Keyword
                            || _tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Number)
                    {
                        functionOrPropertyName += _tokenizer.TokenText;
                        _tokenizer.GetNextToken();
                    }
                }
                _tokenizer.IgnoreWhitespace = true;

                // if we've stopped on a whitespace - advance to the next token
                if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.Whitespace)
                    _tokenizer.GetNextToken();

                if (isFunction)
                {
                    if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.LeftParen)
                        throw BuildParseError("'(' expected.", _tokenizer.CurrentPosition);

                    _tokenizer.GetNextToken();

                    int currentArgument = 0;
                    ParameterInfo[] formalParameters = null;

                    try
                    {
                        formalParameters = GetFunctionParameters(functionOrPropertyName);
                    }
                    catch (Exception e)
                    {
                        throw BuildParseError(e.Message, p0, _tokenizer.CurrentPosition);
                    }

                    while (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.RightParen &&
                            _tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.EOF)
                    {
                        if (currentArgument >= formalParameters.Length)
                        {
                            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                        "Too many actual parameters for '{0}'.", functionOrPropertyName), p0, _tokenizer.CurrentPosition);
                        }

                        ChoExpressionTokenizer.ChoPosition beforeArgument = _tokenizer.CurrentPosition;
                        object e = ParseExpression();
                        ChoExpressionTokenizer.ChoPosition afterArgument = _tokenizer.CurrentPosition;

                        if (!SyntaxCheckOnly())
                        {
                            object convertedValue = SafeConvert(formalParameters[currentArgument].ParameterType,
                                    e, string.Format(CultureInfo.InvariantCulture, "argument {1} ({0}) of {2}()", formalParameters[currentArgument].Name, currentArgument + 1, functionOrPropertyName),
                                    beforeArgument, afterArgument);
                            args.Add(convertedValue);
                        }
                        currentArgument++;
                        if (_tokenizer.CurrentToken == ChoExpressionTokenizer.ChoTokenType.RightParen)
                            break;
                        if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.Comma)
                            throw BuildParseError("',' expected.", _tokenizer.CurrentPosition);

                        _tokenizer.GetNextToken();
                    }
                    if (currentArgument < formalParameters.Length)
                        throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                                    "Too few actual parameters for '{0}'.", functionOrPropertyName), p0, _tokenizer.CurrentPosition);

                    if (_tokenizer.CurrentToken != ChoExpressionTokenizer.ChoTokenType.RightParen)
                        throw BuildParseError("')' expected.", _tokenizer.CurrentPosition);

                    _tokenizer.GetNextToken();
                }

                try
                {
                    if (!SyntaxCheckOnly())
                    {
                        if (isFunction)
                            return EvaluateFunction(functionOrPropertyName, args.ToArray());
                        else
                        {
                            try
                            {
                                return EvaluateProperty(functionOrPropertyName);
                            }
                            catch (ChoFatalApplicationException)
                            {
                                throw;
                            }
                            catch
                            {
                                return functionOrPropertyName;
                            }
                        }
                    }
                    else
                        return null; // this is needed because of short-circuit evaluation
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    if (isFunction)
                        throw BuildParseError("Function call failed.", p0, _tokenizer.CurrentPosition, e);
                    else
                        throw BuildParseError("Property evaluation failed.", p0, _tokenizer.CurrentPosition, e);
                }
            }

            return UnexpectedToken();
        }

        protected ChoExpressionParseException BuildParseError(string desc, ChoExpressionTokenizer.ChoPosition p0)
        {
            return new ChoExpressionParseException(desc, p0.CharIndex);
        }

        protected ChoExpressionParseException BuildParseError(string desc, ChoExpressionTokenizer.ChoPosition p0, ChoExpressionTokenizer.ChoPosition p1)
        {
            return new ChoExpressionParseException(desc, p0.CharIndex, p1.CharIndex);
        }

        protected ChoExpressionParseException BuildParseError(string desc, ChoExpressionTokenizer.ChoPosition p0, ChoExpressionTokenizer.ChoPosition p1, Exception ex)
        {
            return new ChoExpressionParseException(desc, p0.CharIndex, p1.CharIndex, ex);
        }

        protected object SafeConvert(Type returnType, object source, string description, ChoExpressionTokenizer.ChoPosition p0, ChoExpressionTokenizer.ChoPosition p1)
        {
            try
            {
                //
                // TODO - Convert.ChangeType() is very liberal. It allows you to convert "true" to Double (1.0).
                // We shouldn't allow this. Add more cases like this here.
                //
                bool disallow = false;

                if (source == null)
                {
                    if (returnType == typeof(string))
                        return string.Empty;

                    throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                        "Cannot convert {0} to '{1}' (value was null).",
                        description, returnType.FullName), p0, p1);
                }

                if (source is bool)
                {
                    if (returnType != typeof(string) && returnType != typeof(bool))
                    {
                        // boolean can only be converted to string or boolean
                        disallow = true;
                    }
                }

                if (returnType == typeof(bool))
                {
                    if (!(source is string || source is bool))
                    {
                        // only string and boolean can be converted to boolean
                        disallow = true;
                    }
                }

                if (source is DateTime)
                {
                    if (returnType != typeof(string) && returnType != typeof(DateTime))
                    {
                        // DateTime can only be converted to string or DateTime
                        disallow = true;
                    }
                }

                if (returnType == typeof(DateTime))
                {
                    if (!(source is DateTime || source is string))
                    {
                        // only string and DateTime can be converted to DateTime
                        disallow = true;
                    }
                }

                if (source is TimeSpan && returnType != typeof(TimeSpan))
                {
                    // implicit conversion from TimeSpan is not supported, as
                    // TimeSpan does not implement IConvertible
                    disallow = true;
                }

                if (returnType == typeof(TimeSpan) && !(source is TimeSpan))
                {
                    // implicit conversion to TimeSpan is not supported
                    disallow = true;
                }

                if (returnType == typeof(string))
                {
                    if (source is DirectoryInfo)
                        return ((DirectoryInfo)source).FullName;
                    else if (source is FileInfo)
                        return ((FileInfo)source).FullName;
                }

                if (returnType.IsEnum)
                {
                    if (source is string)
                        return Enum.Parse(returnType, (string)source, false);
                    else
                        return Enum.ToObject(returnType, source);
                }

                if (disallow)
                {
                    throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                        "Cannot convert {0} to '{1}' (actual type was '{2}').",
                        description, ChoType.GetTypeName(returnType),
                        ChoType.GetTypeName(source)), p0, p1);
                }

                return Convert.ChangeType(source, returnType, CultureInfo.InvariantCulture);
            }
            catch (ChoExpressionParseException)
            {
                throw;
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                    "Cannot convert {0} to '{1}' (actual type was '{2}').",
                    description, ChoType.GetTypeName(returnType),
                    ChoType.GetTypeName(source)), p0, p1, ex);
            }
        }

        #endregion Parser

        #region Overridables

        protected abstract object EvaluateFunction(string functionName, object[] args);
        protected abstract ParameterInfo[] GetFunctionParameters(string functionName);
        protected abstract object EvaluateProperty(string propertyName);

        protected virtual object UnexpectedToken()
        {
            throw BuildParseError(string.Format(CultureInfo.InvariantCulture,
                "Unexpected token '{0}'.", _tokenizer.CurrentToken), _tokenizer.CurrentPosition);
        }

        #endregion Overridables
    }
}
