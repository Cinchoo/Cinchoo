namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;

    #endregion NameSpaces

    public enum ChoMethodDispatcherStates { Success, Fail, Skip }

    public struct ChoMethodDispatcherResult
    {
        private readonly int _id;
        private readonly ChoMethodDispatcherStates _methodDispatcherState;
        private readonly object _returnValue;
        private readonly object[] _inputValues;
        private readonly string _threadName;
        private readonly Exception _exception;

        internal ChoMethodDispatcherResult(int id, ChoMethodDispatcherStates methodDispatcherState, object retValue, object[] inputValues, Exception exception)
        {
            _id = id;
            _methodDispatcherState = methodDispatcherState;
            _returnValue = retValue;
            _inputValues = inputValues;
            _threadName = Thread.CurrentThread.Name;
            _exception = exception;
        }

        public bool IsExecutedSuccessfully
        {
            get { return _methodDispatcherState == ChoMethodDispatcherStates.Success; }
        }

        public ChoMethodDispatcherStates MethodDispatcherState
        {
            get { return _methodDispatcherState; }
        }

        public object ReturnValue
        {
            get { return _returnValue; }
        }

        public object[] InputValues
        {
            get { return _inputValues; }
        }

        public int Id
        {
            get { return _id; }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}, {2}", Id, _threadName, IsExecutedSuccessfully);
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }
}
