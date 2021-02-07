namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Diagnostics;

    #endregion NameSpaces

    [Serializable]
	[DebuggerDisplay("Name={_name}")]
	public class ChoStreamProfile : ChoBaseProfile
    {
		#region Shared Data Members (Internal)

        //internal static IChoProfile GlobalProfile = new ChoStreamProfile("GLOBAL", "Application Logs...", true);

		#endregion Shared Data Members (Internal)

		#region Constrctors

		static ChoStreamProfile()
        {
        }

        public ChoStreamProfile(string msg)
            : base(msg, ChoProfile.GetContext(new StackFrame(1)))
		{
		}

        public ChoStreamProfile(string msg, IChoProfile outerProfile)
			: base(msg, outerProfile)
		{
		}

        public ChoStreamProfile(string name, string msg, IChoProfile outerProfile = null)
            : base(name, msg, outerProfile == null ?
                (name != ChoProfile.NULL_PROFILE_NAME && name != ChoProfile.DEFAULT_PROFILE_NAME ? ChoProfile.GetContext(new StackFrame(1)) : outerProfile) : outerProfile)
        {
        }

        internal ChoStreamProfile(bool condition, string name, string msg)
            : base(condition, name, msg, ChoProfile.GetContext(new StackFrame(1)))
        {
        }

        internal ChoStreamProfile(bool condition, string name, string msg, IChoProfile outerProfile, bool delayedStartProfile, string startActions, string stopActions)
			: base(condition, name, msg, outerProfile, delayedStartProfile, startActions, stopActions)
		{
        }

        #endregion Constrctors

		protected override void Flush()
		{
		}

		protected override void Write(string msg)
		{
			WriteToBackingStore(msg);
		}
	}
}
