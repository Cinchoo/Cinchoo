namespace Cinchoo.Core.IO
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Web;
    using System.Text;
    using System.Configuration;
    using System.Security.Cryptography;
    using System.Collections.Specialized;

    using Cinchoo.Core;
    using Cinchoo.Core.Threading;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO.IsolatedStorage;
    using System.Text.RegularExpressions;
    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Win32;

    #endregion NameSpaces

    //public class ChoPathEventArgs
    //{
    //    public string BaseDirectory;
    //    public string FilePath;
    //}

    public static class ChoPath
    {
        #region Shared Data Members (Private)

        private static readonly string _pathValidatorExpression = "^[^" + string.Join("", Array.ConvertAll(Path.GetInvalidPathChars(), x => Regex.Escape(x.ToString()))) + "]+$";
        private static readonly Regex _pathValidator = new Regex(_pathValidatorExpression, RegexOptions.Compiled);

        private static readonly string _fileNameValidatorExpression = "^[^" + string.Join("", Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString()))) + "]+$";
        private static readonly Regex _fileNameValidator = new Regex(_fileNameValidatorExpression, RegexOptions.Compiled);

        private static readonly string _pathCleanerExpression = "[" + string.Join("", Array.ConvertAll(Path.GetInvalidPathChars(), x => Regex.Escape(x.ToString()))) + "]";
        private static readonly Regex _pathCleaner = new Regex(_pathCleanerExpression, RegexOptions.Compiled);

        private static readonly string _fileNameCleanerExpression = "[" + string.Join("", Array.ConvertAll(Path.GetInvalidFileNameChars(), x => Regex.Escape(x.ToString()))) + "]";
        private static readonly Regex _fileNameCleaner = new Regex(_fileNameCleanerExpression, RegexOptions.Compiled);

        private static readonly string _assemblyBaseDirectory = null;

        //public static event EventHandler<ChoEventArgs<ChoPathEventArgs>> ResolveFullPath;

        #endregion Shared Data Members (Private)

        #region Constructors

        static ChoPath()
        {
            try
            {
                HttpContext ctx = HttpContext.Current;
                if (ctx == null)
                    _assemblyBaseDirectory = Path.GetDirectoryName(ChoAssembly.GetEntryAssembly().Location);
                else
                    _assemblyBaseDirectory = HttpContext.Current.Request.PhysicalApplicationPath;
            }
            catch (ChoFatalApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ChoApplication.Trace(true, ex.ToString());
            }
        }

        #endregion Constructors

        #region Shared Member Functions (Public)

        public static string ToShortName(string longName)
        {
            if (longName.IsNullOrWhiteSpace()) return longName;

            StringBuilder s = new StringBuilder(1000);
            longName = ChoPath.GetFullPath(longName);
            uint iSize = (uint)s.Capacity;
            uint iRet = ChoKernel32.GetShortPathName(longName, s, iSize);
            return s.ToString();
        }

        public static string ToShortFileName(string filePath)
        {
            if (filePath.IsNullOrWhiteSpace()) return filePath;

            StringBuilder s = new StringBuilder(1000);
            filePath = ChoPath.GetFullPath(filePath);
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            uint iSize = (uint)s.Capacity;
            uint iRet = ChoKernel32.GetShortPathName(directory, s, iSize);
            return Path.Combine(s.ToString(), fileName);
        }

        public static string AssemblyBaseDirectory
        {
            get 
            {
                return _assemblyBaseDirectory; 
                //ChoPathEventArgs e = new ChoPathEventArgs();
                //ResolveFullPath.Raise(null, new ChoEventArgs<ChoPathEventArgs>(e));

                //if (e.BaseDirectory.IsNullOrWhiteSpace())
                //    return _assemblyBaseDirectory;
                //else if (Path.IsPathRooted(e.BaseDirectory))
                //    return e.BaseDirectory;
                //else
                //    return Path.Combine(_assemblyBaseDirectory, e.BaseDirectory);
            }
        }

        /// <summary>
		/// Get relative path for a given file path
		/// </summary>
		/// <param name="baseDirectory">Base directory</param>
		/// <param name="filePath">A file path</param>
		/// <returns>A relative path location</returns>
		public static string GetRelativePath(string baseDirectory, string filePath)
		{
			ChoGuard.ArgumentNotNullOrEmpty(baseDirectory, "BaseDirectory");
			ChoGuard.ArgumentNotNullOrEmpty(filePath, "FilePath");

			if (!baseDirectory.EndsWith("\\"))
				baseDirectory += "\\";

			System.Uri uri1 = new Uri(filePath);
			System.Uri uri2 = new Uri(baseDirectory);

			Uri relativeUri = uri2.MakeRelativeUri(uri1);

			return relativeUri.ToString();
		}

        public static string ChangeExtension(string filePath, string extension)
        {
            if (String.IsNullOrEmpty(filePath)) return filePath;
            if (String.IsNullOrEmpty(extension)) return filePath;

            string currentExtension = Path.GetExtension(filePath);
            if (!String.IsNullOrEmpty(currentExtension) && ChoReservedFileExt.IsValidExtension(currentExtension))
                return Path.ChangeExtension(filePath, extension);
            else
                return AddExtension(filePath, extension);
        }

        public static string ReplaceExtension(string filePath, string extension)
        {
            if (String.IsNullOrEmpty(filePath))
                return filePath;
            if (String.IsNullOrEmpty(extension))
                return filePath;

            string currentExtension = Path.GetExtension(filePath);
            if (!String.IsNullOrEmpty(currentExtension))
                return Path.ChangeExtension(filePath, extension);
            else
                return AddExtension(filePath, extension);
        }

        public static string AddExtension(string filePath)
        {
            return AddExtension(filePath, ChoReservedFileExt.Log);
        }

        public static string AddExtension(string filePath, string extension)
        {
            if (String.IsNullOrEmpty(filePath)) return filePath;

            return string.Format("{0}.{1}", filePath, extension);
        }

        public static string GetRandomFileName()
        {
            byte[] data = new byte[8];
            byte[] srcData = new DESCryptoServiceProvider().Key;
            Array.Copy(srcData, 0, data, 0, srcData.Length > 8 ? 8 : srcData.Length);
            char[] chArray = ChoIsolatedStorage.ToBase32StringSuitableForDirName(data).ToCharArray();
            chArray[8] = '.';
            return new string(chArray, 0, 12);
        }

        public static string GetRandomFileName(string directory)
        {
            ChoGuard.ArgumentNotNullOrEmpty(directory, "Directory");

            if (!Directory.Exists(directory))
                throw new ChoApplicationException(String.Format("'{0}' directory not found.", directory));
        
            string fileName = null;
            do
            {
                fileName = Path.Combine(directory, ChoPath.GetRandomFileName());

                if (File.Exists(fileName))
                    continue;
                else
                    break;
            }
            while (true);

            return fileName;
        }

        public static string GetPathWithoutDriveOrNetworkShare(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return path;

            string pathWithoutDriveOrNetworkShare = path; //  
            
            if ((path.IndexOf(":") == 1))
                pathWithoutDriveOrNetworkShare = path.Substring(Path.GetPathRoot(path).Length);
            else if (path.IndexOf("\\\\") == 0)
                pathWithoutDriveOrNetworkShare = path.Substring(path.IndexOf("\\\\") + 2);

            //if (pathWithoutDriveOrNetworkShare.IndexOf("\\") > 0)
            //    pathWithoutDriveOrNetworkShare = pathWithoutDriveOrNetworkShare.Substring(pathWithoutDriveOrNetworkShare.IndexOf("\\"));

            return pathWithoutDriveOrNetworkShare;
        }

        public static string GetDriveOrNetworkShare(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return path;

            string pathWithoutDriveOrNetworkShare = path; // = path.Substring(Path.GetPathRoot(path).Length);

            if ((path.IndexOf(":") == 1) || (path.IndexOf("\\\\") == 0))
                pathWithoutDriveOrNetworkShare = path.Substring(2); 
        
            if (pathWithoutDriveOrNetworkShare.IndexOf("\\") > 0)
                pathWithoutDriveOrNetworkShare = pathWithoutDriveOrNetworkShare.Substring(pathWithoutDriveOrNetworkShare.IndexOf("\\"));

            return pathWithoutDriveOrNetworkShare;
        }

        public static string GetFullPath(string path, string baseDirectory = null)
        {
			if (path.IsNullOrWhiteSpace())
				return path;

            if (Path.IsPathRooted(path))
                return path;
            else
            {
                if (baseDirectory.IsNull())
                    baseDirectory = _assemblyBaseDirectory;

                if (baseDirectory.IsNull())
                    return Path.GetFullPath(path);
                else
                {
                    if (!Path.IsPathRooted(baseDirectory))
                        baseDirectory = Path.Combine(_assemblyBaseDirectory, baseDirectory);

                    return Path.Combine(baseDirectory, path);
                }

                //ChoPathEventArgs e = new ChoPathEventArgs() { FilePath = path };
                //ResolveFullPath.Raise(null, new ChoEventArgs<ChoPathEventArgs>(e));

                //if (e.FilePath.IsNullOrWhiteSpace())
                //    e.FilePath = path;
                //if (e.BaseDirectory.IsNullOrWhiteSpace())
                //    e.BaseDirectory = _assemblyBaseDirectory;
                
                //if (!Path.IsPathRooted(e.BaseDirectory))
                //    e.BaseDirectory = Path.Combine(_assemblyBaseDirectory, e.BaseDirectory);

                //if (Path.IsPathRooted(e.FilePath))
                //    return e.FilePath;
                //else if (!e.BaseDirectory.IsNullOrEmpty())
                //    return Path.Combine(e.BaseDirectory, e.FilePath);
                //else
                //    return Path.GetFullPath(e.FilePath);
            }
        }

        public static string GetTempFileName()
        {
            try
            {
                return Path.GetTempFileName();
            }
            catch
            {
                //Path.GetTempFileName() funtion has limitation of creating temp files. At one point,
                //will throw "File Exists" exception when it reaches the limit.
                //To avoid this error, periotically clean the temp directory before calling this function
                using (ChoAppSyncMethodExecuter syncMethodInvoke = new ChoAppSyncMethodExecuter(
                    delegate(object state)
                    {
                        try
                        {
                            using (ChoProfile profile = new ChoProfile(String.Format("Temp directory [{0}] is full. Cleaning temp files...", Path.GetTempPath())))
                                ChoFile.Delete(Path.GetTempPath(), "tmp*.tmp");
                        }
                        catch { }

                        return null;
                    }
                    ))
                {
                }

                return Path.GetTempFileName();
            }
        }

        public static bool IsValidPath(string path)
        {
            return _pathValidator.IsMatch(path);
        }

        public static bool IsValidFileName(string fileName)
        {
            return _fileNameValidator.IsMatch(fileName);
        }

        public static string CleanPath(string path)
        {
            return _pathCleaner.Replace(path, "_");
        }

        public static string CleanFileName(string fileName)
        {
            return _fileNameCleaner.Replace(fileName, "_");
        }

        public static bool IsDirectory(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return false;

            path = ChoPath.GetFullPath(path);
            if (File.Exists(path))
                return false;
            else if (Directory.Exists(path))
                return true;
            else
                return Path.GetExtension(Path.GetFileName(path)).IsNullOrEmpty();
        }

        public static bool IsFile(string path)
        {
            return !IsDirectory(path);
        }

        public static bool HasExtension(string filePath, string extension)
        {
            ChoGuard.ArgumentNotNullOrEmpty(extension, "Extension");
            if (filePath.IsNullOrWhiteSpace()) return false;
            if (extension.StartsWith("."))
                extension = extension.Substring(1);

            return Path.GetExtension(filePath) == ".{0}".FormatString(extension);
        }

        #endregion
    }
}
