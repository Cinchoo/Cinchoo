namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Collections.Generic;

    #endregion NameSpaces

    [Flags]
    public enum ChoMethodDispatcherThrowException
    {
        Never = 0x00,
        WhenMethodExcutionCountExceedsOnly = 0x01,
        WhenMethodThrowsExceptionOnly = 0x02,
        Always = WhenMethodExcutionCountExceedsOnly | WhenMethodThrowsExceptionOnly
    }

    public class ChoMethodDispatcher : IChoMethodDispatcher, IDisposable
    {
        #region Instance Data Members (Private)

        private int _counter;

        private readonly int _maxCount = 1;
        private readonly Delegate _method;
        private readonly object _padLock = new object();
        private readonly ChoMethodDispatcherThrowException _methodDispatcherSilentMode;

        #endregion Instance Data Members (Private)

        public ChoMethodDispatcher(Delegate method)
            : this(method, 1, ChoMethodDispatcherThrowException.Never)
        {
        }

        public ChoMethodDispatcher(Delegate method, int count)
            : this(method, 1, ChoMethodDispatcherThrowException.Never)
        {
        }

        public ChoMethodDispatcher(Delegate method, int count, ChoMethodDispatcherThrowException methodDispatcherSilentMode)
        {
            ChoGuard.ArgumentNotNull(method, "Action");
            if (count < 0)
                throw new ArgumentException("Count must have at least 0.");

            _maxCount = count;
            _methodDispatcherSilentMode = methodDispatcherSilentMode;
            _method = method;
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region ISyncMethodExecuter Members

        public ChoMethodDispatcherResult Invoke(params object[] inputs)
        {
            int id = Interlocked.Increment(ref _counter);
            if (_maxCount != 0)
            {
                if (id > _maxCount)
                {
                    if ((_methodDispatcherSilentMode & ChoMethodDispatcherThrowException.WhenMethodExcutionCountExceedsOnly) == ChoMethodDispatcherThrowException.WhenMethodExcutionCountExceedsOnly)
                        return new ChoMethodDispatcherResult(id, ChoMethodDispatcherStates.Skip, null, inputs, null);
                    else
                        throw new ApplicationException();
                }
            }

            //Console.WriteLine("{0}. {1}", id, _method.ToString());
            lock (_padLock)
            {
                try
                {
                    object ouuput = _method.DynamicInvoke(inputs);
                }
                catch (ChoFatalApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if ((_methodDispatcherSilentMode & ChoMethodDispatcherThrowException.WhenMethodThrowsExceptionOnly) == ChoMethodDispatcherThrowException.WhenMethodThrowsExceptionOnly)
                        return new ChoMethodDispatcherResult(id, ChoMethodDispatcherStates.Fail, null, inputs, ex);
                    else
                        throw;
                }
            }

            return new ChoMethodDispatcherResult(id, ChoMethodDispatcherStates.Success, null, inputs, null);
        }

        #endregion
    }
}
