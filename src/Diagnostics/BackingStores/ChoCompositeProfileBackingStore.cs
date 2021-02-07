namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoCompositeProfileBackingStore : IChoProfileBackingStore
    {
        #region Instance Data Members (Private)

        private readonly IChoProfileBackingStore[] _profileBackingStores;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoCompositeProfileBackingStore(IChoProfileBackingStore[] profileBackingStores)
        {
            _profileBackingStores = profileBackingStores;
        }
        
        #endregion Constructors

        #region IChoProfileBackingStore Members

        public void Start(string actionCmds)
        {
            //using (ChoConsoleSession session = new ChoConsoleSession(ConsoleColor.DarkGreen))
            //    session.WriteLine("Starting...");
        }

        public void Stop(string actionCmds)
        {
            //using (ChoConsoleSession session = new ChoConsoleSession(ConsoleColor.DarkBlue))
            //    session.WriteLine("Stopping...");
        }

        public void Write(string msg, object tag)
        {
            _profileBackingStores.ForEach((s) => s.Write(msg, tag));
        }

        #endregion
    }
}
