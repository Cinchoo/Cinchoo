#region NameSpaces

using System;
using System.IO;
using System.Xml;

#endregion NameSpaces

namespace Cinchoo.Core.IO
{
    public class ChoFileSettings
    {
        public static int MaxFileRollCount = 100;
        public static string RollFileNameSeparator = "_";
    }


    public static class ChoFile
    {
        #region Shared Members (Public)

        public static void Roll(string filePath)
        {
            Roll(filePath, ChoFileSettings.MaxFileRollCount, ChoFileSettings.RollFileNameSeparator);
        }

        public static void Roll(string filePath, int maxFileCount)
        {
            Roll(filePath, maxFileCount, ChoFileSettings.RollFileNameSeparator);
        }

        public static void Roll(string filePath, string rollFileNameSeparator)
        {
            Roll(filePath, ChoFileSettings.MaxFileRollCount, rollFileNameSeparator);
        }

        public static void Roll(string filePath, int maxFileCount, string rollFileNameSeparator)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");
            if (!File.Exists(filePath)) return;

            if (maxFileCount <= 0)
                throw new ArgumentException("MaxFileCount should be greater than zero.");

            //Calculate file decimals
            int fileDecimals = 1;
            int decimalBase = 10;

            while (decimalBase < maxFileCount)
            {
                ++fileDecimals;
                decimalBase *= 10;
            }

            string rollFileName = null;
            for (int fileIndex = 0; fileIndex < maxFileCount; ++fileIndex)
            {
                rollFileName = GetBackupFileName(filePath, fileDecimals, fileIndex, rollFileNameSeparator);
                if (File.Exists(rollFileName))
                {
                    rollFileName = null;
                    continue;
                }
                else
                    break;
            }

            if (String.IsNullOrEmpty(rollFileName))
                Roll(filePath, maxFileCount * 100, rollFileNameSeparator);
            else
                Move(filePath, rollFileName);
        }

        private static string GetBackupFileName(string filePath, int fileDecimals, int index, string rollFileNameSeparator)
        {
            string fileExt = Path.GetExtension(filePath);
            if (Path.HasExtension(filePath))
            {
                return Path.Combine(Path.GetDirectoryName(filePath),
                    String.Format("{0}{3}{1}{2}", Path.GetFileNameWithoutExtension(filePath),
                    index.ToString(String.Format("D{0}", fileDecimals)), Path.GetExtension(filePath), rollFileNameSeparator));
            }
            else
                return Path.Combine(Path.GetDirectoryName(filePath),
                    String.Format("{0}{2}{1}", Path.GetFileNameWithoutExtension(filePath),
                    index.ToString(String.Format("D{0}", fileDecimals)), rollFileNameSeparator));
        }

        public static void Clean(string filePath)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");

            if (File.Exists(filePath))
            {
                using (StreamWriter file = new StreamWriter(filePath))
                {
                }
            }
        }

        public static bool TryMove(string sourceFilePath, string targetFilePath, bool overwrite = true)
        {
            ChoGuard.ArgumentNotNull(sourceFilePath, "SourceFilePath");
            ChoGuard.ArgumentNotNull(targetFilePath, "TargetFilePath");

            if (String.Compare(sourceFilePath, targetFilePath, true) == 0)
            {
                File.Move(sourceFilePath, targetFilePath);
            }
            else
            {
                if (File.Exists(sourceFilePath))
                {
                    if (File.Exists(targetFilePath))
                    {
                        if (overwrite)
                        {
                            File.Delete(targetFilePath);
                        }
                        else
                            return false;
                    }

                    File.Move(sourceFilePath, targetFilePath);
                }
            }
            return true;
        }

        public static void Move(string sourceFilePath, string targetFilePath, bool overwrite = true)
        {
            TryMove(sourceFilePath, targetFilePath, overwrite);
        }

        public static void WriteLine(string filePath, string msg)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");

            ChoDirectory.CreateDirectoryFromFilePath(filePath);

            using (StreamWriter Writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                Writer.WriteLine(msg);
        }

        public static void Write(string filePath, string msg)
        {
            ChoGuard.ArgumentNotNull(filePath, "FilePath");

			using (StreamWriter Writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                Writer.Write(msg);
        }

        public static string ReadToEnd(string fileName)
        {
            string foundFileName;
            if (!Exists(fileName, out foundFileName))
                throw new ApplicationException(String.Format("Can't find the `{0}` file.", fileName));

            using (StreamReader streamReader = File.OpenText(foundFileName))
                return streamReader.ReadToEnd();
        }

        public static bool Exists(string fileName)
        {
            string foundFileName;
            return Exists(fileName, out foundFileName);
        }

        public static bool Exists(string fileName, out string foundFileName)
        {
            foundFileName = null;

            if (Path.IsPathRooted(fileName))
            {
                if (File.Exists(fileName))
                {
                    foundFileName = fileName;
                    return true;
                }
            }
            else
            {
                //foreach (string providerPath in WorkflowManager.Paths)
                //{
                //    if (File.Exists(Path.Combine(providerPath, fileName)))
                //    {
                //        foundFileName = Path.Combine(providerPath, fileName);
                //        return true;
                //    }
                //}
            }
            return false;
        }

        public static void Delete(string path, string searchPattern)
        {
            foreach (string file in Directory.GetFiles(path, searchPattern))
                File.Delete(file);
        }

        public static byte[] ReadAllBytes(string path)
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long length = stream.Length;
                if (length > 0x7fffffff)
                    throw new ArgumentException("Reading more than 4gigs with this call is not supported");

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)length);
                return buffer;
            }
        }

        public static object GetObject(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            //ChoXmlSerializerSectionHandler xmlSerializerSectionHandler = new ChoXmlSerializerSectionHandler();
            //return xmlSerializerSectionHandler.Create(null, null, doc.DocumentElement);

            //TODO: Revisit
            return null; // new ChoXmlSerializerSection(doc.DocumentElement).Load(doc.DocumentElement);
        }

		/// <summary>
		/// Sets the passed file as readonly.
		/// </summary>
		/// <param name="filePath">The fully qualified name of a file, or the relative file name.</param>
		public static void SetReadOnly(string filePath)
		{
			SetReadOnly(filePath, true);
		}

		/// <summary>
		/// Sets the passed file to either readonly or not.
		/// </summary>
		/// <param name="filePath">The fully qualified name of a file, or the relative file name.</param>
		/// <param name="readOnly">true to make the file as readonly; otherwise, false.</param>
		public static void SetReadOnly(string filePath, bool readOnly)
		{
			ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");

			if (!File.Exists(filePath)) return;
			FileInfo myFile = new FileInfo(filePath);
			myFile.IsReadOnly = readOnly;
		}

		/// <summary>
		/// Gets a value that determines if the passed file is read only.
		/// </summary>
		/// <param name="filePath">The fully qualified name of a file, or the relative file name.</param>
		/// <returns>true if the current file is read only; otherwise, false.</returns>
		public static bool IsReadOnly(string filePath)
		{
			ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");

			if (!File.Exists(filePath))
				throw new ArgumentException(String.Format("'{0}' file not exists.", filePath));

			FileInfo myFile = new FileInfo(filePath);
			return myFile.IsReadOnly;
		}

		public static long GetFileSize(string filePath)
		{
			ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");

			if (!File.Exists(filePath))
				throw new ArgumentException(String.Format("'{0}' file not exists.", filePath));

			FileInfo myFile = new FileInfo(filePath);
			return myFile.Length;
		}

        #endregion
    }
}
