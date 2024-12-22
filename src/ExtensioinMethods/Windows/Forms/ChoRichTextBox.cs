using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public static class ChoRichTextBox
    {
        public static void AddContextMenu(this RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = true };
                ToolStripMenuItem tsmiUndo = new ToolStripMenuItem("Undo");
                tsmiUndo.Enabled = false;
                rtb.TextChanged += (s, e) => tsmiUndo.Enabled = rtb.CanUndo;
                tsmiUndo.Click += (sender, e) => { if (rtb.CanUndo) rtb.Undo(); };
                cms.Items.Add(tsmiUndo);
                ToolStripSeparator tsSep1 = new ToolStripSeparator();
                cms.Items.Add(tsSep1);
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut") { Image = Cinchoo.Core.Properties.Resources.Cut };
                tsmiCut.Click += (sender, e) => rtb.Cut();
                cms.Items.Add(tsmiCut);
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy") { Image = Cinchoo.Core.Properties.Resources.Copy };
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste") { Image = Cinchoo.Core.Properties.Resources.Paste };
                tsmiPaste.Click += (sender, e) => rtb.Paste();
                cms.Items.Add(tsmiPaste);
                ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("Delete") { Image = Cinchoo.Core.Properties.Resources.Delete };
                tsmiDelete.Click += (sender, e) => rtb.Clear();
                cms.Items.Add(tsmiDelete);
                ToolStripSeparator tsSep2 = new ToolStripSeparator();
                cms.Items.Add(tsSep2);
                ToolStripMenuItem tsmiSelectAll = new ToolStripMenuItem("SelectAll");
                tsmiSelectAll.Click += (sender, e) => { rtb.Focus(); rtb.SelectAll(); };
                cms.Items.Add(tsmiSelectAll);

                rtb.ContextMenuStrip = cms;
            }
        }
    }
}
