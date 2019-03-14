namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public delegate TResult ChoFunc<TResult>();
    public delegate TResult ChoFunc<T, TResult>(T arg);
    public delegate TResult ChoFunc<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TResult ChoFunc<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult ChoFunc<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate void ChoAction();
    public delegate void ChoAction<T>(T arg);
    public delegate void ChoAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate void ChoAction<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void ChoAction<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
