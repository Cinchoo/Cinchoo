using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    [ChoPlugIn("CSharpCodeSnippetPlugIn", typeof(ChoCSharpCodeSnippetPlugIn), typeof(ChoCSharpCodeSnippetPlugInBuilderProperty))]
    public class ChoCSharpCodeSnippetPlugInBuilder : ChoPlugInBuilder
    {
        private ChoCDATA _codeSnippet;
        [XmlElement("codeSnippet")]
        public virtual ChoCDATA CodeSnippet
        {
            get { return _codeSnippet; }
            set
            {
                if (_codeSnippet == value) return;

                _codeSnippet = value;
                RaisePropertyChanged("CodeSnippet");
            }
        }

        private ChoCDATA _arguments;
        [XmlElement("arguments")]
        public virtual ChoCDATA Arguments
        {
            get { return _arguments; }
            set
            {
                if (_arguments == value) return;
                _arguments = value;
                RaisePropertyChanged("Arguments");
            }
        }

        private string _nameSpaces;
        [XmlAttribute("nameSpaces")]
        public virtual string Namespaces
        {
            get { return _nameSpaces; }
            set
            {
                if (_nameSpaces == value) return;
                _nameSpaces = value;
                RaisePropertyChanged("NameSpaces");
            }
        }

        protected override void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;
            
            base.InitPlugIn(plugIn);

            ChoCSharpCodeSnippetPlugIn o = plugIn as ChoCSharpCodeSnippetPlugIn;
            if (o == null) return;

            o.CodeSnippet = CodeSnippet.GetValue();
            o.Arguments = Arguments.GetValue();
            o.Namespaces = Namespaces;
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoCSharpCodeSnippetPlugInBuilderProperty o = plugInBuilderProperty as ChoCSharpCodeSnippetPlugInBuilderProperty;
            if (o == null) return;

            o.CodeSnippet = CodeSnippet.GetValue();
            o.Arguments = Arguments.GetValue();
            o.Namespaces = Namespaces;
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            if (base.ApplyPropertyValues(plugInBuilderProperty, propertyName)) return true;

            ChoCSharpCodeSnippetPlugInBuilderProperty o = plugInBuilderProperty as ChoCSharpCodeSnippetPlugInBuilderProperty;
            if (o == null) return false;

            if (propertyName == "CodeSnippet")
                CodeSnippet = new ChoCDATA(o.CodeSnippet);
            else if (propertyName == "Arguments")
                Arguments = new ChoCDATA(o.Arguments);
            else if (propertyName == "Namespaces")
                Namespaces = o.Namespaces;
            else
                return false;

            return true;
        }

        protected override void Clone(ChoPlugInBuilder o)
        {
            base.Clone(o);
            ChoCSharpCodeSnippetPlugInBuilder p = o as ChoCSharpCodeSnippetPlugInBuilder;
            if (p == null) return;

            p.CodeSnippet = CodeSnippet;
            p.Arguments = Arguments;
            p.Namespaces = Namespaces;
        }
    }
}
