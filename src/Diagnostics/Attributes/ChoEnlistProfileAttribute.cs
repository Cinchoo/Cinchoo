namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ChoEnlistProfileAttribute : ChoProfileAttribute
    {
        #region Constructors

        public ChoEnlistProfileAttribute()
            : base(String.Empty)
        {
        }

        #endregion Constructors

        #region ChoProfileAttribute Overrides (Public)

		public override IChoProfile ConstructProfile(object target, IChoProfile outerProfile)
		{
			throw new NotImplementedException();
		}

        #endregion ChoProfileAttribute Overrides (Public)
	}
}
