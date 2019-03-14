namespace Cinchoo.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    #endregion NameSpaces

    public interface IChoCustomConfigurationSectionHandler : IChoStandardConfigurationSectionHandler
    {
        string Parameters { get; set; }
    }
}
