using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;

namespace PropertDescSample
{
    class Program
    {
        public class ISBNSite : ISite
        {
            private IComponent m_curComponent;
            private IContainer m_curContainer;
            private bool m_bDesignMode;
            private string m_ISBNCmpName;

            public ISBNSite()
            {
            }

            public ISBNSite(IContainer actvCntr, IComponent prntCmpnt)
            {
                m_curComponent = prntCmpnt;
                m_curContainer = actvCntr;
                m_bDesignMode = false;
                m_ISBNCmpName = null;
            }

            //Support the ISite interface.
            public virtual IComponent Component
            {
                get
                {
                    return m_curComponent;
                }
            }

            public virtual IContainer Container
            {
                get
                {
                    return m_curContainer;
                }
            }

            public virtual bool DesignMode
            {
                get
                {
                    return m_bDesignMode;
                }
            }

            public virtual string Name
            {
                get
                {
                    return m_ISBNCmpName;
                }

                set
                {
                    m_ISBNCmpName = value;
                }
            }

            public static IDictionaryService dictionaryService = new DictionaryService();

            //Support the IServiceProvider interface.
            public virtual object GetService(Type serviceType)
            {
                //This example does not use any service object.
                if (serviceType == typeof(IDictionaryService))
                    return dictionaryService;
                else
                    return null;
            }

        }


        public class DictionaryService : IDictionaryService
        {
            Dictionary<object, object> _dictionary = new Dictionary<object, object>();

            #region IDictionaryService Members

            public object GetKey(object value)
            {
                foreach (object key in _dictionary.Keys)
                {
                    if (_dictionary[key] == value) return key;
                }

                return null;
            }

            public object GetValue(object key)
            {
                Console.WriteLine(key.GetType().FullName);

                if (_dictionary.ContainsKey(key))
                    return _dictionary[key];
                else
                    return null;
            }

            public void SetValue(object key, object value)
            {
                if (_dictionary.ContainsKey(key))
                    _dictionary[key] = value;
                else
                    _dictionary.Add(key, value);
            }

            #endregion
        }

        public static ISite site = new ISBNSite();
        public class A1 : IComponent
        {
            public string Draw { get; set; }
            public string Draw1 { get; set; }
            public string Draw2 { get; set; }
            public string Draw3 { get; set; }

            #region IComponent Members

            public event EventHandler Disposed;

            public ISite Site
            {
                get
                {
                    return new ISBNSite();
                }
                set
                {
                    site = value;
                    //// Publish ourselves as new service.
                    //IDictionaryService container = (IDictionaryService)
                    //    GetService(typeof(IDictionaryService));
                    //container.AddService(typeof(A1), this);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion
        }

        public class BaseA
        {
            public BaseA()
            {
                Console.WriteLine(this.GetType().FullName);
            }
        }

        public class A : BaseA
        {
        }

        public class B : BaseA
        {
        }

        static void Main(string[] args)
        {
            A1 A = new A1();

            Console.WriteLine("Properties of an arbitrary object: A:");
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(A);
            foreach (PropertyDescriptor property in properties) Console.WriteLine("- " + property.Name);

            Console.WriteLine("\nAdd our useless provider System.Object and chain it with the original one...");
            TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(A1))), typeof(A1));

            Console.WriteLine("\nProperties of an arbitrary object: Console.Out:");
            properties = TypeDescriptor.GetProperties(A);
            foreach (PropertyDescriptor property in properties) Console.WriteLine("- " + property.Name);
            properties = TypeDescriptor.GetProperties(A);
            foreach (PropertyDescriptor property in properties) Console.WriteLine("- " + property.Name);

            foreach (PropertyInfo property in TypeDescriptor.GetReflectionType(A).GetProperties()) Console.WriteLine("- " + property.Name);
        }
    }

    /// <summary>
    /// This is our custom provider. It simply provides a custom type descriptor
    /// and delegates all its other tasks to its parent 
    /// </summary>
    internal sealed class UselessTypeDescriptionProvider : TypeDescriptionProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal UselessTypeDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Create and return our custom type descriptor and chain it with the original 
        /// custom type descriptor
        /// </summary>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new UselessCustomTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
        }

        public override System.Collections.IDictionary GetCache(object instance)
        {
            return base.GetCache(instance);
        }
    }

    /// <summary>
    /// This is our custom type descriptor. It creates a new property and returns it along
    /// with the original list
    /// </summary>
    internal sealed class UselessCustomTypeDescriptor : CustomTypeDescriptor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal UselessCustomTypeDescriptor(ICustomTypeDescriptor parent)
            : base(parent)
        {
        }

        /// <summary>
        /// This method add a new property to the original collection
        /// </summary>
        public override PropertyDescriptorCollection GetProperties()
        {
            Console.WriteLine("*** Invoked ****");

            // Enumerate the original set of properties and create our new set with it
            PropertyDescriptorCollection originalProperties = base.GetProperties();
            List<PropertyDescriptor> newProperties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in originalProperties) newProperties.Add(pd);

            // Create a new property and add it to the collection
            PropertyDescriptor newProperty = TypeDescriptor.CreateProperty(typeof(object), "UselessProperty", typeof(string));
            newProperties.Add(newProperty);

            // Finally return the list
            return new PropertyDescriptorCollection(newProperties.ToArray(), true);
        }
    }
}