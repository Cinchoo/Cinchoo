using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core.Shell
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChoShellExtensionContextMenuAttribute : Attribute
    {
        public ChoShellExtensionContextMenuAttribute(string fileType)
        {
            ChoGuard.ArgumentNotNullOrEmpty(fileType, "FileType");

            FileType = fileType;
        }

        public string FileType
        {
            get;
            private set;
        }

        public string ShellKeyName
        {
            get;
            set;
        }

        public string MenuText
        {
            get;
            set;
        }

        public string AdditionalCommandLineArgs
        {
            get;
            set;
        }

        public string DefaultArgPrefix
        {
            get;
            set;
        }

        public string Icon
        {
            get;
            set;
        }

        public string IconResourceFilePath
        {
            get;
            set;
        }

        private int _iconIndex;
        public int IconIndex
        {
            get { return _iconIndex; }
            set
            {
                if (value >= 0)
                    _iconIndex = value;
            }
        }
    }
}
