#region Header

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
 * Copyright (c) 2009-2010 Raj Nagalingam.
 *    All rights reserved.
 *
 * This program and the accompanying materials are made available under
 * the terms of the Common Public License v1.0 which accompanies this
 * distribution.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in
 * the documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
 * TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *<author>Raj Nagalingam</author>
 *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

#endregion Header

namespace Cinchoo.Core.Settings
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Pattern.Singleton;
    using Cinchoo.Core.Pattern.Singleton.Attributes;

    #endregion NameSpaces

    /// <summary>
    /// Application level settings class, hold information about application
    /// </summary>
    [ChoCustomSingletonType]
    [Serializable]
    [XmlRoot("applicationSettings")]
    public sealed class ChoApplicationSettings
    {
        #region Instance Data Members (Public)

        /// <summary>
        /// Event log source name, default to ApplicationName
        /// </summary>
        [XmlAttribute("eventLogSourceName")]
        public string EventLogSourceName;

        private string _applicationName;
        /// <summary>
        /// Application Name, default to executable file name
        /// </summary>
        [XmlAttribute("applicationName")]
        public string ApplicationName
        {
            get { return _applicationName; }
            set 
            {
                _applicationName = value;
                if (String.IsNullOrEmpty(EventLogSourceName))
                    EventLogSourceName = _applicationName;
            }
        }

        #endregion Instance Data Members (Public)

        //#region Shared Members (Public)

        ///// <summary>
        ///// Factory method to create an instance of this object
        ///// </summary>
        ///// <param name="appConfigFilePath">Application executable configuration file path</param>
        ///// <returns>Newly created this object</returns>
        //[ChoSingletonFactoryMethod]
        //public static ChoApplicationSettings CreateInstance(string appConfigFilePath)
        //{
        //    ChoGuard.ArgumentNotNullOrEmpty(appConfigFilePath, "AppConfigPath");

        //    //Load the configuration section
        //    ChoApplicationSettings applicationSettings = ChoConfigurationManager.GetConfigFromConfigFile(typeof(ChoApplicationSettings),
        //        appConfigFilePath, @"//choCommon/applicationSettings") as ChoApplicationSettings;

        //    if (applicationSettings != null)
        //    {
        //        //If not specified, default to application executable file name
        //        if (String.IsNullOrEmpty(applicationSettings.ApplicationName))
        //            applicationSettings.ApplicationName = Path.GetFileNameWithoutExtension(appConfigFilePath.Replace(".config", String.Empty));

        //        ////if not specified, default to ApplicationName
        //        //if (String.IsNullOrEmpty(applicationSettings.EventLogSourceName))
        //        //    applicationSettings.EventLogSourceName = applicationSettings.ApplicationName;
        //    }
        //    else
        //    {
        //        applicationSettings = new ChoApplicationSettings();
        //        applicationSettings.EventLogSourceName = applicationSettings.ApplicationName = Path.GetFileNameWithoutExtension(appConfigFilePath.Replace(".config", String.Empty));
        //    }

        //    return applicationSettings;
        //}

        ///// <summary>
        ///// Helper method to get the singleton instance of this type
        ///// </summary>
        ///// <param name="appConfigFilePath">Application configuration file path</param>
        ///// <returns></returns>
        //public static ChoApplicationSettings GetMe(string appConfigFilePath)
        //{
        //    return ChoSingleton<ChoApplicationSettings>.GetInstance(SingletonTypeValidationRules.AllowStaticMembers | SingletonTypeValidationRules.AllowPublicConstructors, appConfigFilePath);
        //}

        //public static ChoApplicationSettings Me
        //{
        //    get { return ChoSingleton<ChoApplicationSettings>.Instance; }
        //}

        //#endregion Shared Members (Public)

        //#region Object Overrides (Public)

        ///// <summary>
        ///// Returns a System.String that represents the current ChoApplicationSettings
        ///// </summary>
        ///// <returns>A System.String that represents the current ChoApplicationSettings</returns>
        //public override string ToString()
        //{
        //    return ChoObject.ToString(this);
        //}

        //#endregion Object Overrides (Public)
    }
}
