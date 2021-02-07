using Cinchoo.Core.Configuration;
using Cinchoo.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [ChoDictionaryConfigurationSection("plugInEditorSettings", ConfigFileNameFromTypeName = typeof(ChoPlugInEditorSettings))]
    public class ChoPlugInEditorSettings : ChoConfigurableObject
    {
        [ChoPropertyInfo("newPlugInDefFileVisible", DefaultValue = "True")]
        public bool NewPlugInDefFileVisible;

        [ChoPropertyInfo("openPlugInDefFileVisible", DefaultValue = "True")]
        public bool OpenPlugInDefFileVisible;

        [ChoPropertyInfo("savePlugInDefFileVisible", DefaultValue = "True")]
        public bool SavePlugInDefFileVisible;

        [ChoPropertyInfo("saveAsPlugInDefFileVisible", DefaultValue = "True")]
        public bool SaveAsPlugInDefFileVisible;

        [ChoPropertyInfo("deletePlugInDefVisible", DefaultValue = "True")]
        public bool DeletePlugInDefVisible;

        [ChoPropertyInfo("organizePlugInDefVisible", DefaultValue = "True")]
        public bool OrganizePlugInDefVisible;

        [ChoPropertyInfo("addNewPlugInEnabled", DefaultValue = "True")]
        public bool AddNewPlugInEnabled;

        [ChoPropertyInfo("copyPastePlugInVisible", DefaultValue = "True")]
        public bool CopyPastePlugInVisible;

        [ChoPropertyInfo("plugInsGroupControlsVisible", DefaultValue = "True")]
        public bool PlugInsGroupControlsVisible;

        [ChoPropertyInfo("plugInsGroupSelectionEnabled", DefaultValue = "True")]
        public bool PlugInsGroupSelectionEnabled;

        [ChoPropertyInfo("addNewPlugInsGroupEnabled", DefaultValue = "True")]
        public bool AddNewPlugInsGroupEnabled;

        [ChoPropertyInfo("deletePlugInsGroupEnabled", DefaultValue = "True")]
        public bool DeletePlugInsGroupEnabled;

        [ChoPropertyInfo("renamePlugInsGroupEnabled", DefaultValue = "True")]
        public bool RenamePlugInsGroupEnabled;

        [ChoPropertyInfo("scriptsFolder", DefaultValue = "Scripts")]
        public string ScriptsFolder;

        [ChoIgnoreProperty]
        public bool Seperator1Visible
        {
            get
            {
                return NewPlugInDefFileVisible || OpenPlugInDefFileVisible || SavePlugInDefFileVisible || SaveAsPlugInDefFileVisible;
            }
        }

        [ChoIgnoreProperty]
        public bool Seperator2Visible
        {
            get
            {
                return CopyPastePlugInVisible;
            }
        }

        [ChoIgnoreProperty]
        public bool Seperator3Visible
        {
            get
            {
                return DeletePlugInDefVisible || OrganizePlugInDefVisible;
            }
        }

        //protected override void OnAfterConfigurationObjectLoaded()
        //{
        //    ChoPath.ResolveFullPath += (s, e) => e.Value.BaseDirectory = ScriptsFolder;
        //}
    }
}