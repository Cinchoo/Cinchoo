using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cinchoo.Core
{
    [Serializable]
    public class ChoDbException : ChoApplicationException
    {
        public ChoDbException()
            : base()
        {
        }

        public ChoDbException(string message)
            : base(message)
        {
        }

        public ChoDbException(string message, Exception e)
            : base(message, e)
        {
        }

        protected ChoDbException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
