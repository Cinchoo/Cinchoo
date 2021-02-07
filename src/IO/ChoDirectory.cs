namespace Cinchoo.Core.IO
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;

    #endregion NameSpaces

    public static class ChoDirectory
    {
        public static void CreateDirectoryFromFilePath(string filePath)
        {
            ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");

            string dir = Path.GetDirectoryName(filePath);
            if (!dir.IsNullOrEmpty())
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        public static IEnumerable<string> GetFiles(string path, string searchPattern = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (searchPattern.IsNullOrWhiteSpace())
            {
                foreach (string file in Directory.GetFiles(path))
                    yield return file;
            }
            else
            {
                foreach (string pattern in searchPattern.SplitNTrim())
                {
                    foreach (string file in Directory.GetFiles(path, pattern, searchOption))
                        yield return file;
                }
            }
        }
    }
}
