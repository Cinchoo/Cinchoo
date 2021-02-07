using Cinchoo.Core.ComponentModel;
using Cinchoo.Core.Configuration;
using Cinchoo.Core.IO;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Cinchoo.Core
{
    public partial class ChoPlugInEditor : Form
    {
        private readonly string Caption;
        private readonly ChoPlugInsRuntime _plugInsRuntime = new ChoPlugInsRuntime();
        private readonly ChoThreadedBindingList<ChoPlugInBuilder> _plugInBuilderList = new ChoThreadedBindingList<ChoPlugInBuilder>();
        private readonly object _padLock = new object();
        private readonly ChoPlugInEditorSettings _plugInEditorSettings = new ChoPlugInEditorSettings();

        private bool _forceClosing = false;
        private ChoPlugInsDefManager _plugInsDefManager;
        private int _index = 0;
        private bool _isPlugInSelected = false;
        private ChoPlugInBuilder _prevPlugInBuilder = null;
        private bool _isDirty = false;
        private ICloneable _copiedBuilder = null;
        private string _prevPlugInsGroupName = null;
        private bool _showFileNameInTitle = true;
        private string _plugInDefFilePath;
        private bool _isPlugInRunning = false;
        private string _defaultPlugInGroupName = null;

        public string Arguments;

        public event EventHandler<ChoPlugInEventArgs> BeforeAddNewPlugIn;

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                _isDirty = value;

                tsbSavePlugInDefFile.Enabled = _isDirty;
                //tsbSaveAsPlugInDefFile.Enabled = _isDirty;
            }
        }
        public string PlugInDefFilePath
        {
            get { return _plugInDefFilePath; }
            private set
            {
                _plugInDefFilePath = value;
                if (_plugInDefFilePath.IsNullOrWhiteSpace() || !_showFileNameInTitle)
                    this.Text = Caption;
                else
                    this.Text = "PlugIns Editor ({0})".FormatString(_plugInDefFilePath);
                _plugInsDefManager = new ChoPlugInsDefManager(_plugInDefFilePath);
                ttPlugIns.SetToolTip(this, _plugInDefFilePath);
            }
        }
        private int Count
        {
            get { return _plugInBuilderList.Count; }
        }

        #region Constructors

        public ChoPlugInEditor(string plugInDefFilePath = null, string defaultPlugInGroupName = null, bool showFileNameInTitle = true)
        {
            InitializeComponent();
            //txtScript.AddContextMenu();
            txtScript.Enabled = false;
            Caption = Text;
            _showFileNameInTitle = showFileNameInTitle;
            PlugInDefFilePath = plugInDefFilePath;
            _defaultPlugInGroupName = defaultPlugInGroupName;

            _plugInsRuntime.RunComplete += (s, e) =>
                {
                    try
                    {
                        txtOutput.SynchronizedInvoke(() => txtOutput.Text = e.Value.EndInvoke().ToNString());
                    }
                    catch (Exception ex)
                    {
                        txtOutput.SynchronizedInvoke(() => txtOutput.Text = FormatException(_plugInsRuntime.ActivePlugIn, ex));
                    }
                    finally
                    {
                        txtOutput.SynchronizedInvoke(() => SetAsPlugInRunComplete());
                    }
                };

            Application.Idle += Application_Idle;
        }

        #endregion Constructors

        private void Application_Idle(object sender, EventArgs e)
        {
            //if (Count  == 0)
            //    _copiedBuilder = null;

            if (!_isPlugInRunning)
            {
                int index = lstPlugIns.SelectedIndex;

                tsbCopyPlugInDef.Enabled = Count > 0;
                tsbPastePlugInDef.Enabled = _copiedBuilder != null;
                tsbNewPlugInDefFile.Enabled = true;
                tsbOpenPlugInDefFile.Enabled = true;
                tsbDeletePlugInDef.Enabled = index >= 0;
                tsbMoveDownPlugInDef.Enabled = Count > 1 && index != 0;
                tsbMoveUpPlugInDef.Enabled = Count > 1 && index != Count - 1;
                tsbClearOutput.Enabled = !txtOutput.Text.IsNullOrEmpty();

                tsbRunPlugInDef.Enabled = index >= 0;
                tsbRunAllPlugInDef.Enabled = Count >= 1;
                tsbCancelRunPlugInDef.Enabled = false;

                if (PlugInDefFilePath.IsNullOrWhiteSpace())
                {
                    //IsDirty = Count > 0;
                    tsbSavePlugInDefFile.Enabled = IsDirty;
                    //tsbSaveAsPlugInDefFile.Enabled = IsDirty;
                }

                tsbCmbPlugInsGroup.Enabled = _plugInEditorSettings.PlugInsGroupSelectionEnabled;
                tsbAddNewPlugInGroup.Enabled = _plugInEditorSettings.AddNewPlugInsGroupEnabled;
                tsbDeletePlugInGroup.Enabled = _plugInEditorSettings.DeletePlugInsGroupEnabled;
                tsbRenamePlugInGroup.Enabled = _plugInEditorSettings.RenamePlugInsGroupEnabled;
            }
            else
            {
                txtOutput.Text = null;

                tsbPastePlugInDef.Enabled = false;
                tsbCopyPlugInDef.Enabled = false;
                tsbNewPlugInDefFile.Enabled = false;
                tsbOpenPlugInDefFile.Enabled = false;
                tsbDeletePlugInDef.Enabled = false;
                tsbMoveDownPlugInDef.Enabled = false;
                tsbMoveUpPlugInDef.Enabled = false;
                tsbClearOutput.Enabled = false;

                tsbRunPlugInDef.Enabled = false;
                tsbRunAllPlugInDef.Enabled = false;
                tsbCancelRunPlugInDef.Enabled = true;

                tsbSavePlugInDefFile.Enabled = false;
                tsbCmbPlugInsGroup.Enabled = false;
                tsbAddNewPlugInGroup.Enabled = false;
                tsbDeletePlugInGroup.Enabled = false;
                tsbRenamePlugInGroup.Enabled = false;
            }
        }

        private void ChoPlugInEditor_Load(object sender, EventArgs e)
        {
            ApplySettings();

            txtParams.Text = Arguments;
            Type plugInType = null;

            SortedDictionary<string, Type> availPlugInDict = new SortedDictionary<string, Type>();
            //Load all plugins builders to combobox
            foreach (Type t in ChoType.GetTypesDerivedFrom<ChoPlugInBuilder>())
            {
                plugInType = ChoPlugInBuilder.GetPlugInType(t);

                if (plugInType != null)
                    availPlugInDict.Add(ChoPlugInBuilder.GetPlugInName(t), t);
            }
            cmbAvailPlugIns.DataSource = new BindingSource(availPlugInDict, null);
            cmbAvailPlugIns.DisplayMember = "Key";
            cmbAvailPlugIns.ValueMember = "Value";

            tsbCmbPlugInsGroup.ComboBox.SelectedValueChanged += ComboBox_SelectedValueChanged;
            tsbCmbPlugInsGroup.ComboBox.DropDown += ComboBox_DropDown;
            Rebind();

            if (!_defaultPlugInGroupName.IsNullOrWhiteSpace())
            {
                if (tsbCmbPlugInsGroup.ComboBox.FindString(_defaultPlugInGroupName) >= 0)
                    tsbCmbPlugInsGroup.ComboBox.Text = _defaultPlugInGroupName;
            }

            //LoadPlugInsFromFile(PlugInDefFilePath);
        }

        private void ComboBox_DropDown(object sender, EventArgs e)
        {
            tsbCmbPlugInsGroup.ComboBox.ResizeDropDownListToMaxWidth();
        }

        private void ComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            lstPlugIns.DataSource = null;
            if (tsbCmbPlugInsGroup.ComboBox.Text.IsNullOrWhiteSpace()) return;

            if (_prevPlugInsGroupName != null)
            {
                if (tsbCmbPlugInsGroup.ComboBox.FindStringExact(_prevPlugInsGroupName) >= 0)
                    ApplyPlugInGroupChanges(_prevPlugInsGroupName, _plugInBuilderList.ToArray());
            }

            int counter = 0;
            _plugInBuilderList.Clear();

            bool cancel = false;
            foreach (ChoPlugInBuilder pb in _plugInsDefManager.GetPlugInBuilders(tsbCmbPlugInsGroup.ComboBox.Text).TryForEach((ex) =>
                {
                    DialogResult r = ChoMessageBox.Show(ex, "Error occurred loading definition. Do you want to ignore and continue loading definition?", Caption);
                    if (r == System.Windows.Forms.DialogResult.Yes)
                        return true;
                    else
                    {
                        cancel = true;
                        return false;
                    }
                }))
            {
                if (!pb.Name.IsNullOrWhiteSpace())
                {
                    if (Int32.TryParse(Regex.Match(pb.Name, "_([0-9]+)").Groups[1].Value, out counter))
                    {
                        if (counter > _index)
                            _index = counter;
                    }
                }

                pb.PropertyChanged += (o, e1) => IsDirty = true;
                _plugInBuilderList.Add(pb);
            }

            if (cancel)
            {
                _forceClosing = true;
                this.Close();
            }
            lstPlugIns.DataSource = _plugInBuilderList;
            lstPlugIns.DisplayMember = "Name";
            //lstPlugIns.ValueMember = "Key";

            _prevPlugInsGroupName = tsbCmbPlugInsGroup.ComboBox.Text;
        }

        private void ApplyPlugInGroupChanges(string plugInGroupName, ChoPlugInBuilder[] plugInBuilderList)
        {
            if (_plugInBuilderList == null || _plugInBuilderList.Count == 0)
                _plugInsDefManager.AddOrReplacePlugInGroup(plugInGroupName, ChoPlugInBuilder.ToXml(null));
            else
                _plugInsDefManager.AddOrReplacePlugInGroup(plugInGroupName, ChoPlugInBuilder.ToXml(plugInBuilderList.ToArray()));
        }

        private void Rebind()
        {
            //Set the source again
            this.tsbCmbPlugInsGroup.ComboBox.DisplayMember = "Key";
            this.tsbCmbPlugInsGroup.ComboBox.ValueMember = "Value";
            if (_plugInsDefManager.PlugInsGroupDict.Count > 0)
            {
                tsbCmbPlugInsGroup.ComboBox.DataSource = new BindingSource(_plugInsDefManager.PlugInsGroupDict, null);
                tsbCmbPlugInsGroup.Enabled = true;
            }
            else
            {
                tsbCmbPlugInsGroup.ComboBox.DataSource = null;
                tsbCmbPlugInsGroup.Enabled = false;
            }
        }

        private void tsbCmbPlugInsGroup_TextChanged(object sender, EventArgs e)
        {
        }

        private void LoadPlugInsFromFile(string plugInDefFilePath)
        {
            if (plugInDefFilePath.IsNullOrWhiteSpace())
                return;
            if (!File.Exists(plugInDefFilePath))
                return;

            int counter = 0;
            foreach (ChoPlugInBuilder pb in ChoPlugInBuilder.LoadFrom(plugInDefFilePath))
            {
                if (!pb.Name.IsNullOrWhiteSpace())
                {
                    if (Int32.TryParse(Regex.Match(pb.Name, "_([0-9]+)").Groups[1].Value, out counter))
                    {
                        if (counter > _index)
                            _index = counter;
                    }
                }

                pb.PropertyChanged += (o, e1) => IsDirty = true;
                _plugInBuilderList.Add(pb);
            }
        }

        private void ApplySettings()
        {
            tsbNewPlugInDefFile.Visible = _plugInEditorSettings.NewPlugInDefFileVisible;
            tsbOpenPlugInDefFile.Visible = _plugInEditorSettings.OpenPlugInDefFileVisible;
            tsbSavePlugInDefFile.Visible = _plugInEditorSettings.SavePlugInDefFileVisible;
            tsbSaveAsPlugInDefFile.Visible = _plugInEditorSettings.SaveAsPlugInDefFileVisible;

            tsbCopyPlugInDef.Visible = _plugInEditorSettings.CopyPastePlugInVisible;
            tsbPastePlugInDef.Visible = _plugInEditorSettings.CopyPastePlugInVisible;

            tsbDeletePlugInDef.Visible = _plugInEditorSettings.DeletePlugInDefVisible;
            tsbMoveUpPlugInDef.Visible = _plugInEditorSettings.OrganizePlugInDefVisible;
            tsbMoveDownPlugInDef.Visible = _plugInEditorSettings.OrganizePlugInDefVisible;

            tss1.Visible = _plugInEditorSettings.Seperator1Visible;
            tss2.Visible = _plugInEditorSettings.Seperator2Visible;
            tss3.Visible = _plugInEditorSettings.Seperator3Visible;

            tss4.Visible = _plugInEditorSettings.PlugInsGroupControlsVisible;
            tsbCmbPlugInsGroup.Visible = _plugInEditorSettings.PlugInsGroupControlsVisible;
            tsbAddNewPlugInGroup.Visible = _plugInEditorSettings.PlugInsGroupControlsVisible;
            tsbDeletePlugInGroup.Visible = _plugInEditorSettings.PlugInsGroupControlsVisible;
            tsbRenamePlugInGroup.Visible = _plugInEditorSettings.PlugInsGroupControlsVisible;

            tsbCmbPlugInsGroup.Enabled = _plugInEditorSettings.PlugInsGroupSelectionEnabled;
            tsbAddNewPlugInGroup.Enabled = _plugInEditorSettings.AddNewPlugInsGroupEnabled;
            tsbDeletePlugInGroup.Enabled = _plugInEditorSettings.DeletePlugInsGroupEnabled;
            tsbRenamePlugInGroup.Enabled = _plugInEditorSettings.RenamePlugInsGroupEnabled;
        }

        private void lstPlugIns_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void lstPlugIns_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ChoPlugInBuilder builder = ((ListBox)sender).Items[e.Index] as ChoPlugInBuilder;
            if (builder == null) return;

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            Brush myBrush = builder.Enabled ? Brushes.DarkGreen : Brushes.Red; //or whatever...
            // Draw the current item text based on the current 
            // Font and the custom brush settings.
            //
            e.Graphics.DrawString(builder.Name,
                  e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle 
            // around the selected item.
            //
            e.DrawFocusRectangle();
        }

        private int NextIndex()
        {
            lock (_padLock)
            {
                return ++_index;
            }
        }

        private void lstPlugIns_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            txtOutput.Text = null;
            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            _isPlugInSelected = builder != null;
            EnableOrDisableMenuItems();
            if (builder == null)
            {
                pgPlugInProperty.SelectedObject = null;
                ttPlugIns.SetToolTip(lstPlugIns, null);
                SetScriptText(null);
                return;
            }

            pgPlugInProperty.SelectedObject = builder.PlugInBuilderProperty;
            ttPlugIns.SetToolTip(lstPlugIns, builder.PlugInBuilderProperty.Description);

            if (_prevPlugInBuilder != null && _prevPlugInBuilder.PlugInBuilderProperty is IChoScriptExtensionObject)
            {
                ((IChoScriptExtensionObject)_prevPlugInBuilder.PlugInBuilderProperty).ScriptTextChanged -= ChoPlugInEditor_ScriptTextChanged;
                _prevPlugInBuilder = null;
            }

            if (builder.PlugInBuilderProperty is IChoScriptExtensionObject)
            {
                _prevPlugInBuilder = builder;
                ((IChoScriptExtensionObject)_prevPlugInBuilder.PlugInBuilderProperty).ScriptTextChanged += ChoPlugInEditor_ScriptTextChanged;
            }

            SetScriptText(builder);
        }

        private void ChoPlugInEditor_ScriptTextChanged(object sender, EventArgs e)
        {
            SetScriptText(_prevPlugInBuilder);
        }

        private void SetScriptText(ChoPlugInBuilder builder)
        {
            txtScript.Enabled = false;
            txtScript.IsReadOnly = !txtScript.Enabled;
            txtScript.Text = null;
            txtOutput.Text = null;
            grpScriptDisplayName.Text = builder == null || builder.PlugInBuilderProperty == null ? "Script" : ChoEnum.ToDescription(builder.PlugInBuilderProperty.ScriptType);

            if (builder == null || !(builder.PlugInBuilderProperty is IChoScriptExtensionObject)) return;

            try
            {
                if (txtScript.Text != ((IChoScriptExtensionObject)builder.PlugInBuilderProperty).ScriptText)
                {
                    txtScript.Text = ((IChoScriptExtensionObject)builder.PlugInBuilderProperty).ScriptText;

                    //txtScript.Select(txtScript.Text.Length, 0);
                }
                txtScript.Enabled = !((IChoScriptExtensionObject)builder.PlugInBuilderProperty).IsScriptReadonly;
                txtScript.IsReadOnly = !txtScript.Enabled;
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.Message;
                txtScript.Enabled = false;
                txtScript.IsReadOnly = !txtScript.Enabled;
            }
        }

        private void EnableOrDisableMenuItems()
        {
            int index = lstPlugIns.SelectedIndex;
            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            bool isPlugInSelected = builder != null;

            if (!isPlugInSelected)
            {
                tsbMoveUpPlugInDef.Enabled = false;
                tsbMoveDownPlugInDef.Enabled = false;
                tsbDeletePlugInDef.Enabled = false;
            }
            else
            {
                tsbMoveUpPlugInDef.Enabled = Count > 1 && index != 0;
                tsbMoveDownPlugInDef.Enabled = Count > 1 && index != Count - 1;
                tsbDeletePlugInDef.Enabled = true;
            }
        }

        private void ChoPlugInEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_forceClosing)
                return;

            SyncData();
            if (!_plugInEditorSettings.SavePlugInDefFileVisible)
            {
                ApplyPlugInGroupChanges(_prevPlugInsGroupName, _plugInBuilderList.ToArray());
                return;
            }

            e.Cancel = Save();
        }

        private bool Save()
        {
            if (IsDirty)
            {
                string msg;
                if (PlugInDefFilePath.IsNullOrWhiteSpace())
                    msg = "Do you want to save changes to untitled?";
                else
                    msg = "Do you want to save changes to '{0}'?".FormatString(Path.GetFileName(PlugInDefFilePath));
                DialogResult r = MessageBox.Show(msg, Caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                    return true;
                else if (r == DialogResult.No)
                {
                    IsDirty = false;
                    return false;
                }
                else
                    return !Save(PlugInDefFilePath);
            }
            return false;
        }

        public string ToXml()
        {
            return _plugInsDefManager.ToXml();
        }

        private bool Save(string filePath)
        {
            if (!SyncData())
                return false;
            ApplyPlugInGroupChanges(_prevPlugInsGroupName, _plugInBuilderList.ToArray());

            try
            {
                if (filePath.IsNullOrWhiteSpace())
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.Filter = "plg files (*.plg)|*.plg";
                        dialog.FilterIndex = 1;
                        dialog.RestoreDirectory = true;
                        dialog.InitialDirectory = ChoApplication.ApplicationBaseDirectory;

                        if (dialog.ShowDialog() == DialogResult.OK)
                            filePath = PlugInDefFilePath = ChoPath.ChangeExtension(dialog.FileName, ChoReservedFileExt.PLG);
                        else
                            return true;
                    }
                }

                _plugInsDefManager.Save(filePath);

                IsDirty = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while saving changes to '{0}'. {1}".FormatString(filePath, ex.Message),
                    Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        private void tsbNewPlugInDefFile_Click(object sender, EventArgs e)
        {
            if (Save())
                return;

            Clear();
        }

        private void Clear()
        {
            PlugInDefFilePath = null;
            IsDirty = false;
            _plugInBuilderList.Clear();
            txtParams.Clear();
            txtScript.Text = String.Empty;
            txtOutput.Clear();
        }

        private void tsbOpenPlugInDefFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "plg files (*.plg)|*.plg";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = ChoApplication.ApplicationBaseDirectory;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Clear();
                    PlugInDefFilePath = ChoPath.ChangeExtension(dialog.FileName, ChoReservedFileExt.PLG);
                    LoadPlugInsFromFile(PlugInDefFilePath);
                }
            }
        }

        private void tsbSavePlugInDefFile_Click(object sender, EventArgs e)
        {
            Save(PlugInDefFilePath);
        }

        private void tsbSaveAsPlugInDefFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "plg files (*.plg)|*.plg";
                dialog.FilterIndex = 1;
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = ChoApplication.ApplicationBaseDirectory;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    PlugInDefFilePath = ChoPath.ChangeExtension(dialog.FileName, ChoReservedFileExt.PLG);
                    Save(PlugInDefFilePath);
                }
            }
        }

        private void tsbCopyPlugInDef_Click(object sender, EventArgs e)
        {
            _copiedBuilder = lstPlugIns.SelectedValue as ICloneable;
        }

        private void tsbPastePlugInDef_Click(object sender, EventArgs e)
        {
            ICloneable builder = _copiedBuilder as ICloneable;
            if (builder == null) return;

            try
            {
                ChoPlugInBuilder newBuilder = builder.Clone() as ChoPlugInBuilder;
                if (newBuilder == null) return;

                newBuilder.Name = "{0}_Copy_{1}".FormatString(ChoPlugInBuilder.GetPlugInName(builder.GetType()), NextIndex());
                AddNewBuilder(newBuilder);
            }
            catch (Exception ex)
            {
                txtOutput.Text = FormatException(null, ex, "Error while pasting PlugIn.");
            }
        }

        private void SetAsPlugInRunning()
        {
            _isPlugInRunning = true;
        }

        private void SetAsPlugInRunComplete()
        {
            _plugInsRuntime.Reset();
            _isPlugInRunning = false;
        }

        private string FormatException(ChoPlugIn plugIn, Exception ex, string errMsg = null)
        {
            StringBuilder msg = new StringBuilder();
            if (plugIn != null)
                msg.AppendLine("Error found running '{0}' plugin.".FormatString(plugIn.Name));
            else
                msg.AppendLine(errMsg.IsNullOrWhiteSpace() ? "Error found running all plugins." : errMsg);

            msg.AppendLine("Error Msg: {0}.".FormatString(ex.Message));
            msg.AppendLine();
            msg.AppendLine("StackTrace:");
            msg.AppendLine(ChoApplicationException.ToString(ex));

            return msg.ToString();
        }

        private void tsbRunPlugInDef_Click(object sender, EventArgs e)
        {
            if (!SyncData())
                return;

            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            if (builder == null) return;

            try
            {
                SetAsPlugInRunning();
                _plugInsRuntime.Reset();
                _plugInsRuntime.Add(builder.CreatePlugIn());
                _plugInsRuntime.RunAsync(txtParams.Text);
            }
            catch (Exception ex)
            {
                txtOutput.Text = FormatException(_plugInsRuntime.ActivePlugIn, ex);
                //SetAsPlugInRunComplete();
            }
        }

        private void tsbRunAllPlugInDef_Click(object sender, EventArgs e)
        {
            if (!SyncData())
                return;

            try
            {
                SetAsPlugInRunning();
                _plugInsRuntime.Reset();
                foreach (ChoPlugInBuilder pb in _plugInBuilderList)
                    _plugInsRuntime.Add(pb.CreatePlugIn());

                _plugInsRuntime.RunAsync(txtParams.Text);
            }
            catch (Exception ex)
            {
                txtOutput.Text = FormatException(_plugInsRuntime.ActivePlugIn, ex);
                //SetAsPlugInRunComplete();
            }
        }

        private void tsbCancelRunPlugInDef_Click(object sender, EventArgs e)
        {
            ChoPlugInsRuntime plugInsManager = _plugInsRuntime;

            try
            {
                if (plugInsManager == null) return;

                plugInsManager.Cancel();
            }
            catch (Exception ex)
            {
                txtOutput.Text = FormatException(_plugInsRuntime.ActivePlugIn, ex);
            }
            finally
            {
                SetAsPlugInRunComplete();
            }
        }

        private void tsbMoveUpPlugInDef_Click(object sender, EventArgs e)
        {
            int index = lstPlugIns.SelectedIndex;

            if (index == 0) return;
            _plugInBuilderList.MoveItem(index, index - 1);
            lstPlugIns.SetSelected(index - 1, true);
            IsDirty = true;
        }

        private void tsbMoveDownPlugInDef_Click(object sender, EventArgs e)
        {
            int index = lstPlugIns.SelectedIndex;
            if (index == Count - 1) return;
            _plugInBuilderList.MoveItem(index, index + 1);
            lstPlugIns.SetSelected(index + 1, true);
            IsDirty = true;
        }

        private void btnAddNewPlugIn_Click(object sender, EventArgs e)
        {
            KeyValuePair<string, Type> keyValuePair = (KeyValuePair<string, Type>)cmbAvailPlugIns.SelectedItem;
            if (keyValuePair.Value == null) return;

            ChoPlugInBuilder builder = ChoActivator.CreateInstance(keyValuePair.Value) as ChoPlugInBuilder;
            if (builder == null) return;

            builder.Name = "{0}_{1}".FormatString(ChoPlugInBuilder.GetPlugInName(builder.GetType()), NextIndex());
            AddNewBuilder(builder);
        }

        private void AddNewBuilder(ChoPlugInBuilder builder)
        {
            BeforeAddNewPlugIn.Raise(null, new ChoPlugInEventArgs() { PlugInBuilder = builder, PlugInGroupName = _prevPlugInsGroupName });

            builder.PropertyChanged += (o, e1) => IsDirty = true;

            _plugInBuilderList.Add(builder);
            IsDirty = true;
            lstPlugIns.SetSelected(Count - 1, true);
        }

        private void tsbDeletePlugInDef_Click(object sender, EventArgs e)
        {
            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            if (builder == null) return;
            DialogResult r = MessageBox.Show("Are you sure want to delete '{0}' plugin?".FormatString(builder.Name), Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.No)
                return;
            _plugInBuilderList.Remove(builder);
            IsDirty = true;
            UpdateControls();
        }

        private void txtScript_TextChanged(object sender, EventArgs e)
        {
            if (txtScript.IsReadOnly) return;
            if (!_isPlugInSelected) return;
            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            if (builder == null) return;
            if (!(builder.PlugInBuilderProperty is IChoScriptExtensionObject)) return;

            IsDirty = true;
        }

        private void txtScript_Leave(object sender, EventArgs e)
        {
            SyncData();
        }

        private bool SyncData()
        {
            if (!IsDirty) return true;
            if (txtScript.IsReadOnly) return true;
            if (!_isPlugInSelected) return true;
            ChoPlugInBuilder builder = lstPlugIns.SelectedValue as ChoPlugInBuilder;
            if (builder == null) return true;
            if (!(builder.PlugInBuilderProperty is IChoScriptExtensionObject)) return true;

            try
            {
                ((IChoScriptExtensionObject)builder.PlugInBuilderProperty).ScriptText = txtScript.Text;
                return true;
            }
            catch (Exception ex)
            {
                txtOutput.Text = ex.Message;
                return false;
            }
        }

        private void tsbClearOutput_Click(object sender, EventArgs e)
        {
            txtOutput.Text = null;
        }

        private void tsbAddNewPlugInGroup_Click(object sender, EventArgs e)
        {
            string plugInGroupName = Interaction.InputBox("Please enter PlugIn group name?", "PlugInEditor", ChoPlugInsDefManager.DEF_PLUGIN_GROUP_NAME);
            if (plugInGroupName.IsNullOrWhiteSpace()) return;
            if (_plugInsDefManager.PlugInsGroupDict.ContainsKey(plugInGroupName))
            {
                MessageBox.Show("Plugin group with the name '{0}' already exists. Please choose another name.".FormatString(plugInGroupName), Caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            _plugInsDefManager.AddNewPlugInGroup(plugInGroupName);
            Rebind();
            int index = tsbCmbPlugInsGroup.FindStringExact(plugInGroupName);
            if (index >= 0)
                tsbCmbPlugInsGroup.SelectedIndex = index;
            IsDirty = true;
        }

        private void tsbDeletePlugInGroup_Click(object sender, EventArgs e)
        {
            string plugInGroupName = tsbCmbPlugInsGroup.ComboBox.Text;
            if (plugInGroupName.IsNullOrWhiteSpace()) return;

            if (MessageBox.Show("'{0}' plugin group will be deleted?".FormatString(plugInGroupName), Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Cancel)
                return;

            if (_plugInsDefManager.PlugInsGroupDict.ContainsKey(plugInGroupName))
            {
                _plugInsDefManager.PlugInsGroupDict.Remove(plugInGroupName);

                Rebind();
                IsDirty = true;
            }
        }

        private void tsbRenamePlugInGroup_Click(object sender, EventArgs e)
        {
            string plugInGroupName = tsbCmbPlugInsGroup.ComboBox.Text;
            if (plugInGroupName.IsNullOrWhiteSpace()) return;
            if (plugInGroupName == ChoPlugInsDefManager.DEF_PLUGIN_GROUP_NAME) return;

            int selectedIndex = tsbCmbPlugInsGroup.SelectedIndex;
            string newPlugInGroupName = Interaction.InputBox("Please enter PlugIn group name?", "PlugInEditor", plugInGroupName);
            if (newPlugInGroupName.IsNullOrWhiteSpace()) return;
            if (plugInGroupName == newPlugInGroupName) return;

            XElement value = _plugInsDefManager.PlugInsGroupDict[plugInGroupName];
            _plugInsDefManager.PlugInsGroupDict.Remove(plugInGroupName);
            _plugInsDefManager.PlugInsGroupDict.Add(newPlugInGroupName, value);
            Rebind();
            tsbCmbPlugInsGroup.SelectedIndex = selectedIndex;
            IsDirty = true;
        }
    }

    public class ChoPlugInEventArgs : EventArgs
    {
        public ChoPlugInBuilder PlugInBuilder;
        public string PlugInGroupName;
    }

}
