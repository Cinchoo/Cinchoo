namespace Cinchoo.Core.ComponentModel
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.ComponentModel;

    #endregion NameSpaces

    public abstract class ChoPropertyDescriptor : PropertyDescriptor
    {
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyDescriptor
        //     class with the name and attributes in the specified System.ComponentModel.MemberDescriptor.
        //
        // Parameters:
        //   descr:
        //     A System.ComponentModel.MemberDescriptor that contains the name of the property
        //     and its attributes.
        protected ChoPropertyDescriptor(MemberDescriptor descr) : base(descr)
        {
        }
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyDescriptor
        //     class with the name in the specified System.ComponentModel.MemberDescriptor
        //     and the attributes in both the System.ComponentModel.MemberDescriptor and
        //     the System.Attribute array.
        //
        // Parameters:
        //   descr:
        //     A System.ComponentModel.MemberDescriptor containing the name of the member
        //     and its attributes.
        //
        //   attrs:
        //     An System.Attribute array containing the attributes you want to associate
        //     with the property.
        protected ChoPropertyDescriptor(MemberDescriptor descr, Attribute[] attrs)
            : base(descr, attrs)
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.PropertyDescriptor
        //     class with the specified name and attributes.
        //
        // Parameters:
        //   name:
        //     The name of the property.
        //
        //   attrs:
        //     An array of type System.Attribute that contains the property attributes.
        protected ChoPropertyDescriptor(string name, Attribute[] attrs)
            : base(name, attrs)
        {
        }

    }
}
