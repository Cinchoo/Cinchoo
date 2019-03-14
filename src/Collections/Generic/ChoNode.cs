namespace Cinchoo.Core.Collections.Generic
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	#endregion NameSpaces

	public class ChoNode<TKey, TValue>
	{
		#region Instance Data Members (Private)

		private volatile ChoNode<TKey, TValue> _next;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoNode(TKey key, TValue value, int hashcode)
			: this(key, value, hashcode, null)
		{
		}

		public ChoNode(TKey key, TValue value, int hashcode, ChoNode<TKey, TValue> next)
		{
			Key = key;
			Value = value;
			Next = next;
			Hashcode = hashcode;
		}

		#endregion Constructors

		#region Instance Properties

		public int Hashcode
		{
			get;
			private set;
		}

		public TKey Key
		{
			get;
			private set;
		}

		public ChoNode<TKey, TValue> Next
		{
			get { return _next; }
			set { _next = value; }
		}

		public TValue Value
		{
			get;
			private set;
		}

		#endregion Instance Properties
	}
}
