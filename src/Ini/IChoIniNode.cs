namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public interface IChoIniNode
    {
        bool Comment();
        bool Comment(char commentChar);
        string ToString();
        bool Uncomment();
        void Clear();
        object SyncRoot { get; }
    }
}
