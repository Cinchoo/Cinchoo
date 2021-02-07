namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.IO;
    using Cinchoo.Core.Xml.Serialization;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Cinchoo.Core.Diagnostics;
    using System.Runtime.Serialization.Json;

    #endregion NameSpaces

    [Serializable]
    public class ChoObject : ChoFormattableObject, ICloneable, INotifyPropertyChanged //, IXmlSerializable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly XmlWriterSettings _xws = new XmlWriterSettings() { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Auto, Indent = true };

        #region Shared Members (Public)

        [ChoHiddenMember]
        public static object Clone(object srcObject)
        {
            return ChoObjectEx.Clone<object>(srcObject);
        }

        #endregion

        #region ICloneable Members

        [ChoHiddenMember]
        public virtual object Clone()
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(buffer, this);
                buffer.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(buffer);
            }
        }

        #endregion

        #region Constructors

        [ChoHiddenMember]
        protected internal ChoObject(bool dontRegisterMe)
            : base()
        {
        }

        public ChoObject()
            : base()
        {
        }

        #endregion

        #region Commented
        /*
		#region IXmlSerializable Members

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			try
			{
				if (_value != null)
				{
					writer.WriteAttributeString("type", _value.GetType().AssemblyQualifiedName);
					writer.WriteAttributeString("isLite", _isLite.ToString());
					if (_isLite)
					{
						LiteBinaryFormatter formatter = new LiteBinaryFormatter();
						ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(typeof(byte[]));
						xmlSerializer.Serialize(writer, formatter.ToByteArray(_value));
					}
					else
					{
						#region Standard Xml Serialization

						if (_value is Array)
						{
							foreach (object arrayItem in _value as Array)
							{
								writer.WriteStartElement("Item");
								if (arrayItem == null)
									writer.WriteAttributeString("type", "[NULL]");
								else
								{
									writer.WriteAttributeString("type", arrayItem.GetType().AssemblyQualifiedName);
									ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(arrayItem.GetType());
									xmlSerializer.Serialize(writer, arrayItem);
								}
								writer.WriteEndElement();
							}
						}
						else
						{
							ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(_value.GetType());
							xmlSerializer.Serialize(writer, _value);
						}

						#endregion Standard Xml Serialization
					}
				}
			}
			catch (Exception ex)
			{
				ChoStreamProfile.WriteLine(ChoLogDirectories.Others,
					ChoLogFiles.SerializationIssues, ChoApplicationException.ToString(ex));
				throw;
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			// TODO:  Add ChoObject.GetSchema implementation
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			try
			{
				string objectType = reader.GetAttribute("type");
				if (objectType == null) { reader.Skip(); return; }

				Type type = ChoType.GetType(objectType);

				if (type == null) throw new ApplicationException(String.Format("Can't find {0} class.", objectType));

				bool isLite = Convert.ToBoolean(reader.GetAttribute("isLite"));

				if (isLite && !LiteSerializationSettings.Me.TurnOn)
					throw new ApplicationException("Failed to deserialize message. LiteSerizalization is not turned on.");


				if (isLite)
				{
					LiteBinaryFormatter formatter = new LiteBinaryFormatter();
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(byte[]));
					reader.Read();
					_value = formatter.FromByteArray(xmlSerializer.Deserialize(reader) as byte[]) as object;
				}
				else
				{
					#region Standard Xml Serialization

					if (type.IsArray)
					{
						int index = 0;
						reader.Read();
						ArrayList elements = new ArrayList();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if (reader.IsStartElement("Item"))
							{
								objectType = reader.GetAttribute("type");

								reader.ReadStartElement("Item");

								if (objectType == "[NULL]")
									elements.Add(null);
								else
								{
									Type elementType = ChoType.GetType(objectType);
									if (elementType == null) throw new ApplicationException(String.Format("Can't find {0} class.", objectType));

									try
									{
										index++;
										XmlSerializer xmlSerializer = new XmlSerializer(elementType);
										elements.Add(xmlSerializer.Deserialize(reader));
									}
									catch (Exception ex)
									{
										throw new XmlException(String.Format("Failed to deserialize {0} array item.", index), ex);
									}

									reader.ReadEndElement();
								}
							}
						}
						_value = elements.ToArray();
					}
					else
					{
						ChoNullNSXmlSerializer xmlSerializer = new ChoNullNSXmlSerializer(type);
						reader.Read();
						_value = xmlSerializer.Deserialize(reader);
					}

					#endregion Standard Xml Serialization
				}
			}
			catch (Exception ex)
			{
				ChoStreamProfile.WriteLine(ChoLogDirectories.Others,
					ChoLogFiles.SerializationIssues, ChoApplicationException.ToString(ex));
				throw;
			}
		}

		#endregion
		*/
        #endregion Commented

        #region Object Overrides

        [ChoHiddenMember]
        public override string ToString()
        {
            return ToString(this);
        }

        #endregion Object Overrides

        #region Shared Members (Public)

        ////TODO: Do recursive lookup
        //[ChoHiddenMember]
        //public static Hashtable ToHashtable(object target)
        //{
        //    ChoGuard.ArgumentNotNull(target, "Target");

        //    Hashtable keyValues = new Hashtable();

        //    Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    if (memberInfos == null || memberInfos.Length == 0) return keyValues;

        //    ChoConfigurationPropertyAttribute memberInfoAttribute = null;
        //    foreach (MemberInfo memberInfo in memberInfos)
        //    {
        //        if (ChoType.IsReadOnlyMember(memberInfo))
        //            continue;

        //        if (configMemberInfos.ContainsKey(memberInfo.Name) && !((ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute))).Persistable)
        //            continue;

        //        memberInfoAttribute = (ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute));
        //        if (memberInfoAttribute == null) continue;

        //        object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);

        //        keyValues.Add(memberInfoAttribute.Name, ChoConvert.ConvertFrom(target, memberValue, ChoType.GetMemberType(target.GetType(), memberInfo.Name),
        //            ChoType.GetTypeConverters(memberInfo, typeof(ChoTypeConverterAttribute))));
        //    }
        //    return keyValues;
        //}

        //TODO: Do recursive lookup
        //[ChoHiddenMember]
        //public static Dictionary<string, object> ToDictionary(object target)
        //{
        //    ChoGuard.ArgumentNotNull(target, "Target");

        //    Dictionary<string, object> keyValues = new Dictionary<string, object>();

        //    Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    if (memberInfos == null || memberInfos.Length == 0) return keyValues;

        //    ChoConfigurationPropertyAttribute memberInfoAttribute = null;
        //    foreach (MemberInfo memberInfo in memberInfos)
        //    {
        //        if (ChoType.IsReadOnlyMember(memberInfo))
        //            continue;

        //        if (configMemberInfos.ContainsKey(memberInfo.Name) && !((ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute))).Persistable)
        //            continue;

        //        memberInfoAttribute = (ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute));
        //        if (memberInfoAttribute == null) continue;

        //        object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);

        //        keyValues.Add(memberInfoAttribute.Name, ChoConvert.ConvertFrom(target, memberValue, ChoType.GetMemberType(target.GetType(), memberInfo.Name),
        //            ChoType.GetTypeConverters(memberInfo, typeof(ChoTypeConverterAttribute))));
        //    }
        //    return keyValues;
        //}

        //TODO: Do recursive lookup
        //[ChoHiddenMember]
        //public static ChoNameValueCollection ToNameValueCollection(object target)
        //{
        //    ChoGuard.ArgumentNotNull(target, "Target");

        //    ChoNameValueCollection nameValues = new ChoNameValueCollection();

        //    Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    if (memberInfos == null || memberInfos.Length == 0) return nameValues;

        //    ChoConfigurationPropertyAttribute memberInfoAttribute = null;
        //    foreach (MemberInfo memberInfo in memberInfos)
        //    {
        //        if (ChoType.IsReadOnlyMember(memberInfo))
        //            continue;

        //        if (configMemberInfos.ContainsKey(memberInfo.Name)) // && !((ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute))).Persistable)
        //            continue;

        //        memberInfoAttribute = (ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute));
        //        if (memberInfoAttribute == null) continue;

        //        object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);

        //        nameValues.Add(memberInfoAttribute.Name, ChoString.ToString(ChoConvert.ConvertFrom(target, memberValue, ChoType.GetMemberType(target.GetType(), memberInfo.Name),
        //            ChoType.GetTypeConverters(memberInfo, typeof(ChoTypeConverterAttribute)))));
        //    }
        //    return nameValues;
        //}

        #region Save Overloads

        [ChoHiddenMember]
        public static string Save(object target, string filePath = null)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            string tmpPath = filePath.IsNullOrWhiteSpace() ? ChoPath.GetTempFileName() : filePath;

            Serialize(tmpPath, target);

            return tmpPath;
        }

        [ChoHiddenMember]
        public static string SaveAsXml(object target, string filePath = null)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            string tmpPath = filePath.IsNullOrWhiteSpace() ? ChoPath.GetTempFileName() : filePath;

            XmlSerialize(tmpPath, target);

            return tmpPath;
        }

        #endregion Save Overloads

        #region XmlSerialize Overloads

        [ChoHiddenMember]
        public static void XmlSerialize(Stream sr, object target, XmlWriterSettings xws = null)
        {
            ChoGuard.ArgumentNotNull(sr, "Stream");
            ChoGuard.ArgumentNotNull(target, "Target");

            using (XmlWriter xtw = XmlTextWriter.Create(sr, xws ?? _xws))
            {
                ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(target.GetType());
                serializer.Serialize(xtw, target);

                xtw.Flush();
            }
        }

        [ChoHiddenMember]
        public static string XmlSerialize(object target, XmlWriterSettings xws = null)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            
            StringBuilder xmlString = new StringBuilder();
            using (XmlWriter xtw = XmlTextWriter.Create(xmlString, xws ?? _xws))
            {
                ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(target.GetType());
                serializer.Serialize(xtw, target);

                xtw.Flush();

                return xmlString.ToString();
            }
        }

        [ChoHiddenMember]
        public static void XmlSerialize(string path, object target, XmlWriterSettings xws = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoDirectory.CreateDirectoryFromFilePath(Path.GetDirectoryName(ChoPath.GetFullPath(path)));

            File.WriteAllText(path, XmlSerialize(target, xws));
        }

        #endregion XmlSerialize Overloads

        #region XmlDeserialize Overloads

        [ChoHiddenMember]
        public static T XmlDeserialize<T>(Stream sr, XmlReaderSettings xrs = null)
        {
            return (T)XmlDeserialize(sr, typeof(T), xrs);
        }

        [ChoHiddenMember]
        public static object XmlDeserialize(Stream sr, Type type, XmlReaderSettings xrs = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(sr, "Stream");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            using (XmlReader xtw = XmlTextReader.Create(sr, xrs ?? new XmlReaderSettings()))
            {
                ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(type);
                return serializer.Deserialize(xtw);
            }
        }

        [ChoHiddenMember]
        public static T XmlDeserialize<T>(string xmlString, XmlReaderSettings xrs = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(xmlString, "xmlString");

            return (T)XmlDeserialize(xmlString, typeof(T), xrs);
        }

        [ChoHiddenMember]
        public static object XmlDeserialize(string xmlString, Type type, XmlReaderSettings xrs = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(xmlString, "XmlString");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            using (StringReader sr = new StringReader(xmlString))
            {
                using (XmlReader xtw = XmlTextReader.Create(sr, xrs ?? new XmlReaderSettings()))
                {
                    ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(type);
                    return serializer.Deserialize(xtw);
                }
            }
        }

        [ChoHiddenMember]
        public static T XmlDeserializeFromFile<T>(string path, XmlReaderSettings xrs = null)
        {
            return (T)XmlDeserializeFromFile(path, typeof(T), xrs);
        }

        [ChoHiddenMember]
        public static object XmlDeserializeFromFile(string path, Type type, XmlReaderSettings xrs = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            using (StreamReader sr = new StreamReader(path))
            {
                using (XmlReader xtw = XmlTextReader.Create(sr, xrs ?? new XmlReaderSettings()))
                {
                    ChoNullNSXmlSerializer serializer = new ChoNullNSXmlSerializer(type);
                    return serializer.Deserialize(xtw);
                }
            }
        }

        #endregion XmlDeserialize Overloads

        #region Serialize Overloads

        [ChoHiddenMember]
        public static void Serialize(Stream stream, object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            new BinaryFormatter().Serialize(stream, target);
        }

        [ChoHiddenMember]
        public static byte[] Serialize(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            using (MemoryStream f = new MemoryStream())
            {
                if (target != null)
                    new BinaryFormatter().Serialize(f, target);

                return f.ToArray();
            }
        }

        [ChoHiddenMember]
        public static void Serialize(string path, object target)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoDirectory.CreateDirectoryFromFilePath(path);

            using (FileStream f = File.OpenWrite(path))
            {
                if (target != null)
                    new BinaryFormatter().Serialize(f, target);
            }
        }

        #endregion Serialize Overloads

        #region Deserialize Overloads

        [ChoHiddenMember]
        public static T Deserialize<T>(Stream steam)
        {
            return (T)new BinaryFormatter().Deserialize(steam);
        }

        [ChoHiddenMember]
        public static T Deserialize<T>(byte[] buffer)
        {
            return (T)Deserialize(buffer);
        }

        [ChoHiddenMember]
        public static object Deserialize(byte[] buffer)
        {
            using (MemoryStream f = new MemoryStream(buffer))
            {
                return new BinaryFormatter().Deserialize(f);
            }
        }

        [ChoHiddenMember]
        public static T Deserialize<T>(string path)
        {
            return (T)Deserialize(path);
        }

        [ChoHiddenMember]
        public static object Deserialize(string path)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");

            using (FileStream f = File.OpenRead(path))
            {
                return f.Length > 0 ? new BinaryFormatter().Deserialize(f) : null;
            }
        }

        #endregion Deserialize Overloads

        #region JsonSerialize Overloads

        public static MemoryStream AsStream(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                return memStream;
            }
        }

        [ChoHiddenMember]
        public static void JsonSerialize(Stream sr, object target)
        {
            ChoGuard.ArgumentNotNull(sr, "Stream");
            ChoGuard.ArgumentNotNull(target, "Target");

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(target.GetType());
            serializer.WriteObject(sr, target);
        }

        [ChoHiddenMember]
        public static string JsonSerialize(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            StringBuilder JsonString = new StringBuilder();
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(target.GetType());
                serializer.WriteObject(ms, target);

                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        [ChoHiddenMember]
        public static void JsonSerialize(string path, object target)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");
            ChoGuard.ArgumentNotNull(target, "Target");

            ChoDirectory.CreateDirectoryFromFilePath(Path.GetDirectoryName(ChoPath.GetFullPath(path)));

            File.WriteAllText(path, JsonSerialize(target));
        }

        #endregion JsonSerialize Overloads

        #region JsonDeserialize Overloads

        [ChoHiddenMember]
        public static T JsonDeserialize<T>(Stream sr)
        {
            return (T)JsonDeserialize(sr, typeof(T));
        }

        [ChoHiddenMember]
        public static object JsonDeserialize(Stream sr, Type type)
        {
            ChoGuard.ArgumentNotNullOrEmpty(sr, "Stream");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(sr);
        }

        [ChoHiddenMember]
        public static T JsonDeserialize<T>(string JsonString)
        {
            ChoGuard.ArgumentNotNullOrEmpty(JsonString, "JsonString");

            return (T)JsonDeserialize(JsonString, typeof(T));
        }

        [ChoHiddenMember]
        public static object JsonDeserialize(string JsonString, Type type)
        {
            ChoGuard.ArgumentNotNullOrEmpty(JsonString, "JsonString");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(JsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                return serializer.ReadObject(ms);
            }
        }

        [ChoHiddenMember]
        public static T JsonDeserializeFromFile<T>(string path)
        {
            return (T)JsonDeserializeFromFile(path, typeof(T));
        }

        [ChoHiddenMember]
        public static object JsonDeserializeFromFile(string path, Type type)
        {
            ChoGuard.ArgumentNotNullOrEmpty(path, "Path");
            ChoGuard.ArgumentNotNullOrEmpty(type, "Type");

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(AsStream(path));
        }

        #endregion JsonDeserialize Overloads

        #region FromXml Overloads

        public static IEnumerable<object> FromXml(string xml)
        {
            ChoGuard.ArgumentNotNullOrEmpty(xml, "Xml");

            return FromXml(new StringReader(xml));
        }

        public static IEnumerable<object> FromXml(Stream xmlStream)
        {
            ChoGuard.ArgumentNotNull(xmlStream, "XmlStream");

            return FromXml(new StreamReader(xmlStream));
        }

        public static IEnumerable<object> FromXml(StreamReader xmlReader)
        {
            return FromXml(xmlReader);
        }

        public static IEnumerable<object> FromXml(TextReader xmlReader)
        {
            string nodeName = null;

            XDocument doc = XDocument.Load(xmlReader);
            if (doc == null || doc.Root == null) yield break;

            foreach (XElement element in doc.Root.Elements())
            {
                nodeName = element.GetNodeName();

                if (element.IsSkipped())
                {
                    //ChoProfile.WriteLine("{0}: Skipping workflow...".FormatString(nodeName));
                    continue;
                }
                Type type = ChoType.GetTypeFromXmlSectionNode(element);
                if (type == null)
                    throw new ChoApplicationException("Can't discover type for '{0}' node.".FormatString(nodeName));

                yield return element.ToObject(type);
            }
        }

        #endregion FromXml Overloads

        //[ChoHiddenMember]
        //public static object ConvertToObject(object inObject)
        //{
        //    if (inObject == null) return null;

        //    object[] objects = ChoString.Split2Objects(inObject.ToString());
        //    return objects != null && objects.Length > 0 ? objects[0] : DBNull.Value;
        //}

        //[ChoHiddenMember]
        //public static object[] ConvertToObjects(object[] inObjects)
        //{
        //    ArrayList retObjects = new ArrayList();
        //    foreach (object inObject in inObjects)
        //        retObjects.Add(ConvertToObject(inObject));

        //    return retObjects.ToArray();
        //}

        [ChoHiddenMember]
        public static Array Trim(object[] values, Type type)
        {
            ArrayList outValues = new ArrayList();
            foreach (object value in values)
            {
                if (value == null) continue;
                outValues.Add(value);
            }

            if (type == null)
                return outValues.ToArray();
            else
                return outValues.ToArray(type);
        }

        [ChoHiddenMember]
        public static Array Trim(object[] values)
        {
            return Trim(values, null);
        }

        [ChoHiddenMember]
        public static object CreateInstance(Type type)
        {
            if (type == null) return null;

            ConstructorInfo defaultConstrutor = null;
            ConstructorInfo[] constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (ConstructorInfo constructorInfo in constructorInfos)
            {
                ParameterInfo[] parameterInfo = constructorInfo.GetParameters();
                if (parameterInfo.Length == 0)
                {
                    defaultConstrutor = constructorInfo;
                    break;
                }
            }
            if (defaultConstrutor == null)
                throw new ApplicationException(String.Format("Missing default constructor in {0} type.", type.FullName));

            return defaultConstrutor.Invoke(null);
        }

        [ChoHiddenMember]
        public static bool IsInterceptableObject(object value)
        {
            if (value == null)
                throw new NullReferenceException("value");

            return ChoType.IsRealProxyObject(value.GetType());
        }

        public static void SetObjectMemberValue(object target, string memberName, object value)
        {
            SetObjectMemberValue(target, CheckNExtractMemberInfo(target, memberName), value);
        }

        [ChoHiddenMember]
        public static void SetObjectMemberValue(object target, MemberInfo memberInfo, object value)
        {
            if (memberInfo == null) return;

            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            ChoType.SetMemberValue(target, memberInfo, value);
        }

        public static void SetObjectMemberConvertedValue(object target, string memberName, object value)
        {
            SetObjectMemberConvertedValue(target, CheckNExtractMemberInfo(target, memberName), value);
        }

        [ChoHiddenMember]
        public static void SetObjectMemberConvertedValue(object target, MemberInfo memberInfo, object value)
        {
            if (memberInfo == null) return;

            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            if (target is Type)
                ChoType.SetMemberValue(null, memberInfo, ChoConvert.ConvertFrom(value, memberInfo, target));
            else
                ChoType.SetMemberValue(target, memberInfo, ChoConvert.ConvertFrom(value, memberInfo, target));

            //if (target is Type)
            //    ChoType.SetMemberValue(null, memberInfo, ChoConvert.ConvertTo(memberValue, ChoType.GetMemberType((Type)target, memberInfo.Name),
            //        ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo)));
            //else
            //    ChoType.SetMemberValue(target, memberInfo, ChoConvert.ConvertTo(target, memberValue, ChoType.GetMemberType(target.GetType(), memberInfo.Name),
            //        ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo)));
        }

        [ChoHiddenMember]
        public static bool HasMember(object target, string memberName)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            MemberInfo memberInfo = ChoType.GetMemberInfo(target.GetType(), memberName);
            if (memberInfo == null)
                return false;
            if (memberInfo.MemberType != MemberTypes.Field
                && memberInfo.MemberType != MemberTypes.Property)
                return false;
            return true;
        }

        [ChoHiddenMember]
        public static object GetObjectMemberValue(object target, string memberName)
        {
            return GetObjectMemberValue(target, CheckNExtractMemberInfo(target, memberName));
        }

        [ChoHiddenMember]
        public static object GetObjectMemberValue(object target, MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            object memberValue = ChoType.GetMemberValue(target, memberInfo);

            if (memberValue == null)
                memberValue = memberInfo.GetDefaultValue();

            return memberValue;
        }

        [ChoHiddenMember]
        public static object GetDefaultValue(object target, string memberName)
        {
            return GetDefaultValue(target, CheckNExtractMemberInfo(target, memberName));
        }

        [ChoHiddenMember]
        public static object GetDefaultValue(object target, MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            return memberInfo.GetDefaultValue();
        }

        [ChoHiddenMember]
        public static object GetConvertedDefaultValue(object target, string memberName)
        {
            return GetConvertedDefaultValue(target, CheckNExtractMemberInfo(target, memberName));
        }

        [ChoHiddenMember]
        public static object GetConvertedDefaultValue(object target, MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            return memberInfo.GetConvertedDefaultValue();
        }

        //[ChoHiddenMember]
        //public static object GetConvertedObjectMemberValue(object target, string memberName)
        //{
        //    return GetConvertedObjectMemberValue(target, CheckNExtractMemberInfo(target, memberName));
        //}

        //[ChoHiddenMember]
        //public static object GetConvertedObjectMemberValue(object target, MemberInfo memberInfo)
        //{
        //    ChoGuard.ArgumentNotNull(target, "Target");
        //    ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

        //    object memberValue = GetObjectMemberValue(target, memberInfo);
        //    return ChoConvert.ConvertFrom(target, memberValue, ChoType.GetMemberType(target.GetType(), memberInfo.Name),
        //        ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo));
        //}

        [ChoHiddenMember]
        public static object ConvertValueToObjectMemberType(object target, string memberName, object value)
        {
            return ConvertValueToObjectMemberType(target, CheckNExtractMemberInfo(target, memberName), value);
        }

        [ChoHiddenMember]
        public static object ConvertValueToObjectMemberType(object target, MemberInfo memberInfo, object value)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            object memberValue = value;

            if (target is Type)
                return ChoConvert.ConvertFrom(value, memberInfo, null);
            else
                return ChoConvert.ConvertFrom(value, memberInfo, target);
        }

        [ChoHiddenMember]
        public static object GetPersistableMemberValue(object target, MemberInfo memberInfo)
        {
            ChoGuard.ArgumentNotNull(target, "Target");
            ChoGuard.ArgumentNotNull(memberInfo, "MemberInfo");

            object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);
            object persistValue = memberValue;
            string propertyName = ChoType.GetMemberName(memberInfo);

            if (target is ChoConfigurableObject && ((ChoConfigurableObject)target).RaiseBeforeConfigurationObjectMemberPersist(memberInfo.Name, propertyName, ref persistValue))
                return persistValue;

            return memberValue;
        }

        public void Reset()
        {
            ChoObject.ResetObject(this);
        }

        public static void ResetObject(object target)
        {
            if (target == null)
                return;

            //MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(target.GetType());
            if (memberInfos != null && memberInfos.Length > 0)
            {
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    try
                    {
                        object o = memberInfo.GetConvertedDefaultValue();
                        if (o != null)
                            ChoType.SetMemberValue(target, memberInfo, o);
                    }
                    catch
                    {
                    }
                }
            }

        }

        [ChoHiddenMember]
        public static int Compare(object obj1, object obj2)
        {
            if (System.Object.ReferenceEquals(obj1, obj2))
                return 0;

            if (obj1 == null)
                return -1;

            if (obj2 == null)
                return 1;

            if (obj1.GetType().IsSimple())
                return ((IComparable)obj1).CompareTo(obj2);
            else
            {
                MemberInfo[] memberInfos = obj1.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetField | BindingFlags.GetProperty);
                if (memberInfos == null || memberInfos.Length == 0)
                    return 0;
                else
                {
                    int retValue = 0;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        if (!ChoType.IsValidObjectMember(memberInfo))
                            continue;

                        if (ChoType.GetMemberAttribute(memberInfo, typeof(ChoIgnoreCompareAttribute)) != null)
                            continue;

                        retValue += Compare(ChoType.GetMemberValue(obj1, memberInfo), ChoType.GetMemberValue(obj2, memberInfo));
                    }

                    return retValue;
                }
            }
        }

        //[ChoHiddenMember]
        //public static bool CompareEquals(object obj1, object obj2)
        //{
        //    // If both are null, or both are same instance, return true.
        //    if (System.Object.ReferenceEquals(obj1, obj2))
        //        return true;
        //    if (object.ReferenceEquals(obj1, null))
        //        return false;
        //    if (object.ReferenceEquals(obj2, null))
        //        return false;

        //    bool isEquatableObj = obj1.GetType().IsImplGenericTypeDefinition(typeof(IEquatable<>));
        //    if (isEquatableObj)
        //        return obj1.Equals(obj2);

        //    isEquatableObj = obj2.GetType().IsImplGenericTypeDefinition(typeof(IEquatable<>));
        //    if (isEquatableObj)
        //        return obj2.Equals(obj1);

        //    ChoEqualityComparerAttribute attribute = obj1.GetType().GetCustomAttribute(typeof(ChoEqualityComparerAttribute)) as ChoEqualityComparerAttribute;
        //    if (attribute == null)
        //        attribute = obj2.GetType().GetCustomAttribute(typeof(ChoEqualityComparerAttribute)) as ChoEqualityComparerAttribute;

        //    if (attribute != null)
        //        return attribute.IsEqualCompare(obj1, obj2);
        //    else
        //        return Equals(obj1, obj2);
        //}

        [ChoHiddenMember]
        public static bool Equals<T>(T obj1, T obj2)
        {
            Type objType = typeof(T);

            if (objType.IsSimple())
                return obj1.Equals(obj2);
            else
            {
                if (!objType.IsValueType)
                {
                    // If both are null, or both are same instance, return true.
                    if (object.ReferenceEquals(obj1, obj2))
                        return true;

                    //If one of the object is null, return false;
                    if (object.ReferenceEquals(obj1, null))
                        return false;
                    if (object.ReferenceEquals(obj2, null))
                        return false;
                }

                bool isEquatableObj = objType.IsImplGenericTypeDefinition(typeof(IEquatable<>));

                if (isEquatableObj)
                {
                    //if (objType.IsValueType)
                    return obj1.Equals(obj2);
                    //return ((IEquatable<T>)obj1).Equals(obj2);
                    //else
                    //    return obj1.Equals(obj2);
                }
                else
                {
                    //if (objType.IsValueType)
                    //    return obj1.Equals(obj2);
                    //else
                    return MemberwiseEquals(obj1, obj2);
                }
            }
        }

        [ChoHiddenMember]
        public new static bool Equals(object obj1, object obj2)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(obj1, obj2))
                return true;
            //If one of the object is null, return false;
            if (object.ReferenceEquals(obj1, null))
                return false;
            if (object.ReferenceEquals(obj2, null))
                return false;

            if (obj1.GetType().IsSimple())
                return obj1.Equals(obj2);
            else
                return MemberwiseEquals(obj1, obj2);
        }

        [ChoHiddenMember]
        public static bool MemberwiseEquals<T>(T obj1, T obj2)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(obj1, obj2))
                return true;
            //If one of the object is null, return false;
            if (object.ReferenceEquals(obj1, null))
                return false;
            if (object.ReferenceEquals(obj2, null))
                return false;

            ChoEqualityComparerAttribute attribute = obj1.GetType().GetCustomAttribute(typeof(ChoEqualityComparerAttribute)) as ChoEqualityComparerAttribute;
            if (attribute == null)
                attribute = obj2.GetType().GetCustomAttribute(typeof(ChoEqualityComparerAttribute)) as ChoEqualityComparerAttribute;

            if (attribute != null)
                return attribute.IsEqualCompare(obj1, obj2);
            else
            {
                IEnumerable<MemberInfo> memberInfos = obj1.GetType().GetGetFieldsNProperties();
                if (memberInfos == null || memberInfos.Count() == 0)
                    return true;
                else
                {
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        if (!ChoType.IsValidObjectMember(memberInfo))
                            continue;

                        if (ChoType.GetMemberAttribute(memberInfo, typeof(ChoIgnoreEqualAttribute)) != null)
                            continue;

                        if (!ChoObject.Equals(GetObjectMemberValue(obj1, memberInfo), GetObjectMemberValue(obj2, memberInfo)))
                            return false;
                    }

                    return true;
                }
            }
        }

        #endregion

        #region Format Members (Public)

        [ChoHiddenMember]
        public static string Format(object value, string format)
        {
            if (value == null) return null;

            bool foundMatchingFormatter = false;
            string retValue = Format(value, format, out foundMatchingFormatter);

            return foundMatchingFormatter ? retValue : value.ToString(format);
        }

        #endregion Format Members (Public)

        #region Evaluate Members (Public)

        [ChoHiddenMember]
        public static object Evaluate(object target, string msg)
        {
            //return ChoString.Evaluate(target, msg);
            return ChoString.ExpandProperties(target, msg);
        }

        #endregion Evaluate Members (Public)

        #region Shared Members (Internal)

        [ChoHiddenMember]
        internal static string Format(object value, string format, out bool foundMatchingFormatter)
        {
            foundMatchingFormatter = false;

            if (value == null)
                return null;

            if (format == null)
                format = String.Empty;

            ChoTypeObjectFormatInfo[] typeObjectsFormatInfo = ChoTypesManager.GetTypeObjectsFormatInfo();
            if (typeObjectsFormatInfo != null && typeObjectsFormatInfo.Length > 0)
            {
                foreach (ChoTypeObjectFormatInfo typeObjectFormatInfo in typeObjectsFormatInfo)
                {
                    if (typeObjectFormatInfo.CanFormat(value))
                    {
                        foundMatchingFormatter = true;
                        return typeObjectFormatInfo.Format(value, format);
                    }
                }
            }

            return null;
        }

        ////TODO: Do recursive lookup
        //[ChoHiddenMember]
        //internal static Hashtable ToPersistableHashtable(object target)
        //{
        //    ChoGuard.ArgumentNotNull(target, "Target");

        //    Hashtable keyValues = new Hashtable();

        //    Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    //MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
        //    MemberInfo[] memberInfos = target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);
        //    if (memberInfos == null || memberInfos.Length == 0)
        //        return keyValues;

        //    ChoConfigurationPropertyAttribute memberInfoAttribute = null;
        //    foreach (MemberInfo memberInfo in memberInfos)
        //    {
        //        if (configMemberInfos.ContainsKey(memberInfo.Name) && !((ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute))).Persistable)
        //            continue;

        //        memberInfoAttribute = (ChoConfigurationPropertyAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoConfigurationPropertyAttribute));
        //        if (memberInfoAttribute == null || !memberInfoAttribute.Persistable)
        //            continue;

        //        object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);

        //        keyValues.Add(memberInfoAttribute.Name, ChoConvert.ConvertTo(target, memberValue, typeof(string),
        //            ChoType.GetTypeConverters(memberInfo, typeof(ChoTypeConverterAttribute))));
        //    }
        //    return keyValues;
        //}

        //TODO: Do recursive lookup
        [ChoHiddenMember]
        internal static NameValueCollection ToPersistableNameValueCollection(object target)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            NameValueCollection nameValues = new NameValueCollection();

            //Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
            //MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(target.GetType());
            if (memberInfos == null || memberInfos.Length == 0)
                return nameValues;

            ChoPropertyInfoAttribute memberInfoAttribute = null;
            string name = null;
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                    continue;

                object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);
                memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                //if (memberInfoAttribute == null) continue;

                name = ChoType.GetMemberName(memberInfo, memberInfoAttribute);

                if (target is ChoConfigurableObject && ((ChoConfigurableObject)target).RaiseBeforeConfigurationObjectMemberPersist(memberInfo.Name, name, ref memberValue))
                {
                    nameValues.Add(name, memberValue == null ? null : memberValue.ToString());
                }
                else
                {
                    if (memberInfoAttribute != null && !memberInfoAttribute.Persistable)
                        continue;

                    memberValue = ChoType.GetMemberValue(target, memberInfo.Name);

                    nameValues.Add(name, ChoString.ToString(ChoConvert.ConvertTo(memberValue, memberInfo, typeof(string), target),
                        String.Empty, String.Empty));
                }
            }
            return nameValues;
        }

        internal static Dictionary<string, object> ToPersistableDictionaryCollection(ChoBaseConfigurationElement configElement)
        {
            return ToPersistableDictionaryCollection(configElement, typeof(string));
        }

        [ChoHiddenMember]
        internal static Dictionary<string, object> ToPersistableDictionaryCollection(ChoBaseConfigurationElement configElement, Type itemType)
        {
            ChoGuard.ArgumentNotNull(configElement, "ConfigElement");

            object target = configElement.ConfigObject;

            ChoGuard.ArgumentNotNull(target, "Target");

            Dictionary<string, object> dict = new Dictionary<string, object>();

            //Dictionary<string, MemberInfo> configMemberInfos = ChoType.GetMembersDictionary(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
            //MemberInfo[] memberInfos = ChoType.GetMembers(target.GetType(), typeof(ChoConfigurationPropertyAttribute));
            MemberInfo[] memberInfos = ChoTypeMembersCache.GetAllMemberInfos(target.GetType());
            if (memberInfos == null || memberInfos.Length == 0)
                return dict;

            ChoPropertyInfoAttribute memberInfoAttribute = null;
            string name = null;
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute<ChoIgnorePropertyAttribute>() != null)
                    continue;

                object memberValue = ChoType.GetMemberValue(target, memberInfo.Name);
                memberInfoAttribute = (ChoPropertyInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoPropertyInfoAttribute));
                name = ChoType.GetMemberName(memberInfo, memberInfoAttribute);

                if (target is ChoConfigurableObject && ((ChoConfigurableObject)target).RaiseBeforeConfigurationObjectMemberPersist(memberInfo.Name, name, ref memberValue))
                {
                    dict.Add(name, memberValue);
                }
                else
                {
                    if (memberInfoAttribute != null && !memberInfoAttribute.Persistable)
                        continue;

                    memberValue = ChoType.GetMemberValue(target, memberInfo.Name);
                    Type destType = ChoConfigurationMetaDataManager.GetSourceType(configElement, name, memberInfoAttribute);

                    if (destType == null)
                    {
                        if (itemType == typeof(Object))
                            destType = ChoType.GetMemberType(memberInfo);
                        else
                            destType = itemType;
                    }
                    dict.Add(name, ChoConvert.ConvertTo(memberValue, memberInfo, destType, target));
                    //dict.Add(name, ChoConvert.ConvertTo(target, memberValue, memberType,
                    //        ChoTypeDescriptor.GetTypeConverters(memberInfo), ChoTypeDescriptor.GetTypeConverterParams(memberInfo), null));
                }
            }
            return dict;
        }

        #endregion Shared Members (Internal)

        #region Shared Members (Private)

        [ChoHiddenMember]
        private static MemberInfo CheckNExtractMemberInfo(object target, string memberName)
        {
            ChoGuard.ArgumentNotNull(target, "Target");

            MemberInfo memberInfo = ChoType.GetMemberInfo(target.GetType(), memberName);
            if (memberInfo == null)
                throw new NullReferenceException(String.Format("Can't find {0} member in {1} type.", memberName, target.GetType().FullName));
            if (memberInfo.MemberType != MemberTypes.Field
                && memberInfo.MemberType != MemberTypes.Property)
                throw new ChoApplicationException(String.Format("Member `{0}` is not a field/property in {1} type.", memberName, target.GetType().FullName));
            return memberInfo;
        }

        #endregion Shared Members (Private)

        #region Merge Memebrs

        public static T Merge<T>(T obj1, T obj2)
        {
            return (T)Merge(obj1 as object, obj2 as object);
        }

        public static object Merge(object obj1, object obj2)
        {
            if (object.ReferenceEquals(obj1, obj2))
                return obj1;

            if (object.ReferenceEquals(obj1, null))
                return obj2;

            if (object.ReferenceEquals(obj2, null))
                return obj1;

            if (obj1 is IChoMergeable)
            {
                ((IChoMergeable)obj1).Merge(obj2);
                return obj1;
            }

            return obj1;
        }

        #endregion Merge Memebrs

        protected void RaisePropertyChanged(string propertyName)
        {
            if (propertyName.IsNullOrWhiteSpace()) return;

            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                foreach (string depends in ChoPropertyDependsOnCache.Instance.GetDependsOn(GetType(), propertyName))
                {
                    propertyChanged(this, new PropertyChangedEventArgs(depends));
                }
            }
        }
    }
}
