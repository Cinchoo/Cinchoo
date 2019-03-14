namespace Cinchoo.Core.Pattern.Singleton
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoSingletonException : ApplicationException
    {
        public ChoSingletonException()
            : base()
        {
        }

        public ChoSingletonException(string message)
            : base(message)
        {
        }

        public ChoSingletonException(string message, Exception e)
            : base(message, e)
        {
        }

        public ChoSingletonException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
