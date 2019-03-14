namespace eSquare.Core.Diagnostics.Attributes
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;

    using eSquare.Core.Reflection;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Diagnostics.Settings;
    using eSquare.Core.Property;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ChoApplicationProfileAttribute : ChoProfileAttribute
    {
        #region Instance Data Members (Private)

        private object _padLock = new object();

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoApplicationProfileAttribute(string message)
            : base(message)
        {
        }

        public ChoApplicationProfileAttribute(bool condition, string message)
            : base(condition, message)
        {
        }

        #endregion Constructors

        #region Instance Members (Public)

        public override IChoProfile ConstructProfile(IChoProfile outerProfile)
        {
            lock (_padLock)
            {
                string fileName = ChoPropertyManager.Translate(FileName);
                string directory = ChoPropertyManager.Translate(Directory);
                string message = ChoPropertyManager.Translate(Message);

                ChoBufferProfile bufferProfile = null;
                if (String.IsNullOrEmpty(fileName))
                    bufferProfile = new ChoBufferProfile(Condition, (StreamWriter)null, message, outerProfile, false);
                else
                    bufferProfile = new ChoBufferProfile(Condition, ChoFileProfileSettings.GetFullPath(directory, fileName), Mode, message, outerProfile, false);

                return DelayedAutoStart ? ChoBufferProfile.DelayedAutoStart(bufferProfile) : bufferProfile;
            }
        }

        #endregion Instance Members (Public)
    }
}
