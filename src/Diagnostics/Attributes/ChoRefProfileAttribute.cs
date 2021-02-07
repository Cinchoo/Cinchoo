namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ChoRefProfileAttribute : ChoProfileAttribute
    {
        #region Instance Data Members (Private)

        private readonly string _refProfileName;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoRefProfileAttribute(string refProfileName)
            : base(null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(refProfileName, "refProfileName");

            _refProfileName = refProfileName;
        }

        #endregion Constructors

        #region ChoProfileAttribute Overrides (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
        {
            IChoProfile profile = null;

            if (ChoProfile.TryGetProfile(_refProfileName, ref profile, null))
                return profile;
            else
                throw new ChoApplicationException("{0} profile not exists.".FormatString(_refProfileName));
        }

        #endregion ChoProfileAttribute Overrides (Public)
    }
}
 