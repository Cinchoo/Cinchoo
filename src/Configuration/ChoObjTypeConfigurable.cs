namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Text;
    using System.Xml.Serialization;
    using System.Collections.Generic;

    using eSquare.Core.Attributes;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Collections.Generic;
    using eSquare.Core.Interfaces;

    #endregion NameSpaces

    public class ChoObjConfigurable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("action")]
        public Action Action = Action.Add;

        #endregion Instance Data Members (Public)
    }

    public class ChoObjNameConfigurable : ChoObjConfigurable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("name")]
        public string Name;

        #endregion Instance Data Members (Public)
    }

    public class ChoObjNamePriorityConfigurable : ChoObjNameConfigurable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("priority")]
        public int Priority;

        #endregion Instance Data Members (Public)
    }

    public class ChoObjPriorityConfigurable : ChoObjConfigurable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("priority")]
        public int Priority;

        #endregion Instance Data Members (Public)
    }

    public class ChoObjTypeConfigurable : ChoObjNameConfigurable
    {
        #region Instance Data Members (Public)

        [XmlAttribute("type")]
        public string Type;

        #endregion Instance Data Members (Public)

        #region Shared Members (Public)

        #region Load (TypeName) Overloads

        public static void Load<T>(string logFileName, string typeName, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
        {
            if (String.IsNullOrEmpty(typeName)) return;
            Load<T>(logFileName, new string[] { typeName }, objDictionary, objTypeConfigurables, defaultObjectKey);
        }

        public static void Load<T>(string logFileName, string typeName, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables) where T : class
        {
            Load<T>(logFileName, typeName, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
        }
        
        public static void Load<T>(string logFileName, string[] typeNames, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables) where T : class
        {
            Load<T>(logFileName, typeNames, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
        }

        public static void Load<T>(string logFileName, string[] typeNames, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
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
            ChoObjTypeConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
        {
            if (type == null) return;
            Load<T>(logFileName, new Type[] { type }, objDictionary, objTypeConfigurables, defaultObjectKey);
        }

        public static void Load<T>(string logFileName, Type type, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables) where T : class
        {
            Load<T>(logFileName, type, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
        }

        public static void Load<T>(string logFileName, Type[] types, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables) where T : class
        {
            Load<T>(logFileName, types, objDictionary, objTypeConfigurables, ChoDefaultObjectKey.FullName);
        }

        public static void Load<T>(string logFileName, Type[] types, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables, ChoDefaultObjectKey defaultObjectKey) where T : class
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
                else
                    ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(logFileName, ChoExt.Err),
                        String.Format("Failed to create {0} object.", typeName));
            }
            catch (Exception ex)
            {
                ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(logFileName, ChoExt.Err),
                    String.Format("Failed to create {0} object. {1}", typeName, ex.Message));
            }
        }

        private static void Adjust<T>(string logFileName, ChoDictionary<string, T> objDictionary,
            ChoObjTypeConfigurable[] objTypeConfigurables, ChoDefaultObjectKey typeName) where T : class
        {
            if (objTypeConfigurables != null && objTypeConfigurables.Length > 0)
            {
                foreach (ChoObjTypeConfigurable objTypeConfigurable in objTypeConfigurables)
                {
                    if (objTypeConfigurable == null) continue;
                    try
                    {
                        string key = objTypeConfigurable.Name;

                        T obj = default(T);
                        if (!String.IsNullOrEmpty(objTypeConfigurable.Type))
                        {
                            obj = ChoObjectManagementFactory.CreateInstance(objTypeConfigurable.Type) as T;
                            if (obj == null)
                                ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(logFileName, ChoExt.Err),
                                    String.Format("Failed to create {0} object.", objTypeConfigurable.Type));
                        }

                        if (obj != null)
                            key = ChoObjectNameableAttribute.GetName(obj.GetType(), typeName);

                        if (objTypeConfigurable.Action == Action.Remove &&
                            objDictionary.ContainsKey(key))
                            objDictionary.Remove(key);
                        else if (!objDictionary.ContainsKey(key) && obj != null)
                            objDictionary.Add(key, obj);
                    }
                    catch (Exception ex)
                    {
                        ChoStreamProfile.WriteLine(ChoLogDirectories.Settings, Path.ChangeExtension(logFileName, ChoExt.Err),
                            String.Format("Failed to create {0} object. {1}", objTypeConfigurable.Type, ex.Message));
                    }
                }
            }
        }

        #endregion Shared Members (Private)
    }
}
