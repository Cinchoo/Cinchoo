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
    [ChoPlugIn("VBNETCodeSnippetPlugIn", typeof(ChoVBNETCodeSnippetPlugIn), typeof(ChoVBNETCodeSnippetPlugInBuilderProperty))]
    public class ChoVBNETCodeSnippetPlugInBuilder : ChoPlugInBuilder
    {
        private ChoCDATA _codeSnippet;
        [XmlElement("codeSnippet")]
        public ChoCDATA CodeSnippet
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
        public ChoCDATA Arguments
        {
            get { return _arguments; }
            set
            {
                if (_arguments == value) return;
                _arguments = value;
                RaisePropertyChanged("Arguments");
            }
        }

        private string _namespaces;
        [XmlAttribute("namespaces")]
        public string Namespaces
        {
            get { return _namespaces; }
            set
            {
                if (_namespaces == value) return;
                _namespaces = value;
                RaisePropertyChanged("NameSpaces");
            }
        }

        protected override void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;
            
            base.InitPlugIn(plugIn);

            ChoVBNETCodeSnippetPlugIn o = plugIn as ChoVBNETCodeSnippetPlugIn;
            if (o == null) return;

            o.CodeSnippet = CodeSnippet.GetValue();
            o.Arguments = Arguments.GetValue();
            o.Namespaces = Namespaces;
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoVBNETCodeSnippetPlugInBuilderProperty o = plugInBuilderProperty as ChoVBNETCodeSnippetPlugInBuilderProperty;
            if (o == null) return;

            o.CodeSnippet = CodeSnippet.GetValue();
            o.Arguments = Arguments.GetValue();
            o.Namespaces = Namespaces;
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            if (base.ApplyPropertyValues(plugInBuilderProperty, propertyName))
                return true;

            ChoVBNETCodeSnippetPlugInBuilderProperty o = plugInBuilderProperty as ChoVBNETCodeSnippetPlugInBuilderProperty;
            if (o == null) return false;

            if (propertyName == "CodeSnippet")
                CodeSnippet = new ChoCDATA(o.CodeSnippet);
            else if (propertyName == "Arguments")
                Arguments = o.Arguments;
            else if (propertyName == "Namespaces")
                Namespaces = o.Namespaces;
            else
                return false;

            return true;
        }

        protected override void Clone(ChoPlugInBuilder o)
        {
            base.Clone(o);
            ChoVBNETCodeSnippetPlugInBuilder p = o as ChoVBNETCodeSnippetPlugInBuilder;
            if (p == null) return;

            p.CodeSnippet = CodeSnippet;
            p.Arguments = Arguments;
            p.Namespaces = Namespaces;
        }
    }
}
