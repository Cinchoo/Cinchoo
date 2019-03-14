namespace eSquare.Core.Remoting.Interception
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;

    using eSquare.Core.Aspect;

    #endregion

    /// <summary>
    /// A class that wraps the inputs of a <see cref="IMethodCallMessage"/> into the
    /// <see cref="IParameterCollection"/> interface.
    /// </summary>
    class ChoRemotingInputParameterCollection : ChoParameterCollection
    {
        /// <summary>
        /// Constructs a new <see cref="RemotingInputParameterCollection"/> that wraps the
        /// given method call and arguments.
        /// </summary>
        /// <param name="callMessage">The call message.</param>
        /// <param name="arguments">The arguments.</param>
        public ChoRemotingInputParameterCollection(IMethodCallMessage callMessage, object[] arguments)
            : base(arguments, callMessage.MethodBase.GetParameters(),
                delegate(ParameterInfo info) { return !info.IsOut; })
        {
        }
    }
}
