namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Resources;
    using System.Reflection;
    using System.Collections;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    #endregion NameSpaces

    /// <summary>
    /// Provides resource support to Cinchoo assemblies.
    /// </summary>
    public static class ChoResourceManager
    {
        #region Private Static Fields

        private static ResourceManager _sharedResourceManager;
        private static readonly Hashtable _resourceManagerDictionary = new Hashtable();

        #endregion Private Static Fields

        #region Public Static Methods

        /// <summary>
        /// Registers the assembly to be used as the fallback if resources
        /// aren't found in the local satellite assembly.
        /// </summary>
        /// <param name="assembly">
        /// A <see cref="System.Reflection.Assembly" /> that represents the
        /// assembly to register.
        /// </param>
        /// <example>
        /// The following example shows how to register a shared satellite
        /// assembly.
        /// <code>
        /// <![CDATA[
        /// Assembly sharedAssembly = Assembly.Load("MyResources.dll");
        /// ChoResourceManager.RegisterSharedAssembly(sharedAssembly);
        /// ]]>
        /// </code>
        /// </example>
        public static void RegisterSharedAssembly(Assembly assembly)
        {
            _sharedResourceManager = new ResourceManager(assembly.GetName().Name, assembly);
        }

        /// <summary>
        /// Returns the value of the specified string resource.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String" /> that contains the name of the
        /// resource to get.
        /// </param>
        /// <returns>
        /// A <see cref="System.String" /> that contains the value of the
        /// resource localized for the current culture.
        /// </returns>
        /// <remarks>
        /// The returned resource is localized for the cultural settings of the
        /// current <see cref="System.Threading.Thread" />.
        /// <note>
        /// The <c>GetString</c> method is thread-safe.
        /// </note>
        /// </remarks>
        /// <example>
        /// The following example demonstrates the <c>GetString</c> method using
        /// the cultural settings of the current <see cref="System.Threading.Thread" />.
        /// <code>
        /// <![CDATA[
        /// string localizedString = ChoResourceManager.GetString("String_HelloWorld");
        /// ]]>
        /// </code>
        /// </example>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetString(string name)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return GetString(name, null, assembly);
        }

        /// <summary>
        /// Returns the value of the specified string resource localized for
        /// the specified culture.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture"></param>
        /// <returns>
        /// A <see cref="System.String" /> that contains the value of the
        /// resource localized for the specified culture. 
        ///</returns>
        /// <remarks>
        /// <note>
        /// The <c>GetString</c> method is thread-safe.
        /// </note>
        /// </remarks>
        /// <example>
        /// The following example demonstrates the <c>GetString</c> method using
        /// a specific culture.
        /// <code>
        /// <![CDATA[
        /// CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        /// string localizedString = ChoResourceManager.GetString("String_HelloWorld", culture);
        /// ]]>
        /// </code>
        /// </example>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetString(string name, CultureInfo culture)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return GetString(name, culture, assembly);
        }

        /// <summary>
        /// Returns the value of the specified string resource localized for
        /// the specified culture for the specified assembly.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String" /> that contains the name of the
        /// resource to get.
        /// </param>
        /// <param name="culture">
        /// A <see cref="System.Globalization.CultureInfo" /> that represents
        /// the culture for which the resource is localized.
        /// </param>
        /// <param name="assembly">
        /// A <see cref="System.Reflection.Assembly" />
        /// </param>
        /// <returns>
        /// A <see cref="System.String" /> that contains the value of the
        /// resource localized for the specified culture.
        /// </returns>
        /// <remarks>
        /// <note>
        /// The <c>GetString</c> method is thread-safe.
        /// </note>
        /// </remarks>
        /// <example>
        /// The following example demonstrates the <c>GetString</c> method using
        /// specific culture and assembly.
        /// <code>
        /// <![CDATA[
        /// CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        /// Assembly assembly = Assembly.GetCallingAssembly();
        /// string localizedString = ChoResourceManager.GetString("String_HelloWorld", culture, assembly);
        /// ]]>
        /// </code>
        /// </example>
        public static string GetString(string name, CultureInfo culture, Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name;

            if (!_resourceManagerDictionary.Contains(assemblyName))
            {
                RegisterAssembly(assembly);
            }

            // retrieve resource manager for assembly
            ResourceManager resourceManager = (ResourceManager)
                _resourceManagerDictionary[assemblyName];

            // try to get the required string from the given assembly
            string localizedString = resourceManager.GetString(name, culture);

            // if the given assembly does not contain the required string, then
            // try to get it from the shared satellite assembly, if registered
            if (localizedString == null && _sharedResourceManager != null)
            {
                return _sharedResourceManager.GetString(name, culture);
            }
            return localizedString;
        }

        #endregion Public Static Methods

        #region Private Static Methods

        /// <summary>
        /// Registers the specified assembly.
        /// </summary>
        /// <param name="assembly">
        /// A <see cref="System.Reflection.Assembly" /> that represents the
        /// assembly to register.
        /// </param>
        private static void RegisterAssembly(Assembly assembly)
        {
            lock (_resourceManagerDictionary)
            {
                string assemblyName = assembly.GetName().Name;

                _resourceManagerDictionary.Add(assemblyName,
                    new ResourceManager(GetResourceName(assemblyName),
                    assembly));
            }
        }

        /// <summary>
        /// Determines the manifest resource name of the resource holding the
        /// localized strings.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>
        /// The manifest resource name of the resource holding the localized
        /// strings for the specified assembly.
        /// </returns>
        /// <remarks>
        /// The manifest resource name of the resource holding the localized
        /// strings should match the name of the assembly, minus <c>Tasks</c>
        /// suffix.
        /// </remarks>
        private static string GetResourceName(string assemblyName)
        {
            return assemblyName + ".Resources.Strings";
        }

        #endregion Private Static Methods
    }
}
