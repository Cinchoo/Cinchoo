namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Globalization;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoGlobalAttributes
    {
        #region Shared Properties

        private static CultureInfo _cultureInfo = new CultureInfo(CultureInfo.InstalledUICulture.ToString());

        public static CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
        }

        #endregion
    }
}
