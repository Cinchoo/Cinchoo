﻿using Cinchoo.Core.IO;
using Cinchoo.Core.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Cinchoo.Core
{
    [ChoPlugIn("JavaScriptPlugIn", typeof(ChoJavaScriptPlugIn), typeof(ChoJavaScriptPlugInBuilderProperty))]
    public class ChoJavaScriptPlugInBuilder : ChoPlugInBuilder
    {
        private ChoCDATA _script = new ChoCDATA();
        [XmlElement("codeScript")]
        public ChoCDATA Script
        {
            get { return _script; }
            set
            {
                if (_script == value) return;
                
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
                if (_arguments == value) return;
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
                if (_workingDirectory == value) return;
                _workingDirectory = value;
                RaisePropertyChanged("WorkingDirectory");
            }
        }

        protected override void InitPlugIn(ChoPlugIn plugIn)
        {
            if (plugIn == null) return;

            base.InitPlugIn(plugIn);

            ChoJavaScriptPlugIn o = plugIn as ChoJavaScriptPlugIn;
            if (o == null) return;

            o.Script = Script.GetValue();
            o.Arguments = Arguments.GetValue();
            o.WorkingDirectory = WorkingDirectory;
        }

        protected override void InitPlugInBuilderProperty(ChoPlugInBuilderProperty plugInBuilderProperty)
        {
            if (plugInBuilderProperty == null) return;

            base.InitPlugInBuilderProperty(plugInBuilderProperty);

            ChoJavaScriptPlugInBuilderProperty o = plugInBuilderProperty as ChoJavaScriptPlugInBuilderProperty;
            if (o == null) return;

            o.Script = Script.GetValue();
            o.Arguments = Arguments.GetValue();
            o.WorkingDirectory = WorkingDirectory;
        }

        protected override bool ApplyPropertyValues(ChoPlugInBuilderProperty plugInBuilderProperty, string propertyName)
        {
            if (plugInBuilderProperty == null) return false;
            base.ApplyPropertyValues(plugInBuilderProperty, propertyName);

            ChoJavaScriptPlugInBuilderProperty o = plugInBuilderProperty as ChoJavaScriptPlugInBuilderProperty;
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
            ChoJavaScriptPlugInBuilder p = o as ChoJavaScriptPlugInBuilder;
            if (p == null) return;

            p.Script = Script;
            p.Arguments = Arguments;
            p.WorkingDirectory = WorkingDirectory;
        }
    }
}
