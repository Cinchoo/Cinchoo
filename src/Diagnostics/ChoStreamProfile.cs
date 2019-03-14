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

		public ChoStreamProfile(string msg) : base(msg)
		{
		}

		public ChoStreamProfile(string msg, ChoBaseProfile outerProfile)
			: base(msg, outerProfile)
		{
		}

        public ChoStreamProfile(string name, string msg)
			: base(name, msg)
        {
        }

        public ChoStreamProfile(bool condition, string name, string msg)
			: base(condition, name, msg)
        {
        }

		internal ChoStreamProfile(bool condition, string name, string msg, ChoBaseProfile outerProfile, bool delayedStartProfile, string startActions, string stopActions)
			: base(condition, name, msg, outerProfile, delayedStartProfile, startActions, stopActions)
		{
        }
		
		internal ChoStreamProfile(string name, string msg, bool dummy) : base(name, msg, dummy)
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
