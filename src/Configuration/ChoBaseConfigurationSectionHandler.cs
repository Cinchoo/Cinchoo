namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Specialized;

    using eSquare.Core.IO;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Collections.Specialized;

    #endregion

    public abstract class ChoBaseConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        ///		Creates a new configuration handler and adds the specified configuration object to the collection.
        /// </summary>
        /// <param name="parent">Composed from the configuration settings in a corresponding parent configuration section.</param>
        /// <param name="context">Provides access to the virtual path for which the configuration section handler computes configuration values. Normally this parameter is reserved and is null.</param>
        /// <param name="section">The XML node that contains the configuration information to be handled. section provides direct access to the XML contents of the configuration section.</param>
        /// <returns></returns>
        public abstract object Create(object parent, object context, XmlNode section);
    }
}
