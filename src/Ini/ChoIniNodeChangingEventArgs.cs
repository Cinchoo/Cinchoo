namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

	public class ChoIniNodeInsertingEventArgs : ChoIniNodeInsertedEventArgs
	{
		#region Instance Data Members (Private)

		private bool _cancel = false;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniNodeInsertingEventArgs(IChoIniNode iniNode, object newValue)
			: base(iniNode, newValue)
		{
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		#endregion Instance Properties (Public)
	}

	public class ChoIniNodeRemovingEventArgs : ChoIniNodeInsertedEventArgs
	{
		#region Instance Data Members (Private)

		private bool _cancel = false;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniNodeRemovingEventArgs(IChoIniNode iniNode, object oldValue)
			: base(iniNode, oldValue)
		{
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		#endregion Instance Properties (Public)
	}

	public class ChoIniNodeChangingEventArgs : ChoIniNodeChangedEventArgs
    {
        #region Instance Data Members (Private)

        private bool _cancel = false;

        #endregion Instance Data Members (Private)

        #region Constructors

		internal ChoIniNodeChangingEventArgs(IChoIniNode iniNode, object oldValue, object newValue, ChoIniNodeChangedAction action)
            : base(iniNode, oldValue, newValue, action)
        {
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        #endregion Instance Properties (Public)
    }
}
