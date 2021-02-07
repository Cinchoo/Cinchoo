using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Windows.Shapes;
using Cinchoo.Core.IO;
using System.IO;
using System.CodeDom;

namespace Cinchoo.Core.Compiler
{
    public enum ChoCodeProviderLanguage { CSharp, VB };

    public class ChoCodeDomProvider : IDisposable
    {
        #region Shared Data Members (Private)

        private static IEnumerable<string> _refAssemblies;
        private static ChoCompilerParametersSettings _compilerParametersSettings = ChoCompilerParametersSettings.Me;

        #endregion Shared Data Members (Private)

        #region Instance Data Members (Private)

        public readonly List<string> Namespaces = new List<string>();
        private readonly string[] _statements;
        private readonly ChoCodeProviderLanguage _language;

        private dynamic _targetFunc = null;
        private dynamic _targetAction = null;

        #endregion Instance Data Members (Private)

        #region Constructors

        static ChoCodeDomProvider()
        {
            _refAssemblies = AppDomain.CurrentDomain
                                        .GetAssemblies()
                                        .Where(a => !a.IsDynamic && !a.Location.IsNullOrWhiteSpace())
                                        .Select(a => a.Location);
        }

        public ChoCodeDomProvider(string[] statements, string[] namespaces = null, ChoCodeProviderLanguage language = ChoCodeProviderLanguage.CSharp)
        {
            ChoGuard.ArgumentNotNullOrEmpty(statements, "statements");

            _statements = statements;
            _language = language;
            AddNamespaces(namespaces);
        }

        public ChoCodeDomProvider(string codeBlockFilePath, string[] namespaces = null, ChoCodeProviderLanguage language = ChoCodeProviderLanguage.CSharp)
        {
            ChoGuard.ArgumentNotNullOrEmpty(codeBlockFilePath, "codeBlockFilePath");

            if (!ChoFile.Exists(codeBlockFilePath))
                throw new ArgumentException("{0} file not exists.".FormatString(codeBlockFilePath));

            _statements = File.ReadAllLines(codeBlockFilePath);
            AddNamespaces(namespaces);
        }

        #endregion Constructors

        #region Instance Members (Public)

        public object ExecuteFunc(params object[] args)
        {
            if (_targetFunc == null)
            {
                string className = "ChoClass_{0}".FormatString(ChoRandom.NextRandom(0, Int32.MaxValue));
                string codeFragment = String.Join(" ", _statements);
                string statement = null;
                switch (_language)
                {
                    case ChoCodeProviderLanguage.VB:
                        statement = String.Format("Public Class {1} {0} Public Function Execute(ByVal args() As Object) as Object {0} {2} {0} End Function {0} End Class", Environment.NewLine, className, codeFragment);
                        //statement = statement.Replace("Return ", "Execute = ");
                        break;
                    default:
                        {
                            if (!codeFragment.IsNullOrWhiteSpace())
                                codeFragment = codeFragment.Replace("@this", "args[0]");
                            statement = String.Format("public class {0} {{ public object Execute(object[] args) {{ {1}; }} }}", className, codeFragment);
                            break;
                        }
                }
                _targetFunc = Activator.CreateInstance(CreateType(className, codeFragment, statement));
            }
            return _targetFunc.Execute(args);
        }

        public void ExecuteAction(params object[] args)
        {
            if (_targetAction == null)
            {
                string className = "ChoClass_{0}".FormatString(ChoRandom.NextRandom(0, Int32.MaxValue));
                string codeFragment = String.Join(" ", _statements);
                string statement = null;
                switch (_language)
                {
                    case ChoCodeProviderLanguage.VB:
                        statement = String.Format("Public Class {1} {0} Public Sub Execute(ByVal args() As Object) {0} {2} {0} End Sub {0} End Class", Environment.NewLine, className, codeFragment);
                        break;
                    default:
                        {
                            if (!codeFragment.IsNullOrWhiteSpace())
                                codeFragment = codeFragment.Replace("@this", "args[0]");
                            statement = String.Format("public class {0} {{ public void Execute(object[] args) {{ {1}; }} }}", className, codeFragment);
                            break;
                        }
                }
                _targetAction = Activator.CreateInstance(CreateType(className, codeFragment, statement));
            }
            _targetAction.Execute(args);
        }

