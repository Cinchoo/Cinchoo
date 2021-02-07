using Cinchoo.Core.Compiler;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.IO;
using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoCSharpCodeSnippetPlugIn : ChoPlugIn
    {
        #region Instance Properties

        public string CodeSnippet;
        public string Arguments;
        public string Namespaces;

        private bool _isInitialized = false;
        private ChoCodeDomProvider _cs;

        #endregion Instance Properties

        protected override void Validate()
        {
            base.Validate();
            ChoGuard.ArgumentNotNullOrEmpty(CodeSnippet, "CodeSnippet");
        }

        protected override object Execute(object value, out bool isHandled)
        {
            isHandled = false;
            Init();
            if (_cs == null) return value;

            string arguments = !Arguments.IsNullOrWhiteSpace() ? "{0} {1}".FormatString(value.ToNString(), ResolveText(Arguments)) : value.ToNString();
            return _cs.ExecuteFunc(ChoString.Split2Objects(arguments, ' '));
        }

        private void Init()
        {
            if (_isInitialized) return;

            try
            {
                if (CodeSnippet.IsNullOrWhiteSpace()) return;

                string codeSnippet = ResolveText(CodeSnippet);
                string[] nameSpaces = Namespaces.SplitNTrim();

                _cs = new ChoCodeDomProvider(new string[] { codeSnippet }, nameSpaces);
            }
            finally
            {
                _isInitialized = true;
            }
        }

        public override void InitializeBuilder(ChoPlugInBuilder builder)
        {
            base.InitializeBuilder(builder);

            ChoCSharpCodeSnippetPlugInBuilder b = builder as ChoCSharpCodeSnippetPlugInBuilder;
            b.CodeSnippet = new ChoCDATA(CodeSnippet);
            b.Arguments = new ChoCDATA(Arguments);
            b.Namespaces = Namespaces;
        }
    }
}
