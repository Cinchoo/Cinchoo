using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core;
using System.Reflection;

namespace System.Data
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ChoDataObjectAttribute : Attribute
    {
        public bool Silent { get; set; }
    }

    public static class ChoDataReaderEx
    {
        public static bool FieldExists(this IDataReader reader, string fieldName)
        {
            reader.GetSchemaTable().DefaultView.RowFilter = string.Format("ColumnName= '{0}'", fieldName);
            return (reader.GetSchemaTable().DefaultView.Count > 0);
        }

        public static IEnumerable<T> AsEnumerable<T>(this IDataReader dr)
        {
            while (dr.Read())
            {
                T target = Activator.CreateInstance<T>();
                ChoDataObjectAttribute objAttr = typeof(T).GetCustomAttribute<ChoDataObjectAttribute>();

                ExtractAndPopulateValues(target, dr, objAttr);
                yield return target;
            }
        }

        private static void ExtractAndPopulateValues<T>(T target, IDataReader dr, ChoDataObjectAttribute objAttr)
        {
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(target.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                string name;
                string fallbackValue = null;
                object memberValue = null;
                object newValue = null;
                object defaultValue = null;
                object value = null;

                ChoPropertyInfoAttribute memberInfoAttribute = null;

                foreach (MemberInfo memberInfo in memberInfos)
                {
                    name = ChoType.GetMemberName(memberInfo);
                    memberValue = null;
                    newValue = null;
                    value = null;

                    memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttributeByBaseType(memberInfo, typeof(ChoPropertyInfoAttribute));
                    
                    try
                    {
                        if (dr.FieldExists(name))
                            value = dr[name];
                        try
                        {
                            defaultValue = memberInfo.GetDefaultValue();
                            //if (memberInfoAttribute != null)
                            //{
                            //    if (!memberInfoAttribute.DefaultValue.IsNullOrWhiteSpace())
                            //        defaultValue = ChoString.ExpandPropertiesEx(memberInfoAttribute.DefaultValue);
                            //}
                        }
                        catch
                        {
                            defaultValue = null;
                        }

                        try
                        {
                            if (memberInfoAttribute != null)
                            {
                                if (!memberInfoAttribute.FallbackValue.IsNullOrWhiteSpace())
                                    fallbackValue = ChoString.ExpandPropertiesEx(memberInfoAttribute.FallbackValue);
                            }
                        }
                        catch
                        {
                            fallbackValue = null;
                        }

                        newValue = memberValue = value;

                        if (newValue.IsNullOrDbNull())
                        {
                            if (fallbackValue == null)
                                newValue = defaultValue;
                            else
                                newValue = fallbackValue;
                        }

                        newValue = ChoConvert.ConvertFrom(newValue, memberInfo, target);
    //                    newValue = ChoConvert.ConvertFrom(target, newValue, ChoType.GetMemberType(memberInfo),
    //ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));

                        if (newValue == null)
                        {
                        }
                        else
                            ChoType.SetMemberValue(target, memberInfo, newValue);
                    }
                    catch (Exception)
                    {
                        if (objAttr == null || (objAttr != null && !objAttr.Silent))
                            throw;
                    }
                }
            }
        }
    }
}