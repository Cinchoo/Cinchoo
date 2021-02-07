using Cinchoo.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoJavaScriptFilePlugIn : ChoPlugIn
    {
        #region Instance Properties

        private string _tmpFileName;
        public string ScriptFilePath;
        public string Arguments;
        public string WorkingDirectory;

        #endregion Instance Properties

        protected override void Validate()
        {
            base.Validate();
            ChoGuard.ArgumentNotNullOrEmpty(ScriptFilePath, "ScriptFilePath");
            if (!File.Exists(ScriptFilePath))
                throw new ChoPlugInException("Script file '{0}' not exists.".FormatString(ScriptFilePath));
        }

        protected override object Execute(object value, out bool isHandled)
        {
            isHandled = false;
            if (ScriptFilePath.IsNullOrWhiteSpace()) return value;

            string scriptFilePath = ResolveText(ScriptFilePath);
            string arguments = !Arguments.IsNullOrWhiteSpace() ? "{0} {1}".FormatString(value.ToNString(), ResolveText(Arguments)) : value.ToNString();
            string workingDirectory = WorkingDirectory.IsNullOrWhiteSpace() ? ChoApplication.ApplicationBaseDirectory : ResolveText(WorkingDirectory);

            //Create temp script file
            _tmpFileName = ResolveFileText(ref scriptFilePath);

            using (ChoProcess p = new ChoProcess("cscript.exe", "{0} {1} //Nologo".FormatString(scriptFilePath, arguments)))
            {
                p.WorkingDirectory = workingDirectory;
                return p.Execute().StdOut;
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            if (!_tmpFileName.IsNullOrWhiteSpace() && File.Exists(_tmpFileName))
                File.Delete(_tmpFileName);
        }

        public override void InitializeBuilder(ChoPlugInBuilder builder)
        {
            base.InitializeBuilder(builder);

            ChoJavaScriptFilePlugInBuilder b = builder as ChoJavaScriptFilePlugInBuilder;
            b.ScriptFilePath = ScriptFilePath;
            b.Arguments = new Xml.Serialization.ChoCDATA(Arguments);
            b.WorkingDirectory = WorkingDirectory;
        }
    }
}
