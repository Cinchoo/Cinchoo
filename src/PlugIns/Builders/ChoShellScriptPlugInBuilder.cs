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
    [ChoPlugIn("ShellScriptPlugIn", typeof(ChoShellScriptPlugIn), typeof(ChoShellScriptPlugInBuilderProperty))]
    public class ChoShellScriptPlugInBuilder : ChoPlugInBuilder
    {
        private ChoCDATA _script = new ChoCDATA();
        [XmlElement("codeScript")]
        public ChoCDATA Script
        {
            get { return _script; }
            set
            {
                if (value == _script) return;
                _script = value;
                RaisePropertyChanged("Script");
            }
        }

        private ChoCDATA _arguments;
        [XmlElement("arguments")]
        public ChoCDATA Arguments
        {
            get { return _arguments; }
            set
            {
                if (value == _arguments) return;
                _arguments = value;
                RaisePropertyChanged("Arguments");
            }
        }

        private string _workingDirectory;
        [XmlAttribute("workingDirectory")]
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set
            {
                if (value == _workingDirectory) return;
                _workingDirectory = value;
                RaisePropertyChanged("WorkingDirectory");
            }
        }

        protected override void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;
            
            base.InitPlugIn(plugIn);

            ChoShellScriptPlugIn o = plugIn as ChoShellScriptPlugIn;
            if (o == null) return;

            o.Script = Script.GetValue();
            o.Arguments = Arguments.GetValue();
            o.WorkingDirectory = WorkingDirectory;
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoShellScriptPlugInBuilderProperty o = plugInBuilderProperty as ChoShellScriptPlugInBuilderProperty;
            if (o == null) return;

            o.Script = Script.GetValue();
            o.Arguments = Arguments.GetValue();
            o.WorkingDirectory = WorkingDirectory;
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            base.ApplyPropertyValues(plugInBuilderProperty, propertyName);

            ChoShellScriptPlugInBuilderProperty o = plugInBuilderProperty as ChoShellScriptPlugInBuilderProperty;
            if (o == null) return false;

            if (propertyName == "Script")
                Script = new ChoCDATA(o.Script);
            else if (propertyName == "Arguments")
                Arguments = new ChoCDATA(o.Arguments);
            else if (propertyName == "WorkingDirectory")
                WorkingDirectory = o.WorkingDirectory;
            else
                return false;

            return true;
        }

        protected override void Clone(ChoPlugInBuilder o)
        {
            base.Clone(o);
            ChoShellScriptPlugInBuilder p = o as ChoShellScriptPlugInBuilder;
            if (p == null) return;

            p.Script = Script;
            p.Arguments = Arguments;
            p.WorkingDirectory = WorkingDirectory;
        }
    }
}
