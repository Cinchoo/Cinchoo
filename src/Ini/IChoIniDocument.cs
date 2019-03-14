namespace Cinchoo.Core.Ini
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;

    #endregion NameSpaces

    public interface IChoIniDocument
    {
        ChoIniNewLineNode AddNewLineToHeadingComments();
        ChoIniCommentNode AddHeadingComment(string commentLine);
        ChoIniCommentNode[] AddHeadingComments(string[] commentLines);
        ChoIniNode[] GetHeadingComments();
        bool RemoveHeadingComments();
		
        ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath);
        ChoIniIncludeFileNode AddIniIncludeFileNode(string filePath, string comments);
        ChoIniIncludeFileNode GetIniIncludeFileNode(string filePath);
        bool TryGetIniIncludeFileNode(string filePath, out ChoIniIncludeFileNode iniIncludeFileNode);
        bool ContainsIniIncludeFileNode(string filePath);
		bool RemoveIniIncludeFileNode(string filePath);
		bool CommentIniIncludeFileNode(string filePath);
		bool UncommentIniIncludeFileNode(string filePath);

        ChoIniSectionNode AddSection(string sectionName);
        ChoIniSectionNode GetSection(string sectionName);
        bool TryGetSection(string sectionName, out ChoIniSectionNode section);
        bool ContainsSection(string sectionName);
        bool RemoveSection(string sectionName);
        bool CommentSection(string sectionName);
        bool UncommentSection(string sectionName);

        IEnumerator<ChoIniNode> GetEnumerator();
        string ToString();
        string[] AllIniFilePaths { get; }
        string Path { get; }
    }
}
