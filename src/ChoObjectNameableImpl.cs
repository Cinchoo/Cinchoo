namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    public class ChoObjectNameableImpl : IChoObjectNameable
    {
        #region Instance Data Members (Private)

        private string _name;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoObjectNameableImpl(string name)
        {
            Name = name;
        }

        #endregion Constructors

        #region IChoObjectNameable Members

        public string Name
        {
            get { return _name; }
            set
            {
                ChoGuard.ArgumentNotNull(value, "Name");
                _name = value;
            }
        }

        #endregion
    }
}
