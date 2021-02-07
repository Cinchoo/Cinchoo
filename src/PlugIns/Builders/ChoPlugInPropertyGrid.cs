using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core
{
    public partial class ChoPlugInPropertyGrid : Form
    {
        public ChoPlugInPropertyGrid()
        {
            InitializeComponent();
        }

        public object SelectedObject
        {
            get { return pgMain.SelectedObject; }
            set { pgMain.SelectedObject = value; }
        }
    }
}
