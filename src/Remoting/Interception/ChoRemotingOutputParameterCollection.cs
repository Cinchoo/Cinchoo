namespace eSquare.Core.Remoting.Interception
{
    #region NameSpaces

    using System.Reflection;
    using System.Runtime.Remoting.Messaging;

    using eSquare.Core.Aspect;

    #endregion NameSpaces

    namespace Microsoft.Practices.EnterpriseLibrary.PolicyInjection.RemotingInterception
    {
        /// <summary>
        /// A class that wraps the outputs of a <see cref="IMethodCallMessage"/> into the
        /// <see cref="IParameterCollection"/> interface.
        /// </summary>
        class ChoRemotingOutputParameterCollection : ChoParameterCollection
        {
            /// <summary>
            /// Constructs a new <see cref="RemotingOutputParameterCollection"/> that wraps the
            /// given method call and arguments.
            /// </summary>
            /// <param name="callMessage">The call message.</param>
            /// <param name="arguments">The arguments.</param>
            public ChoRemotingOutputParameterCollection(IMethodCallMessage callMessage, object[] arguments)
                : base(arguments, callMessage.MethodBase.GetParameters(),
                    delegate(ParameterInfo info) { return info.IsOut; })
            {
            }
        }
    }
}
