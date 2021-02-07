namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Cinchoo.Core.Common;

    #endregion NameSpaces

	public class ChoGlobalExpressionEvaluatorReplacer : IChoCustomPropertyReplacer
    {
        #region IChoCustomPropertyReplacer Members

        public bool Format(object target, ref string msg)
        {
            //try
            //{
                if (msg.IndexOf("{") == -1)
                    return true;
                msg = ChoString.ExpandPropertiesInternal(target, msg, '{', '}', '^', new IChoPropertyReplacer[] { new ChoCustomKeyValuePropertyReplacer(target) });
                return true;
            //}
            //catch (ChoFatalApplicationException)
            //{
            //    throw;
            //}
            //catch (Exception ex)
            //{
            //    msg = ChoPropertyManager.FormatException(msg, ex);
            //    return false;
            //}
        }

        public string Name
        {
            get { return GetType().FullName; }
        }

        public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
        {
            get { return null; }
        }

        #endregion

		[Serializable]
		private class ChoCustomKeyValuePropertyReplacer : IChoKeyValuePropertyReplacer
        {
            #region Instance Data Members (Private)

            private object _target;

            #endregion Instance Data Members (Private)

            #region Constructors

            public ChoCustomKeyValuePropertyReplacer(object target)
            {
                _target = target;
            }

            #endregion Constructors

            #region IChoKeyValuePropertyReplacer Members

            public bool ContainsProperty(string propertyName, object context)
            {
                //return true;
                try
                {
                    ChoString.Evaluate(_target, propertyName);
                    return true;
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public string ReplaceProperty(string propertyName, string format, object context)
            {
                //try
                //{
                    //return ChoString.ToString(ChoString.Evaluate(_target, propertyName));
                    return ChoObject.ToString(ChoString.Evaluate(_target, propertyName), format);
                //}
                //catch (Exception ex)
                //{
                //    return ChoPropertyManager.FormatException(propertyName, ex);
                //}
            }

            #endregion

            #region IChoKeyValuePropertyReplacer Members


            public string GetPropertyDescription(string propertyName)
            {
                return null;
            }

            #endregion

            #region IChoPropertyReplacer Members

            public string Name
            {
                get { return GetType().FullName; }
            }

            public IEnumerable<KeyValuePair<string, string>> AvailablePropeties
            {
                get { return null; }
            }

            #endregion
        }
    }
}
