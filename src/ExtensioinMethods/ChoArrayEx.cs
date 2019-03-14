namespace System
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	#endregion NameSpaces

	public static class ChoArrayEx
	{
		public static bool IsNullOrEmpty(this Array array)
		{
			return array == null || array.Length == 0;
		}

		public static void CheckIndex(this Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			if (index == 0 && array.Length == 0)
				return;

			if (index < 0 || (uint)index >= (uint)array.Length)
				throw new ArgumentOutOfRangeException("index");
		}
		
		public static void CheckRange(this Array array, int idx, int count)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			int size = array.Length;

			if (idx < 0)
				throw new ArgumentOutOfRangeException("index");

			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			if ((uint)idx + (uint)count > (uint)size)
				throw new ArgumentException("index and count exceed length of list");
		}
	}
}
