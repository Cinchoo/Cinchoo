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

    #endregion NameSpaces

    public class ChoGlobalDynamicMethodInvokeReplacer : IChoCustomPropertyReplacer
    {
        private static ChoCustomKeyValuePropertyReplacer _customKeyValuePropertyReplacer = new ChoCustomKeyValuePropertyReplacer();

        #region IChoCustomPropertyReplacer Members

        public bool Format(object target, ref string msg)
        {
            try
            {
                if (msg.IndexOf("~") == -1)
                    return true;

                msg = ChoString.ExpandProperties(target, msg, '~', '~', '^', _customKeyValuePropertyReplacer);
                return true;
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                msg = ChoPropertyManager.FormatException(msg, ex);
                return false;
            }
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
            #region Shared Data Members (Private)

            private readonly static CompilerParameters _compilerParameters;

            #endregion Shared Data Members (Private)

            #region Constructors

            static ChoCustomKeyValuePropertyReplacer()
            {
                _compilerParameters = new CompilerParameters();
                _compilerParameters.GenerateExecutable = false;
                _compilerParameters.GenerateInMemory = true;
                _compilerParameters.IncludeDebugInformation = true;

                var assemblies = AppDomain.CurrentDomain
                                            .GetAssemblies()
                                            .Where(a => !a.IsDynamic)
                                            .Select(a => a.Location);

                foreach (string s in assemblies)
                    _compilerParameters.ReferencedAssemblies.Add(Path.GetFileName(s));
            }

            public ChoCustomKeyValuePropertyReplacer()
            {
            }

            #endregion Constructors

            #region IChoKeyValuePropertyReplacer Members

            public bool ContainsProperty(string propertyName)
            {
                return true;
            }

            public string ReplaceProperty(string propertyName, string format)
            {
                //try
                //{
                    object output = null;
                    string className = "ChoClass_{0}".FormatString(ChoRandom.NextRandom(0, Int32.MaxValue));
                    string statement = String.Format("public class {0} {{ public object Execute() {{ return {1}; }} }}", className, propertyName);

                    using (Microsoft.CSharp.CSharpCodeProvider foo =
                               new Microsoft.CSharp.CSharpCodeProvider())
                    {
                        var res = foo.CompileAssemblyFromSource(_compilerParameters,statement);

                        if (res.Errors.Count > 0)
                        {
                            StringBuilder errors = new StringBuilder();
                            foreach (CompilerError CompErr in res.Errors)
                                errors.AppendFormat("Line number {0}, Error Number: {1}, {2}{3}", CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText, Environment.NewLine);

                            throw new ChoApplicationException("Exception compiling dynamic statement {1}{1}{0}{1}{1}{2}".FormatString(
                                propertyName, Environment.NewLine, errors.ToString()));
                        }

                        var type = res.CompiledAssembly.GetType(className);
                        var obj = Activator.CreateInstance(type);

                        output = type.GetMethod("Execute").Invoke(obj, new object[] { });
                    }

                    return ChoObject.ToString(output, format);
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
