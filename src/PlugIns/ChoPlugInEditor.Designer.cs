using Cinchoo.Core.Windows.Forms;
namespace Cinchoo.Core
{
    partial class ChoPlugInEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoPlugInEditor));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tsMain = new System.Windows.Forms.ToolStrip();
            this.tsbNewPlugInDefFile = new System.Windows.Forms.ToolStripButton();
            this.tsbOpenPlugInDefFile = new System.Windows.Forms.ToolStripButton();
            this.tsbSavePlugInDefFile = new System.Windows.Forms.ToolStripButton();
            this.tsbSaveAsPlugInDefFile = new System.Windows.Forms.ToolStripButton();
            this.tss1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCopyPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbPastePlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tss2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCancelRunPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbRunPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbRunAllPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tss3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDeletePlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbMoveUpPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbMoveDownPlugInDef = new System.Windows.Forms.ToolStripButton();
            this.tsbClearOutput = new System.Windows.Forms.ToolStripButton();
            this.tss4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCmbPlugInsGroup = new System.Windows.Forms.ToolStripComboBox();
            this.tsbAddNewPlugInGroup = new System.Windows.Forms.ToolStripButton();
            this.tsbDeletePlugInGroup = new System.Windows.Forms.ToolStripButton();
            this.tsbRenamePlugInGroup = new System.Windows.Forms.ToolStripButton();
            this.ttPlugIns = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddNewPlugIn = new System.Windows.Forms.Button();
            this.splitContainer1 = new Cinchoo.Core.Windows.Forms.ChoSplitContainer();
            this.splitContainer2 = new Cinchoo.Core.Windows.Forms.ChoSplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbAvailPlugIns = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstPlugIns = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pgPlugInProperty = new System.Windows.Forms.PropertyGrid();
            this.splitContainer3 = new Cinchoo.Core.Windows.Forms.ChoSplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtParams = new System.Windows.Forms.TextBox();
            this.grpScriptDisplayName = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtScript = new ICSharpCode.TextEditor.TextEditorControl();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.grpScriptDisplayName.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(705, 0);
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(705, 25);
            this.toolStripContainer1.TabIndex = 6;
            this.toolStripContainer1.TabStop = false;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsMain);
            // 
            // tsMain
            // 
            this.tsMain.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNewPlugInDefFile,
            this.tsbOpenPlugInDefFile,
            this.tsbSavePlugInDefFile,
            this.tsbSaveAsPlugInDefFile,
            this.tss1,
            this.tsbCopyPlugInDef,
            this.tsbPastePlugInDef,
            this.tss2,
            this.tsbCancelRunPlugInDef,
            this.tsbRunPlugInDef,
            this.tsbRunAllPlugInDef,
            this.tss3,
            this.tsbDeletePlugInDef,
            this.tsbMoveUpPlugInDef,
            this.tsbMoveDownPlugInDef,
            this.tsbClearOutput,
            this.tss4,
            this.tsbCmbPlugInsGroup,
            this.tsbAddNewPlugInGroup,
            this.tsbDeletePlugInGroup,
            this.tsbRenamePlugInGroup});
            this.tsMain.Location = new System.Drawing.Point(3, 0);
            this.tsMain.Name = "tsMain";
            this.tsMain.Size = new System.Drawing.Size(527, 25);
            this.tsMain.TabIndex = 0;
            // 
            // tsbNewPlugInDefFile
            // 
            this.tsbNewPlugInDefFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNewPlugInDefFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbNewPlugInDefFile.Image")));
            this.tsbNewPlugInDefFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNewPlugInDefFile.Name = "tsbNewPlugInDefFile";
            this.tsbNewPlugInDefFile.Size = new System.Drawing.Size(23, 22);
            this.tsbNewPlugInDefFile.ToolTipText = "Create new PlugIn definition";
            this.tsbNewPlugInDefFile.Click += new System.EventHandler(this.tsbNewPlugInDefFile_Click);
            // 
            // tsbOpenPlugInDefFile
            // 
            this.tsbOpenPlugInDefFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenPlugInDefFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpenPlugInDefFile.Image")));
            this.tsbOpenPlugInDefFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenPlugInDefFile.Name = "tsbOpenPlugInDefFile";
            this.tsbOpenPlugInDefFile.Size = new System.Drawing.Size(23, 22);
            this.tsbOpenPlugInDefFile.ToolTipText = "Open a existing PlugIn definition file";
            this.tsbOpenPlugInDefFile.Click += new System.EventHandler(this.tsbOpenPlugInDefFile_Click);
            // 
            // tsbSavePlugInDefFile
            // 
            this.tsbSavePlugInDefFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSavePlugInDefFile.Enabled = false;
            this.tsbSavePlugInDefFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbSavePlugInDefFile.Image")));
            this.tsbSavePlugInDefFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSavePlugInDefFile.Name = "tsbSavePlugInDefFile";
            this.tsbSavePlugInDefFile.Size = new System.Drawing.Size(23, 22);
            this.tsbSavePlugInDefFile.ToolTipText = "Save PlugIn definitions to file";
            this.tsbSavePlugInDefFile.Click += new System.EventHandler(this.tsbSavePlugInDefFile_Click);
            // 
            // tsbSaveAsPlugInDefFile
            // 
            this.tsbSaveAsPlugInDefFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveAsPlugInDefFile.Image = ((System.Drawing.Image)(resources.GetObject("tsbSaveAsPlugInDefFile.Image")));
            this.tsbSaveAsPlugInDefFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAsPlugInDefFile.Name = "tsbSaveAsPlugInDefFile";
            this.tsbSaveAsPlugInDefFile.Size = new System.Drawing.Size(23, 22);
            this.tsbSaveAsPlugInDefFile.ToolTipText = "Save As the PlugIn definitions to new file";
            this.tsbSaveAsPlugInDefFile.Click += new System.EventHandler(this.tsbSaveAsPlugInDefFile_Click);
            // 
            // tss1
            // 
            this.tss1.Name = "tss1";
            this.tss1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCopyPlugInDef
            // 
            this.tsbCopyPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopyPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopyPlugInDef.Image")));
            this.tsbCopyPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyPlugInDef.Name = "tsbCopyPlugInDef";
            this.tsbCopyPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbCopyPlugInDef.ToolTipText = "Copy PlugIn";
            this.tsbCopyPlugInDef.Click += new System.EventHandler(this.tsbCopyPlugInDef_Click);
            // 
            // tsbPastePlugInDef
            // 
            this.tsbPastePlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPastePlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbPastePlugInDef.Image")));
            this.tsbPastePlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPastePlugInDef.Name = "tsbPastePlugInDef";
            this.tsbPastePlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbPastePlugInDef.ToolTipText = "Paste PlugIn";
            this.tsbPastePlugInDef.Click += new System.EventHandler(this.tsbPastePlugInDef_Click);
            // 
            // tss2
            // 
            this.tss2.Name = "tss2";
            this.tss2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCancelRunPlugInDef
            // 
            this.tsbCancelRunPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCancelRunPlugInDef.Enabled = false;
            this.tsbCancelRunPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbCancelRunPlugInDef.Image")));
            this.tsbCancelRunPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCancelRunPlugInDef.Name = "tsbCancelRunPlugInDef";
            this.tsbCancelRunPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbCancelRunPlugInDef.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.tsbCancelRunPlugInDef.ToolTipText = "Cancel the run of PlugIns";
            this.tsbCancelRunPlugInDef.Click += new System.EventHandler(this.tsbCancelRunPlugInDef_Click);
            // 
            // tsbRunPlugInDef
            // 
            this.tsbRunPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRunPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbRunPlugInDef.Image")));
            this.tsbRunPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRunPlugInDef.Name = "tsbRunPlugInDef";
            this.tsbRunPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbRunPlugInDef.ToolTipText = "Run just selected PlugIn";
            this.tsbRunPlugInDef.Click += new System.EventHandler(this.tsbRunPlugInDef_Click);
            // 
            // tsbRunAllPlugInDef
            // 
            this.tsbRunAllPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRunAllPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbRunAllPlugInDef.Image")));
            this.tsbRunAllPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRunAllPlugInDef.Name = "tsbRunAllPlugInDef";
            this.tsbRunAllPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbRunAllPlugInDef.ToolTipText = "Run all PlugIns from the selected one";
            this.tsbRunAllPlugInDef.Click += new System.EventHandler(this.tsbRunAllPlugInDef_Click);
            // 
            // tss3
            // 
            this.tss3.Name = "tss3";
            this.tss3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbDeletePlugInDef
            // 
            this.tsbDeletePlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeletePlugInDef.Enabled = false;
            this.tsbDeletePlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbDeletePlugInDef.Image")));
            this.tsbDeletePlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeletePlugInDef.Name = "tsbDeletePlugInDef";
            this.tsbDeletePlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbDeletePlugInDef.ToolTipText = "Remove PlugIn";
            this.tsbDeletePlugInDef.Click += new System.EventHandler(this.tsbDeletePlugInDef_Click);
            // 
            // tsbMoveUpPlugInDef
            // 
            this.tsbMoveUpPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveUpPlugInDef.Enabled = false;
            this.tsbMoveUpPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbMoveUpPlugInDef.Image")));
            this.tsbMoveUpPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveUpPlugInDef.Name = "tsbMoveUpPlugInDef";
            this.tsbMoveUpPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbMoveUpPlugInDef.ToolTipText = "Move up the order of the PlugIn";
            this.tsbMoveUpPlugInDef.Click += new System.EventHandler(this.tsbMoveUpPlugInDef_Click);
            // 
            // tsbMoveDownPlugInDef
            // 
            this.tsbMoveDownPlugInDef.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveDownPlugInDef.Enabled = false;
            this.tsbMoveDownPlugInDef.Image = ((System.Drawing.Image)(resources.GetObject("tsbMoveDownPlugInDef.Image")));
            this.tsbMoveDownPlugInDef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveDownPlugInDef.Name = "tsbMoveDownPlugInDef";
            this.tsbMoveDownPlugInDef.Size = new System.Drawing.Size(23, 22);
            this.tsbMoveDownPlugInDef.ToolTipText = "Move down the order of the PlugIn";
            this.tsbMoveDownPlugInDef.Click += new System.EventHandler(this.tsbMoveDownPlugInDef_Click);
            // 
            // tsbClearOutput
            // 
            this.tsbClearOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbClearOutput.Image = ((System.Drawing.Image)(resources.GetObject("tsbClearOutput.Image")));
            this.tsbClearOutput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClearOutput.Name = "tsbClearOutput";
            this.tsbClearOutput.Size = new System.Drawing.Size(23, 22);
            this.tsbClearOutput.ToolTipText = "Clear output";
            this.tsbClearOutput.Click += new System.EventHandler(this.tsbClearOutput_Click);
            // 
            // tss4
            // 
            this.tss4.Name = "tss4";
            this.tss4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCmbPlugInsGroup
            // 
            this.tsbCmbPlugInsGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsbCmbPlugInsGroup.Name = "tsbCmbPlugInsGroup";
            this.tsbCmbPlugInsGroup.Size = new System.Drawing.Size(121, 25);
            this.tsbCmbPlugInsGroup.ToolTipText = "Select PlugIn Group";
            // 
            // tsbAddNewPlugInGroup
            // 
            this.tsbAddNewPlugInGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddNewPlugInGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsbAddNewPlugInGroup.Image")));
            this.tsbAddNewPlugInGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddNewPlugInGroup.Name = "tsbAddNewPlugInGroup";
            this.tsbAddNewPlugInGroup.Size = new System.Drawing.Size(23, 22);
            this.tsbAddNewPlugInGroup.ToolTipText = "Add New PlugIn Group";
            this.tsbAddNewPlugInGroup.Click += new System.EventHandler(this.tsbAddNewPlugInGroup_Click);
            // 
            // tsbDeletePlugInGroup
            // 
            this.tsbDeletePlugInGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeletePlugInGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsbDeletePlugInGroup.Image")));
            this.tsbDeletePlugInGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeletePlugInGroup.Name = "tsbDeletePlugInGroup";
            this.tsbDeletePlugInGroup.Size = new System.Drawing.Size(23, 22);
            this.tsbDeletePlugInGroup.ToolTipText = "Remove PlugIn Group";
            this.tsbDeletePlugInGroup.Click += new System.EventHandler(this.tsbDeletePlugInGroup_Click);
            // 
            // tsbRenamePlugInGroup
            // 
            this.tsbRenamePlugInGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRenamePlugInGroup.Image = ((System.Drawing.Image)(resources.GetObject("tsbRenamePlugInGroup.Image")));
            this.tsbRenamePlugInGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRenamePlugInGroup.Name = "tsbRenamePlugInGroup";
            this.tsbRenamePlugInGroup.Size = new System.Drawing.Size(23, 22);
            this.tsbRenamePlugInGroup.ToolTipText = "Rename PlugIn Group";
            this.tsbRenamePlugInGroup.Click += new System.EventHandler(this.tsbRenamePlugInGroup_Click);
            // 
            // btnAddNewPlugIn
            // 
            this.btnAddNewPlugIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewPlugIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddNewPlugIn.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNewPlugIn.Image")));
            this.btnAddNewPlugIn.Location = new System.Drawing.Point(237, 15);
            this.btnAddNewPlugIn.Name = "btnAddNewPlugIn";
            this.btnAddNewPlugIn.Size = new System.Drawing.Size(24, 23);
            this.btnAddNewPlugIn.TabIndex = 1;
            this.ttPlugIns.SetToolTip(this.btnAddNewPlugIn, "Add new PlugIn");
            this.btnAddNewPlugIn.UseVisualStyleBackColor = true;
            this.btnAddNewPlugIn.Click += new System.EventHandler(this.btnAddNewPlugIn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(-4, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(717, 603);
            this.splitContainer1.SpliterLineVisible = true;
            this.splitContainer1.SplitterDistance = 281;
            this.splitContainer1.TabIndex = 4;
            this.splitContainer1.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(276, 624);
            this.splitContainer2.SpliterLineVisible = true;
            this.splitContainer2.SplitterDistance = 291;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnAddNewPlugIn);
            this.groupBox1.Controls.Add(this.cmbAvailPlugIns);
            this.groupBox1.Location = new System.Drawing.Point(6, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 42);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Avails PlugIns";
            // 
            // cmbAvailPlugIns
            // 
            this.cmbAvailPlugIns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbAvailPlugIns.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvailPlugIns.FormattingEnabled = true;
            this.cmbAvailPlugIns.Location = new System.Drawing.Point(6, 15);
            this.cmbAvailPlugIns.Name = "cmbAvailPlugIns";
            this.cmbAvailPlugIns.Size = new System.Drawing.Size(225, 21);
            this.cmbAvailPlugIns.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lstPlugIns);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Location = new System.Drawing.Point(3, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(270, 237);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Added PlugIns";
            // 
            // lstPlugIns
            // 
            this.lstPlugIns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstPlugIns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstPlugIns.DisplayMember = "Name";
            this.lstPlugIns.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstPlugIns.FormattingEnabled = true;
            this.lstPlugIns.Location = new System.Drawing.Point(7, 19);
            this.lstPlugIns.Name = "lstPlugIns";
            this.lstPlugIns.Size = new System.Drawing.Size(257, 210);
            this.lstPlugIns.TabIndex = 0;
            this.lstPlugIns.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstPlugIns_DrawItem);
            this.lstPlugIns.SelectedIndexChanged += new System.EventHandler(this.lstPlugIns_SelectedIndexChanged);
            this.lstPlugIns.SelectedValueChanged += new System.EventHandler(this.lstPlugIns_SelectedValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.pgPlugInProperty);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(270, 284);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // pgPlugInProperty
            // 
            this.pgPlugInProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgPlugInProperty.Location = new System.Drawing.Point(7, 19);
            this.pgPlugInProperty.Name = "pgPlugInProperty";
            this.pgPlugInProperty.Size = new System.Drawing.Size(257, 259);
            this.pgPlugInProperty.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer3.Location = new System.Drawing.Point(0, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.label1);
            this.splitContainer3.Panel1.Controls.Add(this.txtParams);
            this.splitContainer3.Panel1.Controls.Add(this.grpScriptDisplayName);
            this.splitContainer3.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer3.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer3.Size = new System.Drawing.Size(425, 597);
            this.splitContainer3.SpliterLineVisible = true;
            this.splitContainer3.SplitterDistance = 384;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Arguments";
            // 
            // txtParams
            // 
            this.txtParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtParams.Location = new System.Drawing.Point(6, 22);
            this.txtParams.Name = "txtParams";
            this.txtParams.Size = new System.Drawing.Size(413, 20);
            this.txtParams.TabIndex = 3;
            // 
            // grpScriptDisplayName
            // 
            this.grpScriptDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpScriptDisplayName.Controls.Add(this.panel1);
            this.grpScriptDisplayName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.grpScriptDisplayName.Location = new System.Drawing.Point(6, 42);
            this.grpScriptDisplayName.Name = "grpScriptDisplayName";
            this.grpScriptDisplayName.Size = new System.Drawing.Size(413, 328);
            this.grpScriptDisplayName.TabIndex = 1;
            this.grpScriptDisplayName.TabStop = false;
            this.grpScriptDisplayName.Text = "Script";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtScript);
            this.panel1.Location = new System.Drawing.Point(5, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 301);
            this.panel1.TabIndex = 2;
            // 
            // txtScript
            // 
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScript.IsReadOnly = false;
            this.txtScript.LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.FullRow;
            this.txtScript.Location = new System.Drawing.Point(3, 3);
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(387, 290);
            this.txtScript.TabIndex = 1;
            this.txtScript.TextChanged += new System.EventHandler(this.txtScript_TextChanged);
            this.txtScript.Leave += new System.EventHandler(this.txtScript_Leave);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.txtOutput);
            this.groupBox5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox5.Location = new System.Drawing.Point(4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(413, 190);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Output";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutput.Location = new System.Drawing.Point(8, 19);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(399, 165);
            this.txtOutput.TabIndex = 1;
            // 
            // ChoPlugInEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 622);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChoPlugInEditor";
            this.Text = "PlugIn Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChoPlugInEditor_FormClosing);
            this.Load += new System.EventHandler(this.ChoPlugInEditor_Load);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tsMain.ResumeLayout(false);
            this.tsMain.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.grpScriptDisplayName.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ChoSplitContainer splitContainer1;
        private ChoSplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private ChoSplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.GroupBox grpScriptDisplayName;
        private ICSharpCode.TextEditor.TextEditorControl txtScript;
        private System.Windows.Forms.ListBox lstPlugIns;
        private System.Windows.Forms.PropertyGrid pgPlugInProperty;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAddNewPlugIn;
        private System.Windows.Forms.ComboBox cmbAvailPlugIns;
        private System.Windows.Forms.ToolTip ttPlugIns;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtParams;
        private System.Windows.Forms.ToolStrip tsMain;
        private System.Windows.Forms.ToolStripButton tsbNewPlugInDefFile;
        private System.Windows.Forms.ToolStripButton tsbOpenPlugInDefFile;
        private System.Windows.Forms.ToolStripButton tsbSavePlugInDefFile;
        private System.Windows.Forms.ToolStripButton tsbSaveAsPlugInDefFile;
        private System.Windows.Forms.ToolStripSeparator tss1;
        private System.Windows.Forms.ToolStripButton tsbCancelRunPlugInDef;
        private System.Windows.Forms.ToolStripButton tsbRunPlugInDef;
        private System.Windows.Forms.ToolStripButton tsbRunAllPlugInDef;
        private System.Windows.Forms.ToolStripSeparator tss3;
        private System.Windows.Forms.ToolStripButton tsbDeletePlugInDef;
        private System.Windows.Forms.ToolStripButton tsbMoveUpPlugInDef;
        private System.Windows.Forms.ToolStripButton tsbMoveDownPlugInDef;
        private System.Windows.Forms.ToolStripButton tsbClearOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton tsbCopyPlugInDef;
        private System.Windows.Forms.ToolStripButton tsbPastePlugInDef;
        private System.Windows.Forms.ToolStripSeparator tss2;
        private System.Windows.Forms.ToolStripSeparator tss4;
        private System.Windows.Forms.ToolStripComboBox tsbCmbPlugInsGroup;
        private System.Windows.Forms.ToolStripButton tsbAddNewPlugInGroup;
        private System.Windows.Forms.ToolStripButton tsbDeletePlugInGroup;
        private System.Windows.Forms.ToolStripButton tsbRenamePlugInGroup;
    }
}