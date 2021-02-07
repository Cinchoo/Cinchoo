using Cinchoo.Core.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms.Design;

namespace Cinchoo.Core
{
    public class ChoVBScriptFilePlugInBuilderProperty : ChoPlugInBuilderProperty, IChoScriptExtensionObject
    {
        public class VBSFileNameEditor : FileNameEditor
        {
            protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
            {
                openFileDialog.Filter = "VBS files (*.VBS)|*.VBS";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
            }
        }

        public event EventHandler<EventArgs> ScriptTextChanged;

        private string _scriptFilePath;
        
        [Category("Miscellaneous")]
        [Description("VB script file path")]
        [DisplayName("ScriptFile")]
        [EditorAttribute(typeof(VBSFileNameEditor), typeof(UITypeEditor))]
        public string ScriptFilePath
        {
            get { return _scriptFilePath; }
            set
            {
                ChoGuard.ArgumentNotNullOrEmpty(value, "ScriptFilePath");
                if (!ChoPath.HasExtension(value, ChoReservedFileExt.VBS))
                    throw new ArgumentException("Invalid file path specified. Must have extension of VBS.");

                if (_scriptFilePath == value) return;

                _scriptFilePath = ChoPath.GetFullPath(value, new ChoPlugInEditorSettings().ScriptsFolder);
                RaisePropertyChanged("ScriptFilePath");
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

        private string _scriptText;
        [Browsable(false)]
        public string ScriptText
        {
            get
            {
                if (ScriptFilePath.IsNullOrWhiteSpace())
                    throw new ChoPlugInException("Missing script file path.");

                if (!File.Exists(ScriptFilePath))
                {
                    FileIOPermission FilePermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, ScriptFilePath);
                    FilePermission.Demand();

                    var myFile = File.CreateText(ScriptFilePath);
                    myFile.Close();
                    if (!File.Exists(ScriptFilePath))
                        throw new ChoPlugInException("'{0}' file not exists.".FormatString(ScriptFilePath));
                }

                return File.ReadAllText(ScriptFilePath);
            }
            set
            {
                if (_scriptText == value) return;

                _scriptText = value;
                FileIOPermission FilePermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, ScriptFilePath);
                FilePermission.Demand();
                File.WriteAllText(ScriptFilePath, value);
            }
        }

        [Browsable(false)]
        public virtual bool IsScriptReadonly
        {
            get
            {
                return false;
            }
        }
    }
}
