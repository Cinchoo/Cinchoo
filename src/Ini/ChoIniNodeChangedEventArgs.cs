namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ChoIniNodeChangedAction { Value, Comment, Uncomment, ReplaceNode }

	public class ChoIniNodeInsertedEventArgs : EventArgs
	{
		#region Instance Data Members (Private)

		private readonly IChoIniNode _iniNode;
		private readonly object _newValue;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniNodeInsertedEventArgs(IChoIniNode iniNode, object newValue)
		{
			ChoGuard.ArgumentNotNull(iniNode, "IniNode");

			_iniNode = iniNode;
			_newValue = newValue;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public object NewValue
		{
			get { return _newValue; }
		}

		public IChoIniNode Node
		{
			get
			{
				return _iniNode;
			}
		}

		#endregion Instance Properties (Public)
	}

	public class ChoIniNodeRemovedEventArgs : EventArgs
	{
		#region Instance Data Members (Private)

		private readonly IChoIniNode _iniNode;
		private readonly object _oldValue;

		#endregion Instance Data Members (Private)

		#region Constructors

		internal ChoIniNodeRemovedEventArgs(IChoIniNode iniNode, object oldValue)
		{
			ChoGuard.ArgumentNotNull(iniNode, "IniNode");

			_iniNode = iniNode;
			_oldValue = oldValue;
		}

		#endregion Constructors

		#region Instance Properties (Public)

		public object OldValue
		{
			get { return _oldValue; }
		}

		public IChoIniNode Node
		{
			get
			{
				return _iniNode;
			}
		}

		#endregion Instance Properties (Public)
	}

	public class ChoIniNodeChangedEventArgs : EventArgs
    {
        #region Instance Data Members (Private)

		private readonly IChoIniNode _iniNode;
        private readonly object _oldValue;
        private readonly object _newValue;
        private readonly ChoIniNodeChangedAction _action;

        #endregion Instance Data Members (Private)

        #region Constructors

		internal ChoIniNodeChangedEventArgs(IChoIniNode iniNode, object oldValue, object newValue, ChoIniNodeChangedAction action)
        {
            ChoGuard.ArgumentNotNull(iniNode, "IniNode");

            _iniNode = iniNode;
            _oldValue = oldValue;
            _newValue = newValue;
            _action = action;
        }

        #endregion Constructors

        #region Instance Properties (Public)

        public ChoIniNodeChangedAction Action
        {
            get { return _action; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }

		public IChoIniNode Node
        {
            get
            {
                return _iniNode;
            }
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        #endregion Instance Properties (Public)
    }
}
