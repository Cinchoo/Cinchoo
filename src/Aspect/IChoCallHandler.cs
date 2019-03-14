namespace eSquare.Core.Aspect
{
    #region NameSpaces
    
    using System;
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;
    using System.Text;

    #endregion NameSpaces

    /// <summary>
    /// Handlers implement this interface and are called for each
    /// invocation of the pipelines that they're included in.
    /// </summary>
    public interface IChoCallHandler
    {
        /// <summary>
        /// Implement this method to execute your handler processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the handler
        /// chain.</param>
        /// <returns>Return value from the target.</returns>
        IChoMethodReturn Invoke(IChoMethodInvocation input, GetNextHandlerDelegate getNext);

        /// <summary>
        /// Order in which the handler will be executed
        /// </summary>
        int Order { get; set; }
    }

    /// <summary>
    /// This delegate type is the type that points to the next
    /// method to execute in the current pipeline.
    /// </summary>
    /// <param name="input">Inputs to the current method call.</param>
    /// <param name="getNext">Delegate to get the next handler in the chain.</param>
    /// <returns>Return from the next method in the chain.</returns>
    public delegate IChoMethodReturn InvokeHandlerDelegate(IChoMethodInvocation input, GetNextHandlerDelegate getNext);

    /// <summary>
    /// This delegate type is passed to each handler's Invoke method.
    /// Call the delegate to get the next delegate to call to continue
    /// the chain.
    /// </summary>
    /// <returns>Next delegate in the handler chain to call.</returns>
    public delegate InvokeHandlerDelegate GetNextHandlerDelegate();
}
