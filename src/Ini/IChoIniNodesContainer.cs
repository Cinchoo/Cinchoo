namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    public interface IChoIniNodesContainer
    {
        bool Remove(ChoIniNode node);
        ChoIniNewLineNode AppendNewLine();
    }
}
