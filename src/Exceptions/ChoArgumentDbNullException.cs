using System;
using System.Collections.Generic;
using System.Text;

namespace eSquare.Core.Exceptions
{
    [Serializable, ComVisible(true)]
    public class ChoArgumentDbNullException : ArgumentException
    {
        // Methods
        public ChoArgumentDbNullException()
            : base(Environment.GetResourceString("ArgumentNull_Generic"))
        {
            base.SetErrorCode(-2147467261);
        }

        public ChoArgumentDbNullException(string paramName)
            : base(Environment.GetResourceString("ArgumentNull_Generic"), paramName)
        {
            base.SetErrorCode(-2147467261);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        protected ChoArgumentDbNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ChoArgumentDbNullException(string message, Exception innerException)
            : base(message, innerException)
        {
            base.SetErrorCode(-2147467261);
        }

        public ChoArgumentDbNullException(string paramName, string message)
            : base(message, paramName)
        {
            base.SetErrorCode(-2147467261);
        }
    }

}
