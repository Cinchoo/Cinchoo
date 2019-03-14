namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	public interface IChoMergeable
	{
        void Merge(object source);
	}

    public interface IChoMergeable<T> : IChoMergeable
    {
        void Merge(T source);
    }
}
