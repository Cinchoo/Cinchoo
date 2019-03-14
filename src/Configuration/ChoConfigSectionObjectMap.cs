//namespace eSquare.Core.Configuration
//{
//    #region NameSpaces

//    using System;
//    using System.IO;
//    using System.Text;
//    using System.Reflection;
//    using System.Collections;
//    using System.Configuration;
//    using System.Collections.Generic;
//    using System.Collections.Specialized;

//    using eSquare.Core.Properties;
//    using eSquare.Core.Exceptions;
//    using eSquare.Core.Attributes;
//    using eSquare.Core.Converters;
//    using eSquare.Core.Diagnostics;
//    using eSquare.Core.Collections.Specialized;
//    using eSquare.Core.Configuration.Sections;

//    #endregion NameSpaces

//    public class ChoConfigSectionObjectMap
//    {
//        #region Instance Data Members

//        private object _configObject;
//        private IChoConfigSectionable _configSection;

//        #endregion

//        #region Constructors

//        internal ChoConfigSectionObjectMap()
//        {
//        }

//        internal ChoConfigSectionObjectMap(IChoConfigSectionable configSection, object configObject)
//        {
//            if (configSection == null)
//                throw new NullReferenceException("configSection");
//            if (configObject == null)
//                throw new NullReferenceException("configObject");

//            _configSection = configSection;
//            _configObject = configObject;
//        }

//        #endregion

//        #region Instance Properties

//        public IChoConfigSectionable ConfigSection
//        {
//            get { return _configSection; }
//            internal set
//            {
//                if (value == null)
//                    throw new NullReferenceException("configSection");

//                _configSection = value;
//            }
//        }

//        public object ConfigObject
//        {
//            get { return _configObject; }
//            internal set
//            {
//                if (value == null)
//                    throw new NullReferenceException("configObject");

//                _configObject = value;
//                ChoObjectManagementFactory.SetInstance(value);
//            }
//        }

//        #endregion Instance Properties

//        #region Instance Members (Public)

//        public Hashtable ToHashtable()
//        {
//            Hashtable keyValues = new Hashtable();

//            Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(_configObject.GetType(), typeof(ChoMemberInfoAttribute));
//            if (_configObject is ChoConfigurableObject)
//            {
//                MemberInfo[] memberInfos = ChoType.GetMembers(_configObject.GetType(), typeof(ChoMemberInfoAttribute));
//                if (memberInfos == null || memberInfos.Length == 0) return keyValues;

//                ChoMemberInfoAttribute memberInfoAttribute = null;
//                foreach (MemberInfo memberInfo in memberInfos)
//                {
//                    if (configMemberInfos.ContainsKey(memberInfo.Name) && !((ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute))).Persistable)
//                        continue;

//                    memberInfoAttribute = (ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute));
//                    if (memberInfoAttribute == null) continue;

//                    object memberValue = ChoType.GetMemberValue(_configObject, memberInfo.Name);

//                    keyValues.Add(memberInfoAttribute.Name, ChoConvert.ChangeType(_configObject, memberValue, ChoType.GetMemberType(_configObject.GetType(), memberInfo.Name),
//                        ChoType.GetTypeConverters(ChoType.GetMember(_configObject.GetType(), memberInfo.Name), typeof(ChoPersistTypeConverterAttribute))));
//                }
//            }
//            return keyValues;
//        }


//        public Hashtable ToPersistableHashtable()
//        {
//            Hashtable keyValues = new Hashtable();

//            Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(_configObject.GetType(), typeof(ChoMemberInfoAttribute));
//            if (_configObject is ChoConfigurableObject)
//            {
//                MemberInfo[] memberInfos = ChoType.GetMembers(_configObject.GetType(), typeof(ChoMemberInfoAttribute));
//                if (memberInfos == null || memberInfos.Length == 0) return keyValues;

//                ChoMemberInfoAttribute memberInfoAttribute = null;
//                foreach (MemberInfo memberInfo in memberInfos)
//                {
//                    if (configMemberInfos.ContainsKey(memberInfo.Name) && !((ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute))).Persistable)
//                        continue;

//                    memberInfoAttribute = (ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute));
//                    if (memberInfoAttribute == null || !memberInfoAttribute.Persistable) continue;

//                    object memberValue = ChoType.GetMemberValue(_configObject, memberInfo.Name);

//                    keyValues.Add(memberInfoAttribute.Name, ChoConvert.ChangeType(_configObject, memberValue, ChoType.GetMemberType(_configObject.GetType(), memberInfo.Name),
//                        ChoType.GetTypeConverters(ChoType.GetMember(_configObject.GetType(), memberInfo.Name), typeof(ChoPersistTypeConverterAttribute))));
//                }
//            }
//            return keyValues;
//        }

//        //public object GetMemberValue(ChoMemberInfoAttribute memberInfoAttribute, MemberInfo memberInfo)
//        //{
//        //    //TO BE CHECKED
//        //    //if (_configSection != null && _configSection[memberInfo.Name] != null)
//        //    //    return _configSection[memberInfo.Name];

//        //    return ChoObject.GetObjectMemberValue(_configObject, memberInfo);

//        //    ////Get default values
//        //    //else if (memberInfoAttribute.DefaultValue != null)
//        //    //    return memberInfoAttribute.DefaultValue;
//        //    //else if (memberInfo.MemberType == MemberTypes.Field && ChoType.GetMemberValue(_configObject, memberInfo.Name) != null)
//        //    //    return ChoType.GetMemberValue(_configObject, memberInfo.Name);
//        //    //else
//        //    //    return null;
//        //}

//        //public object GetConvertedMemberValue(ChoMemberInfoAttribute memberInfoAttribute, MemberInfo memberInfo)
//        //{
//        //    return ChoObject.GetObjectConvertedMemberValue(_configObject, memberInfo);
//        //    //object memberValue = GetMemberValue(memberInfoAttribute, memberInfo);
//        //    //return ChoConvert.ChangeType(_configObject, memberValue, ChoType.GetMemberType(memberValue.GetType(), memberInfo.Name),
//        //    //    ChoType.GetTypeConverters(ChoType.GetMember(_configObject.GetType(), memberInfo.Name), typeof(ChoMemberInfoAttribute)));
//        //}

//        #endregion Instance Members (Public)
//    }
//}
