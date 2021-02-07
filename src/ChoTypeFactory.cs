namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    [ChoTypeFormatter("Type Factory")]
    [ChoObjectFactory(ChoObjectConstructionType.Singleton)]
	[ChoBufferProfile("Configured types with short name....", NameFromTypeFullName = typeof(ChoTypeFactory))]
    [ChoEnlistProfile(NameFromTypeFullName = typeof(ChoTypeFactory))]
    public class ChoTypeFactory : /* ChoConfigurableObject, */ IChoObjectInitializable
    {
        #region Instance Data Members (Private)

        private object _padLock = new object();
        private ChoDictionary<string, string> _typeShortNameMap = ChoDictionary<string, string>.Synchronized(new ChoDictionary<string, string>());

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoTypeFactory()
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

        public string this[string typeShortName]
        {
            get
            {
                ChoGuard.ArgumentNotNullOrEmpty(typeShortName, "TypeShortName");
                lock (_padLock)
                {
                    return _typeShortNameMap.ContainsKey(typeShortName) ? _typeShortNameMap[typeShortName] : null;
                }
            }
        }

        #endregion Instance Members (Public)

        #region Shared Properties

        public static ChoTypeFactory Me
        {
            get { return ChoObjectManagementFactory.CreateInstance(typeof(ChoTypeFactory)) as ChoTypeFactory; }
        }

        #endregion Shared Properties

        #region IChoObjectInitializable Members

        public bool Initialize(bool beforeFieldInit, object state)
        {
            if (beforeFieldInit) return false;

			//REVISIT
            //ChoProfile.InitializeProfile(GetType());

            ChoDictionary<string, string> typeShortNameMap = ChoDictionary<string, string>.Synchronized(new ChoDictionary<string, string>());
            using (ChoBufferProfileEx errProfile = ChoBufferProfileEx.DelayedAutoStart(new ChoBufferProfileEx(true, ChoPath.AddExtension(typeof(ChoTypeFactory).FullName, ChoReservedFileExt.Err), "Below are the duplicate type short names founds...")))
            {
                foreach (Type type in ChoType.GetTypes(typeof(ChoTypeShortNameAttribute)))
                {
                    ChoTypeShortNameAttribute typeShortNameAttribute = ChoType.GetAttribute<ChoTypeShortNameAttribute>(type);
                    AddToMap(typeShortNameMap, errProfile, type, typeShortNameAttribute.Name, false);
                }
                if (ChoShortTypeNameSettings.Me.ValidTypsShortNames != null)
                {
                    foreach (ChoTypsShortName typsShortName in ChoShortTypeNameSettings.Me.ValidTypsShortNames)
                    {
                        if (typsShortName.Type == null) continue;
                        AddToMap(typeShortNameMap, errProfile, typsShortName.Type, typsShortName.TypeShortName, typsShortName.Override);
                    }
                }
            }
            lock (_padLock)
            {
                _typeShortNameMap = typeShortNameMap;
            }

            return false;
        }

        private void AddToMap(ChoDictionary<string, string> typeShortNameMap, ChoBufferProfileEx errProfile, Type type, string typeShortName, bool overrideType)
        {
            if (typeShortNameMap.ContainsKey(typeShortName))
            {
                if (!overrideType)
                {
                    errProfile.AppendLine(String.Format("Type: {0}, ShortName: {1}", type.SimpleQualifiedName(), typeShortName));
                    return;
                }
                else
                {
                    ChoTrace.Debug(String.Format("DELETED: Type: {0}, ShortName: {1}", typeShortNameMap[typeShortName], typeShortName));
                    typeShortNameMap.Remove(typeShortName);
                }
            }

            typeShortNameMap.Add(typeShortName, type.SimpleQualifiedName());

            ChoTrace.Debug(String.Format("ADDED: Type: {0}, ShortName: {1}", type.SimpleQualifiedName(), typeShortName));
        }

        #endregion
    }
}
