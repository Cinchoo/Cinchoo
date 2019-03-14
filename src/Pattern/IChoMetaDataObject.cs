namespace Cinchoo.Core.Pattern
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces

    public interface IChoMetaDataObject<TMetaDataInfo>
        where TMetaDataInfo : IEquatable<TMetaDataInfo>, new()
    {
        string Name
        {
            get;
        }
        void SetMetaData(TMetaDataInfo metaDataInfo);
        string NodeLocateXPath
        {
            get;
        }
        string NodeCreateXPath
        {
            get;
        }
        TMetaDataInfo MetaDataInfo
        {
            get;
        }
    }
}
