namespace Cinchoo.Core.Common
{
	#region NameSpaces

    using System;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinchoo.Core.Collections.Generic;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Diagnostics;

    #endregion NameSpaces

    [Serializable]
	public class ChoObjConfigurable
	{
		#region Instance Data Members (Public)

		[XmlAttribute("action")]
		public ConfigAction Action = ConfigAction.Add;

		[XmlAttribute("name")]
		public string Name;

		[XmlAttribute("priority")]
		public int Priority;

		[XmlAttribute("type")]
		public string Type;

		#endregion Instance Data Members (Public)

		#region Object Overrrides

		public override string ToString()
		{
			return Name;
		}
		#endregion Object Overrides

		#region Shared Members (Public)

		#region GetDistinct Members

		public ChoObjConfigurable[] ConvertToDistinctArray(ChoObjConfigurable[] objConfigurables)
		{
			if (ChoGuard.IsArgumentNotNullOrEmpty(objConfigurables)) return objConfigurables;

			ChoDictionary<string, ChoObjConfigurable> _distinctObjectConfigurables = new ChoDictionary<string, ChoObjConfigurable>();
			foreach (ChoObjConfigurable objConfigurable in objConfigurables)
			{
				if (objConfigurable == null) continue;
				if (String.IsNullOrEmpty(objConfigurable.Name)) continue;

				if (_distinctObjectConfigurables.ContainsKey(objConfigurable.Name))
				{
                    ChoTrace.Debug(String.Format("Item with {0} key already exists.", objConfigurable.Name));
					continue;
				}

				_distinctObjectConfigurables.Add(objConfigurable.Name, objConfigurable);
			}

			return _distinctObjectConfigurables.ToValuesArray();
		}

		public ChoDictionary<string, ChoObjConfigurable> ConvertToDistinctDictionary(ChoObjConfigurable[] objConfigurables)
		{
			ChoDictionary<string, ChoObjConfigurable> _distinctObjectConfigurables = new ChoDictionary<string, ChoObjConfigurable>();

			if (ChoGuard.IsArgumentNotNullOrEmpty(objConfigurables)) return _distinctObjectConfigurables;

			foreach (ChoObjConfigurable objConfigurable in objConfigurables)
			{
				if (objConfigurable == null) continue;
				if (String.IsNullOrEmpty(objConfigurable.Name)) continue;

				if (_distinctObjectConfigurables.ContainsKey(objConfigurable.Name))
				{
                    ChoTrace.Debug(String.Format("Item with {0} key already exists.", objConfigurable.Name));
					continue;
				}

				_distinctObjectConfigurables.Add(objConfigurable.Name, objConfigurable);
			}

			return _distinctObjectConfigurables;
		}

		#endregion GetDistinct Members

		#region Load (TypeName) Overloads

