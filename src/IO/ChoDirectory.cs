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
    }
}
