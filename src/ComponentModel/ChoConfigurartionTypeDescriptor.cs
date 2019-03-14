namespace Cinchoo.Core.ComponentModel
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion NameSpaces

    public class ChoConfigurartionTypeDescriptor : ChoCustomTypeDescriptor
    {
        public ChoConfigurartionTypeDescriptor(ChoTypeDescriptionProvider typeDescriptionProvider, ICustomTypeDescriptor customTypeDescriptor, Type objectType,
            object instance) : base(typeDescriptionProvider, customTypeDescriptor, objectType, instance)
        {
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Retrieve cached properties and filtered properties
            bool filtering = attributes != null && attributes.Length > 0;

            FilterCache<PropertyDescriptorCollection> propertyCache = TypeDescriptionProvider._filterPropertyCache;
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptionProvider._propertyCache;

            // Use a cached version if we can
            if (filtering && propertyCache != null && propertyCache.IsValid(attributes)) return propertyCache.FilteredMembers;
            else if (!filtering && propertyDescriptorCollection != null) return propertyDescriptorCollection;

            // Otherwise, create the property collection
            propertyDescriptorCollection = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor prop in base.GetProperties(attributes))
            {
                propertyDescriptorCollection.Add(prop);
            }
            foreach (FieldInfo field in ObjectType.GetFields())
            {
                ChoConfigurationFieldPropertyDescriptor fieldDesc = new ChoConfigurationFieldPropertyDescriptor(field);
                if (!filtering || fieldDesc.Attributes.Contains(attributes)) propertyDescriptorCollection.Add(fieldDesc);
            }

            // Store the updated properties
            if (filtering)
            {
                propertyCache = new FilterCache<PropertyDescriptorCollection>();
                propertyCache.FilteredMembers = propertyDescriptorCollection;
                propertyCache.Attributes = attributes;
                TypeDescriptionProvider._filterPropertyCache = propertyCache;
            }
            else 
                TypeDescriptionProvider._propertyCache = propertyDescriptorCollection;

            // Return the computed properties
            return propertyDescriptorCollection;
        }
    }
}
