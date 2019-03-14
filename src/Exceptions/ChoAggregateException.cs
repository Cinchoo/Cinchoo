namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cinchoo.Core.Collections.Generic;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoAggregateException : Exception
    {
        #region Instance Members (Private)

        private readonly ChoList<Exception> _innerExceptions = new ChoList<Exception>();

        #endregion Instance Members (Private)

        #region Constructors

        public ChoAggregateException()
            : base()
        {
        }

        public ChoAggregateException(string message)
            : base(message, null)
        {
        }

        public ChoAggregateException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoAggregateException(SerializationInfo info, StreamingContext ctx)
            : base(info, ctx)
        {
        }

        public ChoAggregateException(params Exception[] innerExceptions)
            : this(string.Empty, innerExceptions)
        {
        }

        public ChoAggregateException(string message, params Exception[] innerExceptions)
            : this(message, (IEnumerable<Exception>)innerExceptions)
        {
        }

        public ChoAggregateException(IEnumerable<Exception> innerExceptions)
            : this(string.Empty, innerExceptions)
        {
        }

        public ChoAggregateException(string message, IEnumerable<Exception> inner)
            : base(GetFormattedMessage(message, inner))
        {
            _innerExceptions.AddRange(inner);
        }

        #endregion Constructors

        #region Instance Members (Public)

        public ChoAggregateException Flatten()
        {
            List<Exception> inner = new List<Exception>();

            foreach (Exception e in _innerExceptions)
            {
                ChoAggregateException aggEx = e as ChoAggregateException;
                if (aggEx != null)
                {
                    inner.AddRange(aggEx.Flatten().InnerExceptions);
                }
                else
                {
                    inner.Add(e);
                }
            }

            return new ChoAggregateException(inner);
        }

        public void Handle(Func<Exception, bool> handler)
        {
            List<Exception> failed = new List<Exception>();
            foreach (var e in _innerExceptions)
            {
                try
                {
                    if (!handler(e))
                        failed.Add(e);
                }
                catch
                {
                    throw new ChoAggregateException(failed);
                }
            }
            if (failed.Count > 0)
                throw new ChoAggregateException(failed);
        }

        public ChoList<Exception> InnerExceptions
        {
            get
            {
                return ChoList<Exception>.AsReadOnly(_innerExceptions);
            }
        }

        public override string ToString()
        {
            return this.Message;
        }

        #endregion Instance Members (Public)

        #region Shared Members (Private)

        private const string baseMessage = "Exception(s) occurred : {0}.";
        private static string GetFormattedMessage(string customMessage, IEnumerable<Exception> inner)
        {
            StringBuilder finalMessage = new System.Text.StringBuilder(string.Format(baseMessage, customMessage));
            foreach (Exception e in inner)
            {
                finalMessage.Append(Environment.NewLine);
                finalMessage.Append("[ ");
                finalMessage.Append(e.ToString());
                finalMessage.Append(" ]");
                finalMessage.Append(Environment.NewLine);
            }
            return finalMessage.ToString();
        }

        #endregion Shared Members (Private)
    }
}