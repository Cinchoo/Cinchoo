using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public enum ChoScriptType 
    {
        [Description("Script")]
        Script,
        [Description("DOS Script")]
        DOSScript,
        [Description("C# Code")]
        CSharp,
        [Description("VB.NET Code")]
        VBNet,
        [Description("JavaScript")]
        JavaScript,
        [Description("VBScript")]
        VBScript,
    }

    public abstract class ChoPlugInBuilderProperty : ChoNotifyPropertyChangedObject
    {
        private string _name;
        [Category("Common")]
        [Description("Indicate the name of the plugin")]
        [DisplayName("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "Name");
                if (_name == value) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _description;
        [Category("Common")]
        [Description("Indicate the short description of the plugin")]
        [DisplayName("Description")]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        private bool _doPropertyResolve;
        [Category("Common")]
        [Description("Indicate to perform property resolve on script, arguments etc")]
        [DisplayName("DoPropertyResolve")]
        public bool DoPropertyResolve
        {
            get { return _doPropertyResolve; }
            set
            {
                if (_doPropertyResolve == value) return;
                _doPropertyResolve = value;
                RaisePropertyChanged("DoPropertyResolve");
            }
        }

        private bool _enabled;
        [Category("Common")]
        [Description("Indicate the plugin is enabled or not")]
        [DisplayName("Enabled")]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                RaisePropertyChanged("Enabled");
            }
        }

        [Browsable(false)]
        public virtual ChoScriptType ScriptType
        {
            get
            {
                return ChoScriptType.Script;
            }
        }
    }
}
