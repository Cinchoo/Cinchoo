using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChoSerializerAttribute : Attribute
    {
        public readonly Type ObjectSerializerType;

        public ChoSerializerAttribute(string objectSerializerTypeName)
        {
            try
            {
                ObjectSerializerType = ChoType.GetType(objectSerializerTypeName);
                if (!typeof(IChoObjectSerializer).IsAssignableFrom(ObjectSerializerType))
                    ObjectSerializerType = null;
            }
            catch (Exception ex)
            {
                ChoTrace.Debug("Failed to find '{0}' object serializer. {1}".FormatString(objectSerializerTypeName, ex.Message));
            }
        }

        public ChoSerializerAttribute(Type objectSerializerType)
        {
            ObjectSerializerType = objectSerializerType;
        }
    }
}
