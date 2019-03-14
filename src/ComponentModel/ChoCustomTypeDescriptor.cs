namespace Cinchoo.Core.ComponentModel
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;

    #endregion NameSpaces

    internal class FilterCache<T>
    {
        public Attribute[] Attributes;
        public T FilteredMembers;
        public bool IsValid(Attribute[] other)
        {
            if (other == null || Attributes == null) return false;
            if (Attributes.Length != other.Length) return false;
            for (int i = 0; i < other.Length; i++)
            {
                if (!Attributes[i].Match(other[i])) return false;
            }
            return true;
        }
    }

    public abstract class ChoCustomTypeDescriptor : CustomTypeDescriptor
    {
        #region Instance Data Members (Protected)

        protected ChoTypeDescriptionProvider TypeDescriptionProvider;
        protected Type ObjectType;
        protected object Instance;

        #endregion Instance Data Members (Protected)

        public ChoCustomTypeDescriptor(ChoTypeDescriptionProvider typeDescriptionProvider, ICustomTypeDescriptor customTypeDescriptor, Type objectType,
            object instance)
            : base(customTypeDescriptor)
        {
            ChoGuard.ArgumentNotNull(typeDescriptionProvider, "typeDescriptionProvider");
            ChoGuard.ArgumentNotNull(customTypeDescriptor, "customTypeDescriptor");
            ChoGuard.ArgumentNotNull(objectType, "objectType");

            TypeDescriptionProvider = typeDescriptionProvider;
            ObjectType = objectType;
            Instance = instance;
        }
    }
}
