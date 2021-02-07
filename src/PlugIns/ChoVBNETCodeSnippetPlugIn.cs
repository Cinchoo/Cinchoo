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
    public class ChoVBNETCodeSnippetPlugIn : ChoPlugIn
    {
        #region Instance Properties

        public string CodeSnippet;
        public string Arguments;
        public string Namespaces;

        #endregion Instance Properties

        protected override void Validate()
        {
            base.Validate();
            ChoGuard.ArgumentNotNullOrEmpty(CodeSnippet, "CodeSnippet");
        }

        protected override object Execute(object value, out bool isHandled)
        {
            isHandled = false;
            if (CodeSnippet.IsNullOrWhiteSpace()) return value;

            string codeSnippet = ResolveText(CodeSnippet);
            string arguments = !Arguments.IsNullOrWhiteSpace() ? "{0} {1}".FormatString(value.ToNString(), ResolveText(Arguments)) : value.ToNString();
            string[] nameSpaces = Namespaces.SplitNTrim();

            ChoCodeDomProvider cs = new ChoCodeDomProvider(new string[] { codeSnippet }, nameSpaces, ChoCodeProviderLanguage.VB);
            return cs.ExecuteFunc(ChoString.Split2Objects(arguments, ' '));
        }

        public override void InitializeBuilder(ChoPlugInBuilder builder)
        {
            base.InitializeBuilder(builder);

            ChoVBNETCodeSnippetPlugInBuilder b = builder as ChoVBNETCodeSnippetPlugInBuilder;
            b.CodeSnippet = new ChoCDATA(CodeSnippet);
            b.Arguments = new ChoCDATA(Arguments);
            b.Namespaces = Namespaces;
        }
    }
}
