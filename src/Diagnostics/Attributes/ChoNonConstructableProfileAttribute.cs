namespace eSquare.Core.Diagnostics.Attributes
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    using eSquare.Core.Reflection;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.IO;
    using eSquare.Core.Property;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class ChoNonConstructableProfileAttribute : ChoBaseProfileAttribute
    {
        #region Constructors

        public ChoNonConstructableProfileAttribute(bool condition, string message) : base(condition, message)
        {
        }

        public ChoNonConstructableProfileAttribute(string message)
            : this(ChoTrace.ChoSwitch.TraceVerbose, message)
        {
        }

        #endregion Constructors
    }
}
