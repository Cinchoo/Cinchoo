namespace Cinchoo.Core.ComponentModel
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;
    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    public abstract class ChoTypeDescriptionProvider : TypeDescriptionProvider
    {
        #region Instance Data Members (Private)

        internal PropertyDescriptorCollection _propertyCache;
        internal FilterCache<PropertyDescriptorCollection> _filterPropertyCache = new FilterCache<PropertyDescriptorCollection>();

        #endregion Instance Data Members (Private)

        #region TypeDescriptionProvider Overrrides

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);

            ChoTypeProviderAttribute typeProviderAttribute = ChoType.GetAttribute<ChoTypeProviderAttribute>(GetType());
            if (typeProviderAttribute != null)
            {
                if (ChoGuard.IsArgumentNotNullOrEmpty(typeProviderAttribute.AdditionalConstructorParameters))
                    return ChoType.CreateInstance(typeProviderAttribute.Type, 
                        ChoArray.Combine<object>(new object[] { this, defaultDescriptor, objectType, instance }, typeProviderAttribute.AdditionalConstructorParameters))
                        as ICustomTypeDescriptor;
                else
                    return ChoType.CreateInstance(typeProviderAttribute.Type, 
                        new object[] { this, defaultDescriptor, objectType, instance }) as ICustomTypeDescriptor;
                
                //return instance == null ? defaultDescriptor : new FieldsToPropertiesTypeDescriptor(this, defaultDescriptor, objectType);
            }
            return defaultDescriptor;
        }

        #endregion TypeDescriptionProvider Overrrides
    }
}
