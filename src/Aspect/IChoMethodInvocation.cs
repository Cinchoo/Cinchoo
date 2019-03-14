namespace eSquare.Core.Aspect
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Reflection;

    #endregion NameSpaces

    /// <summary>
    /// This interface is used to represent the call to a method.
    /// An implementation of IMethodInvocation is passed to the
    /// call handlers so that they may manipulate the call
    /// (typically by changing the parameters) before the final target
    /// gets called.
    /// </summary>
    public interface IChoMethodInvocation
    {
        /// <summary>
        /// Gets the inputs for this call.
        /// </summary>
        IChoParameterCollection Inputs { get; }

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref.
        /// </summary>
        IChoParameterCollection Arguments { get; }

        /// <summary>
        /// Retrieves a dictionary that can be used to store arbitrary additional
        /// values. This allows the user to pass values between call handlers.
        /// </summary>
        IDictionary InvocationContext { get; }

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// The method on Target that we're aiming at.
        /// </summary>
        MethodBase MethodBase { get; }

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn.
        /// </summary>
        /// <param name="returnValue">Return value to be placed in the IMethodReturn object.</param>
        /// <param name="outputs">All arguments passed or returned as out/byref to the method. 
        /// Note that this is the entire argument list, including in parameters.</param>
        /// <returns>New IMethodReturn object.</returns>
        IChoMethodReturn CreateMethodReturn(object returnValue, params object[] outputs);

        /// <summary>
        /// Factory method that creates the correct implementation of
        /// IMethodReturn in the presence of an exception.
        /// </summary>
        /// <param name="ex">Exception to be set into the returned object.</param>
        /// <returns>New IMethodReturn object</returns>
        IChoMethodReturn CreateExceptionMethodReturn(Exception ex);

    }
}

