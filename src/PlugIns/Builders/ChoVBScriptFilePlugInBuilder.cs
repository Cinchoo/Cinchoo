using Cinchoo.Core.IO;
using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    [ChoPlugIn("VBScriptFilePlugIn", typeof(ChoVBScriptFilePlugIn), typeof(ChoVBScriptFilePlugInBuilderProperty))]
    public class ChoVBScriptFilePlugInBuilder : ChoPlugInBuilder
    {
        private string _scriptFilePath;
        [XmlAttribute("scriptFilePath")]
        public string ScriptFilePath
        {
            get { return _scriptFilePath; }
            set
            {
                if (value == _scriptFilePath) return;

                if (!ChoPath.HasExtension(value, ChoReservedFileExt.VBS))
                    throw new ArgumentException("Invalid file path specified. Must have extension of VBS.");

                _scriptFilePath = value;
                ChoDirectory.CreateDirectoryFromFilePath(_scriptFilePath);
                RaisePropertyChanged("ScriptFilePath");
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

            ChoVBScriptFilePlugIn o = plugIn as ChoVBScriptFilePlugIn;
            if (o == null) return;

            o.ScriptFilePath = ScriptFilePath;
            o.Arguments = Arguments.GetValue();
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoVBScriptFilePlugInBuilderProperty o = plugInBuilderProperty as ChoVBScriptFilePlugInBuilderProperty;
            if (o == null) return;

            o.ScriptFilePath = ScriptFilePath;
            o.Arguments = Arguments.GetValue();
            o.WorkingDirectory = WorkingDirectory;
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            base.ApplyPropertyValues(plugInBuilderProperty, propertyName);

            ChoVBScriptFilePlugInBuilderProperty o = plugInBuilderProperty as ChoVBScriptFilePlugInBuilderProperty;
            if (o == null) return false;

            if (propertyName == "ScriptFilePath")
                ScriptFilePath = o.ScriptFilePath;
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
            ChoVBScriptFilePlugInBuilder p = o as ChoVBScriptFilePlugInBuilder;
            if (p == null) return;

            p.ScriptFilePath = ScriptFilePath;
            p.Arguments = Arguments;
            p.WorkingDirectory = WorkingDirectory;
        }
    }
}
