namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Runtime.Remoting.Proxies;
	using Cinchoo.Core.Diagnostics;
	using System.IO;
	using Cinchoo.Core.IO;

	#endregion NameSpaces

    public abstract class ChoProxyAttribute : ProxyAttribute
    {
        #region Instance Properties (Public)

        protected readonly object SyncRoot = new object();

        #endregion Instance Properties (Public)

        #region Shared Members (Public)

        public static T GetMe<T>(object target) where T : Attribute
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            return GetMe<T>(target.GetType());
        }

        public static T GetMe<T>(Type type) where T : Attribute
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            return ChoType.GetAttribute<T>(type);
        }

        #endregion Shared Members (Public)
    }
}
