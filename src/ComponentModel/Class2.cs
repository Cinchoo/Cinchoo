using System;
using System.Text;
using System.Reflection;
using System.ComponentModel;

public class FieldsToPropertiesTypeDescriptionProvider : TypeDescriptionProvider
{
    private TypeDescriptionProvider _baseProvider;
    private PropertyDescriptorCollection _propCache;
    private FilterCache _filterCache;

    public FieldsToPropertiesTypeDescriptionProvider(Type t)
    {
        _baseProvider = TypeDescriptor.GetProvider(t);
    }

    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
    {
        return new FieldsToPropertiesTypeDescriptor(
            this, _baseProvider.GetTypeDescriptor(objectType, instance), objectType);
    }

    private class FilterCache
    {
        public Attribute[] Attributes;
        public PropertyDescriptorCollection FilteredProperties;
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

    public class FieldPropertyDescriptor : PropertyDescriptor
    {
        private FieldInfo _field;

        public FieldPropertyDescriptor(FieldInfo field)
            : base(field.Name,
                (Attribute[])field.GetCustomAttributes(typeof(Attribute), true))
        {
            _field = field;
        }

        public FieldInfo Field { get { return _field; } }

        public override bool Equals(object obj)
        {
            FieldPropertyDescriptor other = obj as FieldPropertyDescriptor;
            return other != null && other._field.Equals(_field);
        }

        public override int GetHashCode() { return _field.GetHashCode(); }

        public override bool IsReadOnly { get { return false; } }
        public override void ResetValue(object component) { }
        public override bool CanResetValue(object component) { return false; }
        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return _field.DeclaringType; }
        }
        public override Type PropertyType { get { return _field.FieldType; } }

        public override object GetValue(object component)
        {
            return _field.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _field.SetValue(component, value);
            OnValueChanged(component, EventArgs.Empty);
        }
    }

    private class FieldsToPropertiesTypeDescriptor : CustomTypeDescriptor
    {
        private Type _objectType;
        private FieldsToPropertiesTypeDescriptionProvider _provider;

        public FieldsToPropertiesTypeDescriptor(FieldsToPropertiesTypeDescriptionProvider provider, ICustomTypeDescriptor descriptor, Type objectType)
            : base(descriptor)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (descriptor == null) throw new ArgumentNullException("descriptor");
            if (objectType == null) throw new ArgumentNullException("objectType");
            _objectType = objectType;
            _provider = provider;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            // Retrieve cached properties and filtered properties
            bool filtering = attributes != null && attributes.Length > 0;
            FilterCache cache = _provider._filterCache;
            PropertyDescriptorCollection props = _provider._propCache;

            // Use a cached version if we can
            if (filtering && cache != null && cache.IsValid(attributes)) return cache.FilteredProperties;
            else if (!filtering && props != null) return props;

            // Otherwise, create the property collection
            props = new PropertyDescriptorCollection(null);
            foreach (PropertyDescriptor prop in base.GetProperties(attributes))
            {
                props.Add(prop);
            }
            foreach (FieldInfo field in _objectType.GetFields())
            {
                FieldPropertyDescriptor fieldDesc = new FieldPropertyDescriptor(field);
                if (!filtering || fieldDesc.Attributes.Contains(attributes)) props.Add(fieldDesc);
            }

            // Store the updated properties
            if (filtering)
            {
                cache = new FilterCache();
                cache.FilteredProperties = props;
                cache.Attributes = attributes;
                _provider._filterCache = cache;
            }
            else _provider._propCache = props;

            // Return the computed properties
            return props;
        }
    }
}