using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    public class ChoVBScriptPlugIn : ChoPlugIn
    {
        #region Instance Properties

        private string _tmpFileName;

        public string Script;
        public string Arguments;
        public string WorkingDirectory;
    
        #endregion Instance Properties

        protected override void Validate()
        {
            base.Validate();
            ChoGuard.ArgumentNotNullOrEmpty(Script, "Script");
        }

        protected override object Execute(object value, out bool isHandled)
        {
            isHandled = false;
            if (Script.IsNullOrWhiteSpace()) return value;

            string script = ResolveText(Script);
            string arguments = !Arguments.IsNullOrWhiteSpace() ? "{0} {1}".FormatString(value.ToNString(), ResolveText(Arguments)) : value.ToNString();
            string workingDirectory = WorkingDirectory.IsNullOrWhiteSpace() ? ChoApplication.ApplicationBaseDirectory : ResolveText(WorkingDirectory);

            //Create temp script file
            _tmpFileName = Path.ChangeExtension(ChoPath.GetTempFileName(), ChoReservedFileExt.VBS);
            File.WriteAllText(_tmpFileName, script);
            ChoTrace.DebugFormat("{0}: Temp file created at '{1}'", Name, _tmpFileName);

            using (ChoProcess p = new ChoProcess("cscript.exe", "{0} {1} //Nologo".FormatString(_tmpFileName, arguments)))
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

            ChoVBScriptPlugInBuilder b = builder as ChoVBScriptPlugInBuilder;
            b.Script = new Xml.Serialization.ChoCDATA(Script);
            b.Arguments = new Xml.Serialization.ChoCDATA(Arguments);
            b.WorkingDirectory = WorkingDirectory;
        }
    }
}
