namespace Cinchoo.Core.Collections.Generic
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Diagnostics;

    #endregion NameSpaces


    [Serializable]
    [ComVisible(false)]
    [DebuggerDisplay("Count = {Count}")]
    public class ChoNestedList<T> : List<T>, IList<T>, ICloneable
    {
        #region Instance Data Members (Private)

        private object _syncRoot = new object();
        private List<T> 
        #endregion Instance Data Members (Private)

        #region Constructors

        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that is empty and has the default initial capacity.
        public ChoNestedList()
            : base()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that contains elements copied from the specified collection and has sufficient
        //     capacity to accommodate the number of elements copied.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements are copied to the new list.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        public ChoNestedList(IEnumerable<T> collection)
            : base(collection)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.List<T> class
        //     that is empty and has the specified initial capacity.
        //
        // Parameters:
        //   capacity:
        //     The number of elements that the new list can initially store.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     capacity is less than 0.
        public ChoNestedList(int capacity)
            : base(capacity)
        {
        }

        #endregion Constructors

        #region ICloneable Members

        public object Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
