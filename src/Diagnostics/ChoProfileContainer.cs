namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core.Collections.Generic;

    #endregion NameSpaces

    public abstract class ChoProfileContainer : ChoObject
    {
        #region Instance Data Members (Private)

        private ChoDictionary<string, IChoProfile> _childProfiles = ChoDictionary<string, IChoProfile>.Synchronized(new ChoDictionary<string, IChoProfile>());

        #endregion Instance Data Members (Private)

		#region Constructors

		public ChoProfileContainer()
			: base(true)
		{
		}

		#endregion Constructors

		#region Instance Members (Public)

		internal virtual void Add(IChoProfile profile)
        {
            ChoGuard.ArgumentNotNull(profile, "Profile");

            if (_childProfiles.ContainsKey(profile.ProfilerName)) return;
            _childProfiles.Add(profile.ProfilerName, profile);
        }

		internal virtual void Remove(IChoProfile profile)
        {
            ChoGuard.ArgumentNotNull(profile, "Profile");

            lock (_childProfiles.SyncRoot)
            {
                _Remove(profile);
            }
        }

		internal virtual void Clear()
        {
            if (_childProfiles != null)
            {
                lock (_childProfiles.SyncRoot)
                {
                    foreach (IChoProfile profile in _childProfiles.ToValuesArray())
                    {
                        profile.Dispose();
                        _Remove(profile);
                    }
                }
            }
        }

        #endregion Instance Members (Public)

        #region Instance Members (Private)

        private void _Remove(IChoProfile profile)
        {
            if (!_childProfiles.ContainsKey(profile.ProfilerName)) return;

            if (_childProfiles[profile.ProfilerName] is IDisposable)
                ((IDisposable)_childProfiles[profile.ProfilerName]).Dispose();

            _childProfiles.Remove(profile.ProfilerName);
        }

        #endregion Instance Members (Private)
    }
}
