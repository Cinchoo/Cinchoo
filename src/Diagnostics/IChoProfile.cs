namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    #endregion NameSpaces

    // Summary:
    //     Specifies how the operating system should open a file.
    [Serializable]
    [ComVisible(true)]
    public enum ChoProfileIntializationAction { Truncate, Append, Roll, Delete }

    #region IChoProfile Interface

    public interface IChoProfile : IDisposable
    {
        string ProfilerName { get; }
        int Indent { get; }
        TimeSpan ElapsedTimeTaken { get; }

        void AppendIf(bool condition, Exception ex);
        void Append(Exception ex);

        string AppendIf(bool condition, string msg);
        string AppendIf(bool condition, string format, params object[] args);

        string Append(string msg);
        string Append(string format, params object[] args);

        string AppendLineIf(bool condition, string msg);
        string AppendLineIf(bool condition, string format, params object[] args);

        string AppendLine(string msg);
        string AppendLine(string format, params object[] args);
    }

    #endregion IChoProfile Interface
}
