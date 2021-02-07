using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    // Because this class is derived from MarshalByRefObject, a proxy
    // to a MarshalByRefType object can be returned across an AppDomain
    // boundary.
    public class ChoMarshalByRefType : MarshalByRefObject
    {
        // Call this method via a proxy.
        public void Do(Action action)
        {
            action();
        }
    }
}