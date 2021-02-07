using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cinchoo.Core.Windows.Forms
{
    public class ChoToolStripBindableButton : ToolStripButton, IBindableComponent
    {
        private ControlBindingsCollection _dataBindings;
        private BindingContext _bindingContext;

        public ChoToolStripBindableButton()
        {
            DataBindings = new ControlBindingsCollection(this);
            BindingContext = new BindingContext();
        }

        public ControlBindingsCollection DataBindings
        {
            get;
            private set;
        }

        public BindingContext BindingContext
        {
            get;
            set;
        }
    }
}
