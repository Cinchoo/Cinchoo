using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoVBNETCodeSnippetPlugInBuilderProperty : ChoPlugInBuilderProperty, IChoScriptExtensionObject
    {
        private string _codeSnippet;
        
        [Category("Miscellaneous")]
        [Description("VB Code Snippet")]
        [DisplayName("CodeSnippet")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string CodeSnippet
        {
            get { return _codeSnippet; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "CodeSnippet");
                if (_codeSnippet == value) return;
                _codeSnippet = value;
                RaisePropertyChanged("CodeSnippet");
                ScriptTextChanged.Raise(null, null);
            }
        }

        private string _arguments;

        [Category("Miscellaneous")]
        [Description("Arguments to C# method")]
        [DisplayName("Arguments")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Arguments
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

        [Category("Miscellaneous")]
        [Description("Namespaces to compile the code")]
        [DisplayName("Namespaces")]
        public string Namespaces
        {
            get { return _namespaces; }
            set
            {
                if (_namespaces == value) return;
                _namespaces = value;
                RaisePropertyChanged("Namespaces");
            }
        }

        [Browsable(false)]
        public string ScriptText
        {
            get
            {
                return _codeSnippet;
            }
            set
            {
                if (_codeSnippet == value) return;
                _codeSnippet = value;
                RaisePropertyChanged("CodeSnippet");
            }
        }

        public event EventHandler<EventArgs> ScriptTextChanged;

        [Browsable(false)]
        public virtual bool IsScriptReadonly
        {
            get
            {
                return false;
            }
        }

        [Browsable(false)]
        public override ChoScriptType ScriptType
        {
            get
            {
                return ChoScriptType.VBNet;
            }
        }
    }
}
