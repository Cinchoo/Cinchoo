using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Factory
{
    [ChoObject("Child")]
    public class ChildObject
    {
        int x = 10;

        public ChildObject()
        {
        }
    }

    [ChoObject("Sample", DefaultConstructor="Const2")]
    public class SampleObject
    {
        int _x;
        double _y;
        string _z;

        [ChoObjectField(ReferenceId = "Child")]
        ChildObject _childObject;

        [ChoObjectConstructor("5, 10", Id="Const1")]
        public SampleObject(int x, double y)
        {
            _x = x;
            _y = y;
        }

        [ChoObjectConstructor("5, asdsd", Id = "Const2")]
        public SampleObject(int x, string z)
        {
            _x = x;
            _z = z;
        }

        [ChoObjectProperty(Value=100.021)]
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
    }
}