		public static void Load<T>(string logFileName, string typeName, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
		{
			if (String.IsNullOrEmpty(typeName)) return;
			Load<T>(logFileName, new string[] { typeName }, objDictionary, objTypeConfigurables, defaultObjectKey);
		}

		public static void Load<T>(string logFileName, string typeName, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables) where T : class
		{
			Load<T>(logFileName, typeName, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
		}

		public static void Load<T>(string logFileName, string[] typeNames, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables) where T : class
		{
			Load<T>(logFileName, typeNames, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
		}

		public static void Load<T>(string logFileName, string[] typeNames, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
		{
			ChoGuard.ArgumentNotNull(logFileName, "LogFileName");
			ChoGuard.ArgumentNotNull(objDictionary, "ObjectDictionary");

			if (typeNames == null || typeNames.Length == 0) return;

			foreach (string typeName in typeNames)
			{
				if (String.IsNullOrEmpty(typeName)) continue;
				Add<T>(logFileName, objDictionary, defaultObjectKey, typeName);
			}
			Adjust<T>(logFileName, objDictionary, objTypeConfigurables, defaultObjectKey);
		}

		#endregion Load (TypeName) Overloads

		#region Load (Type) Overloads

		public static void Load<T>(string logFileName, Type type, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
		{
			if (type == null) return;
			Load<T>(logFileName, new Type[] { type }, objDictionary, objTypeConfigurables, defaultObjectKey);
		}

		public static void Load<T>(string logFileName, Type type, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables) where T : class
		{
			Load<T>(logFileName, type, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
		}

		public static void Load<T>(string logFileName, Type[] types, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables) where T : class
		{
			Load<T>(logFileName, types, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
		}

		public static void Load<T>(string logFileName, Type[] types, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
		{
			ChoGuard.ArgumentNotNull(logFileName, "LogFileName");
			ChoGuard.ArgumentNotNull(objDictionary, "ObjectDictionary");

			if (types == null || types.Length == 0) return;

			foreach (Type type in types)
				Add<T>(logFileName, objDictionary, defaultObjectKey, type.AssemblyQualifiedName);

			Adjust<T>(logFileName, objDictionary, objTypeConfigurables, defaultObjectKey);
		}

		#endregion Load (Type) Overloads

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static void Add<T>(string logFileName, ChoDictionary<string, T> objDictionary, ChoDefaultObjectKey defaultObjectKey, string typeName) where T : class
		{
			try
			{
				T obj = ChoObjectManagementFactory.CreateInstance(typeName) as T;
				if (obj != null)
				{
					string key;
					if (obj is IChoObjectNameable)
						key = ((IChoObjectNameable)obj).Name;
					else
						key = ChoObjectNameableAttribute.GetName(obj.GetType(), defaultObjectKey);

					ChoGuard.NotNullOrEmpty(key, String.Format("{0}: Name can't be empty.", typeName));

					if (!objDictionary.ContainsKey(key))
						objDictionary.Add(key, obj);
				}
				//else
				//    ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, Path.ChangeExtension(logFileName, ChoReservedFileExt.Err),
				//        String.Format("Failed to create {0} object.", typeName));
			}
			catch (Exception)
			{
				//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, Path.ChangeExtension(logFileName, ChoReservedFileExt.Err),
				//    String.Format("Failed to create {0} object. {1}", typeName, ex.Message));
			}
		}

		private static void Adjust<T>(string logFileName, ChoDictionary<string, T> objDictionary,
			ChoObjConfigurable[] objTypeConfigurables, ChoDefaultObjectKey typeName) where T : class
		{
			if (objTypeConfigurables != null && objTypeConfigurables.Length > 0)
			{
				foreach (ChoObjConfigurable objTypeConfigurable in objTypeConfigurables)
				{
					if (objTypeConfigurable == null) continue;
					try
					{
						string key = objTypeConfigurable.Name;

						T obj = default(T);
						if (!String.IsNullOrEmpty(objTypeConfigurable.Type))
						{
							obj = ChoObjectManagementFactory.CreateInstance(objTypeConfigurable.Type) as T;
							//if (obj == null)
							//    ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, Path.ChangeExtension(logFileName, ChoReservedFileExt.Err),
							//        String.Format("Failed to create {0} object.", objTypeConfigurable.Type));
						}

						if (obj != null)
							key = ChoObjectNameableAttribute.GetName(obj.GetType(), typeName);

						if (objTypeConfigurable.Action == ConfigAction.Remove &&
							objDictionary.ContainsKey(key))
							objDictionary.Remove(key);
						else if (!objDictionary.ContainsKey(key) && obj != null)
							objDictionary.Add(key, obj);
					}
					catch (Exception)
					{
						//ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, Path.ChangeExtension(logFileName, ChoReservedFileExt.Err),
						//    String.Format("Failed to create {0} object. {1}", objTypeConfigurable.Type, ex.Message));
					}
				}
			}
		}

		#endregion Shared Members (Private)
	}
}
