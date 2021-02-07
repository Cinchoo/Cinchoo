namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Cinchoo.Core.Common;
    using Cinchoo.Core.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection.Emit;
    using System.CodeDom.Compiler;
    using System.Reflection;
    using System.IO;
    using Cinchoo.Core.Compiler;
    using System.Diagnostics;

    #endregion NameSpaces

    public class ChoGlobalDynamicMethodInvokeReplacer : IChoCustomPropertyReplacer
    {
        private static ChoCustomKeyValuePropertyReplacer _customKeyValuePropertyReplacer = new ChoCustomKeyValuePropertyReplacer();

        #region IChoCustomPropertyReplacer Members

        public bool Format(object target, ref string msg)
        {
            //try
            //{
                if (msg.IndexOf("~") == -1)
                    return true;

                msg = ChoString.ExpandPropertiesInternal(target, msg, '~', '~', '^', new IChoPropertyReplacer[] { _customKeyValuePropertyReplacer });
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
            private static IEnumerable<string> _refAssemblies;

            #region Constructors

            static ChoCustomKeyValuePropertyReplacer()
            {
                _refAssemblies = AppDomain.CurrentDomain
                                            .GetAssemblies()
                                            .Where(a => !a.IsDynamic)
                                            .Select(a => a.Location);
            }

            public ChoCustomKeyValuePropertyReplacer()
            {
            }

            #endregion Constructors

            #region IChoKeyValuePropertyReplacer Members

            public bool ContainsProperty(string propertyName, object context = null)
            {
                //return true;
                try
                {
                    object output = null;
                    string codeBlock = propertyName.Trim();
                    if (!codeBlock.Contains(";") && !codeBlock.StartsWith("return"))
                        codeBlock = "return {0};".FormatString(codeBlock);

                    using (ChoCodeDomProvider cs = new ChoCodeDomProvider(new string[] { codeBlock }))
                        output = cs.ExecuteFunc(context);

                    return true;
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    return false;
                }
            }

            public string ReplaceProperty(string propertyName, string format, object context)
            {
                //try
                //{
                    object output = null;
                    string codeBlock = propertyName.Trim();
                    if (!codeBlock.Contains(";") && !codeBlock.StartsWith("return"))
                        codeBlock = "return {0};".FormatString(codeBlock);

                    using (ChoCodeDomProvider cs = new ChoCodeDomProvider(new string[] { codeBlock }))
                        output = cs.ExecuteFunc(context);

                    return ChoObject.ToString(output, format);
                //}
                //catch (ChoFatalApplicationException)
                //{
                //    throw;
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
