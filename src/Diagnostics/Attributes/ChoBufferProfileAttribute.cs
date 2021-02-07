namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ChoBufferProfileAttribute : ChoProfileAttribute
    {
        #region Constructors

        public ChoBufferProfileAttribute(string message)
            : base(message)
        {
        }

        public ChoBufferProfileAttribute(bool condition, string message)
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
                () => new ChoBufferProfile(Condition, Name, message, (ChoBaseProfile)outerProfile, false, StartActions, StopActions)))
                return profile;
            else
                return null;
        }

        #endregion ChoProfileAttribute Overrides (Public)
    }
}
 