        private Type CreateType(string className, string codeFragment, string statement)
        {
            CompilerParameters compilerParameters = CreateCompilerParameters();

            statement = String.Format("{0}{2}{1}", BuildNameSpaces(), statement, Environment.NewLine);

            CodeDomProvider codeDomProvider = null;
            switch (_language)
            {
                case ChoCodeProviderLanguage.VB:
                    codeDomProvider = new Microsoft.VisualBasic.VBCodeProvider();
                    break;
                default:
                    codeDomProvider = new Microsoft.CSharp.CSharpCodeProvider();
                    break;
            }

            using (codeDomProvider)
            {
                var res = codeDomProvider.CompileAssemblyFromSource(compilerParameters, statement);

                if (res.Errors.Count > 0)
                {
                    StringBuilder errors = new StringBuilder();
                    foreach (CompilerError CompErr in res.Errors)
                        errors.AppendFormat("Line number {0}, Error Number: {1}, {2}{3}", CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText, Environment.NewLine);

                    throw new ChoApplicationException("Exception compiling code fragment: {1}{1}{0}{1}{1}Exceptions:{1}{2}".FormatString(
                        statement.Indent(1), Environment.NewLine, errors.ToString().Indent(1)));
                }

                return res.CompiledAssembly.GetType(className);
            }
        }

        #endregion Instance Members (Public)

        #region Shared Members (Private)

        private string BuildNameSpaces()
        {
            StringBuilder ns = new StringBuilder();

            foreach (string s in Namespaces)
            {
                switch (_language)
                {
                    case ChoCodeProviderLanguage.VB:
                        ns.Append(String.Format("Imports {0}{1}", s, Environment.NewLine));
                        break;
                    default:
                        ns.Append(String.Format("using {0};{1}", s, Environment.NewLine));
                        break;
                }
            }

            return ns.ToString();
        }

        private void AddNamespaces(string[] namespaces)
        {
            CodeNamespace ns = new CodeNamespace();
            ns.Imports.Add(new CodeNamespaceImport("System"));
            CodeCompileUnit cu = new CodeCompileUnit();
            cu.Namespaces.Add(ns);

            if (_language == ChoCodeProviderLanguage.VB)
                Namespaces.Add("Microsoft.VisualBasic");

            Namespaces.Add("System");
            Namespaces.Add("System.Collections.Generic");
            Namespaces.Add("System.Collections");
            Namespaces.Add("System.Linq");
            Namespaces.Add("System.Text");
            Namespaces.Add("System.Data");
            Namespaces.Add("System.Xml");
            Namespaces.Add("System.Diagnostics");

            if (namespaces != null)
            {
                foreach (string s in namespaces)
                    Namespaces.Add(s);
            }

            if (!_compilerParametersSettings.Namespaces.IsNullOrWhiteSpace())
            {
                foreach (string s in _compilerParametersSettings.Namespaces.SplitNTrim(';'))
                    Namespaces.Add(s);
            }
        }

        private static CompilerParameters CreateCompilerParameters()
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateExecutable = _compilerParametersSettings.GenerateExecutable;
            compilerParameters.GenerateInMemory = _compilerParametersSettings.GenerateInMemory;
            compilerParameters.IncludeDebugInformation = _compilerParametersSettings.IncludeDebugInformation;
            compilerParameters.TreatWarningsAsErrors = _compilerParametersSettings.TreatWarningsAsErrors;
            compilerParameters.WarningLevel = _compilerParametersSettings.WarningLevel;

            if (_refAssemblies != null)
            {
                foreach (string s in _refAssemblies)
                {
                    bool found = false;
                    if (!_compilerParametersSettings.ExcludeRefAssemblies.IsNullOrWhiteSpace())
                    {
                        found = (from s1 in _compilerParametersSettings.ExcludeRefAssemblies.SplitNTrim(';')
                                 where s1 == System.IO.Path.GetFileName(s)
                                 select s1).FirstOrDefault() != null;
                    }

                    if (!found)
                    {
                        //compilerParameters.ReferencedAssemblies.Add(System.IO.Path.GetFileName(s));
                        compilerParameters.ReferencedAssemblies.Add(s);
                    }
                }
            }

            if (!_compilerParametersSettings.RefAssemblies.IsNullOrWhiteSpace())
            {
                foreach (string s in _compilerParametersSettings.RefAssemblies.SplitNTrim(';'))
                    compilerParameters.ReferencedAssemblies.Add(System.IO.Path.GetFileName(s));
            }

            return compilerParameters;
        }

        #endregion Shared Members (Private)

        public void Dispose()
        {
        }
    }
}