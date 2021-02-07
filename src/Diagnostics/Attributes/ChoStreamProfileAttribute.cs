namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    using Cinchoo.Core.Reflection;
    using Cinchoo.Core.Diagnostics;
	using Cinchoo.Core.IO;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoStreamProfileAttribute : ChoProfileAttribute
	{
		#region Constructors

		public ChoStreamProfileAttribute(string message)
            : base(message)
        {
        }

        public ChoStreamProfileAttribute(bool condition, string message)
            : base(condition, message)
        {
        }

        #endregion Constructors

        #region ChoProfileAttribute Overrides (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
        {
			string message = null;
			if (!String.IsNullOrEmpty(Message))
				message = ChoPropertyManager.ExpandProperties(target, Message);

            IChoProfile profile = null;

            if (ChoProfile.TryGetProfile(Name, ref profile,
                () => new ChoStreamProfile(Condition, Name, message, (ChoBaseProfile)outerProfile, false, StartActions, StopActions)))
                return profile;
            else
			    return null;
        }

        #endregion ChoProfileAttribute Overrides (Public)
    }
}
