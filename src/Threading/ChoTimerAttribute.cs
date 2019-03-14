namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;
    
    #endregion NameSpaces

    public class ChoTimerAttribute : ChoObjectNameableAttribute
    {
        #region Instance Members (Public)

        private int _dueTime;
        public int DueTime
        {
            get { return _dueTime; }
            set 
            {
                if (value < 0) throw new ArgumentException(String.Format("Invalid {0} duetime value passed.", value));
                _dueTime = value; 
            }
        }

        //private int _dueTime;
        //public int DueTime
        //{
        //    get { return _dueTime; }
        //    set
        //    {
        //        if (value < 0) throw new ArgumentException(String.Format("Invalid {0} duetime value passed.", value));
        //        _dueTime = value;
        //    }
        //}

        #endregion Instance Members (Public)
    }
}
