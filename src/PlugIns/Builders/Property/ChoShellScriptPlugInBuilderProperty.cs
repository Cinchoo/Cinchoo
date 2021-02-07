using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoShellScriptPlugInBuilderProperty : ChoPlugInBuilderProperty, IChoScriptExtensionObject
    {
        private string _script;
        
        [Category("Miscellaneous")]
        [Description("DOS script code snippet")]
        [DisplayName("Script")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string Script
        {
            get { return _script; }
            set
            {
                if (_script == value) return;
                _script = value;
                RaisePropertyChanged("Script");
                ScriptTextChanged.Raise(null, null);
            }
        }

        private string _arguments;

        [Category("Miscellaneous")]
        [Description("Arguments to script")]
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

        private string _workingDirectory;

        [Category("Miscellaneous")]
        [Description("Working directory of the script")]
        [DisplayName("Working Directory")]
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set
            {
                if (_workingDirectory == value) return;
                _workingDirectory = value;
                RaisePropertyChanged("WorkingDirectory");
            }
        }

        [Browsable(false)]
        public string ScriptText
        {
            get
            {
                return _script;
            }
            set
            {
                if (_script == value) return;
                _script = value;
                RaisePropertyChanged("Script");
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
                return ChoScriptType.DOSScript;
            }
        }
    }
}
