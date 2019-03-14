namespace Cinchoo.Core
{
    #region NameSpaces
    
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Cinchoo.Core.Reflection;
    using System.Reflection;

    #endregion NameSpaces

    public class ChoExpressionEvaluator : ChoBaseExpressionEvaluator
    {
        #region Constants

        internal const string THIS = "this";

        #endregion Constants

        #region Instance Data Members (Private)

        private Stack<string> _visiting;
        private Dictionary<string, object> _state;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoExpressionEvaluator()
        {
            _visiting = new Stack<string>();
            _state = new Dictionary<string, object>();
        }

        public ChoExpressionEvaluator(Stack<string> visiting, Dictionary<string, object> state)
        {
            _visiting = visiting;
            _state = state;
        }

        #endregion Constructors

        #region ChoBaseExpressionEvaluator Overrides

        private object GetTargetState()
        {
            return _state.ContainsKey(THIS) ? _state[THIS] : null;
        }

        protected override object EvaluateFunction(string functionName, object[] args)
        {
            object target = GetTargetState();

            string[] functionParts = functionName.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            string typeName = functionParts[0];
            if (String.Compare(typeName, THIS, true) == 0)
                return ChoType.InvokeMethod(target, functionParts[1], args);
            else
            {
                Type objType = ChoType.GetType(ChoTypeFactory.Me[typeName]);
                if (objType == null)
                    throw new ChoExpressionParseException(String.Format("Can't find {0} type.", typeName));
                return ChoType.InvokeMethod(objType, functionParts[1], args);
            }
        }

        protected override System.Reflection.ParameterInfo[] GetFunctionParameters(string functionName)
        {
            object target = GetTargetState();
            
            string[] functionParts = functionName.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            string typeName = functionParts[0];
            if (String.Compare(typeName, THIS, true) == 0)
            {
                //if (typeName != THIS) return propertyName;
                MethodInfo methodInfo = target.GetType().GetMethod(functionParts[1]);

                if (methodInfo == null)
                    throw new ChoExpressionParseException(String.Format("Can't find {0} instance method in {1} type.", functionParts[1], target.GetType().FullName));

                return methodInfo.GetParameters();
            }
            else
            {
                Type objType = ChoType.GetType(ChoTypeFactory.Me[typeName]);
                if (objType == null)
                    throw new ChoExpressionParseException(String.Format("Can't find {0} type.", typeName));

                MethodInfo methodInfo = objType.GetMethod(functionParts[1]);

                if (methodInfo == null)
                    throw new ChoExpressionParseException(String.Format("Can't find {0} instance method in {1} type.", functionParts[1], objType.FullName));

                return methodInfo.GetParameters();
            }
        }

        protected override object EvaluateProperty(string propertyName)
        {
            return GetPropertyValue(propertyName);
        }

        #endregion

        #region Instance Members (Private)

        private const string _matchString = @"(?<class>this|[\w+\.\$]+)\.(?<member>\w+)[\,]*\s*(?<assembly>[\w+\.\$]*)";
        private object GetPropertyValue(string propertyName)
        {
            object target = GetTargetState();

            Match match = Regex.Match(propertyName, _matchString);
            if (!match.Success) return propertyName;

            string className = match.Groups["class"].ToString();
            string memberName = match.Groups["member"].ToString();
            string assemblyName = match.Groups["assembly"].ToString();

            if (String.Compare(className, THIS, true) == 0)
            {
                if (className != THIS || target == null) return propertyName;

                //Call the object member, return the value
                return ChoType.GetMemberValue(target.GetType(), target, memberName);
            }
            else
            {
                //Call the object shared member, return the value
                Type objType = null;

                string typeName = null;
                try
                {
                    if (String.IsNullOrEmpty(assemblyName))
                        typeName = className;
                    else
                        typeName = String.Format("{0}, {1}", className, assemblyName);

                    objType = ChoType.GetType(typeName);

                    if (objType == null)
                    {
                        string typeName1 = ChoTypeFactory.Me[typeName];
                        if (!String.IsNullOrEmpty(typeName1))
                            objType = ChoType.GetType(typeName1);
                    }
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(String.Format("Failed to load {0} type.", typeName), ex);
                }
                if (objType == null)
                    throw new ApplicationException(String.Format("Failed to load {0} type.", typeName));

                return ChoType.GetMemberValue(objType, null, memberName);
            }
        }

        #endregion Instance Members (Private)
    }
}
