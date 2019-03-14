namespace eSquare.Core.Configuration
{
    #region NameSpaces

    using System;
    using System.IO;
    using System.Text;
    using System.Reflection;
    using System.Collections;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using eSquare.Core.Properties;
    using eSquare.Core.Exceptions;
    using eSquare.Core.Diagnostics;
    using eSquare.Core.Attributes;
    using eSquare.Core.Collections.Specialized;

    #endregion NameSpaces

    #region ChoConfigurationElementMapAttribute Class

    public abstract class ChoConfigurationElementMapHandler
    {
        #region Instance Data Members

        private object _configObject;
        private IChoConfigSectionable _configSection;
        private string _traceOutputFileName;

        #endregion

        #region Instance Members

        public abstract IChoConfigSectionable GetConfig();
        public abstract void SaveConfig(IChoConfigSectionable _configSection);

        public virtual void Construct(object configObject)
        {
            _configObject = configObject;

            LoadConfig(false);
        }

        private void ChoConfigurationChangeWatcherManager_ConfigurationChanged(object sender, ChoConfigurationChangedEventArgs e)
        {
            LoadConfig(true);
        }

        private void LoadConfig(bool refresh)
        {
            object configObject = _configObject;

            //Set default trace output file name
            _traceOutputFileName = configObject.GetType().Name;

            MemberInfo[] memberInfos = ChoType.GetMembers(configObject.GetType(), typeof(ChoMemberInfoAttribute));
            if (memberInfos == null || memberInfos.Length == 0) return;

            _configSection = GetConfig();
            if (_configSection == null)
                throw new ChoConfigurationConstructionException("Missing configuration section.");

            if (!refresh)
            {
                //Hookup the configuration watch job
                ChoConfigurationChangeWatcherManager.SetWatcherForConfigSource(_configSection.ConfigurationChangeWatcher);
                ChoConfigurationChangeWatcherManager.ConfigurationChanged += new ChoConfigurationChangedEventHandler(ChoConfigurationChangeWatcherManager_ConfigurationChanged);
            }
            else
                _configSection.ConfigurationChangeWatcher.StopWatching();

            ErrMsg = _configSection.ErrMsg;

            if (!IgnoreError && ErrMsg != null)
                throw new ApplicationException(ErrMsg);

            Hashtable hashTable = _configSection.ToHashtable();
            //Set member values
            string name;
            ChoMemberInfoAttribute memberInfoAttribute = null;
            foreach (MemberInfo memberInfo in memberInfos)
            {
                memberInfoAttribute = (ChoMemberInfoAttribute)ChoType.GetMemberAttribute(memberInfo, typeof(ChoMemberInfoAttribute));
                if (memberInfoAttribute == null) continue;

                name = memberInfoAttribute.Name;

                try
                {
                    //Set the config values
                    if (hashTable[name] != null)
                        ChoType.SetMemberValue(configObject, memberInfo.Name, hashTable[name]);
                    //Set default values
                    else if (memberInfoAttribute.DefaultValue != null)
                        ChoType.SetMemberValue(configObject, memberInfo.Name, memberInfoAttribute.DefaultValue);
                }
                catch (Exception ex)
                {
                    if (IgnoreError)
                        SetMemberError(configObject, memberInfo.Name, String.Format(Resources.ConfigConstructMsg, ChoString.ToString(hashTable[name]), ex.Message));
                    else
                        throw new ChoConfigurationConstructionException(String.Format(Resources.ConfigConstructExceptionMsg, ChoString.ToString(hashTable[name]), configObject.GetType().Name,
                            memberInfo.Name), ex);
                }

                //try
                //{
                //    //Validate the member with associated attributes
                //    foreach (IChoMemberValidator memberValidator in ChoType.GetMemberAttributesByBaseInterface(memberInfo, typeof(IChoMemberValidator)))
                //    {
                //        memberValidator.Validate(configObject, memberInfo);
                //    }
                //    foreach (ChoConfigurationValidatorAttribute memberValidator in ChoType.GetMemberAttributesByBaseInterface(memberInfo, typeof(ChoConfigurationValidatorAttribute)))
                //    {
                //        if (memberValidator.ValidatorInstance.CanValidate(ChoType.GetMemberType(memberInfo)))
                //            memberValidator.ValidatorInstance.Validate(ChoType.GetMemberValue(configObject, memberInfo.Name));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    if (IgnoreError)
                //        SetMemberError(configObject, memberInfo.Name, String.Format(Resources.ConfigConstructValidationMsg, ChoString.ToString(ChoType.GetMemberValue(configObject, memberInfo.Name)),
                //            ex.Message));
                //    else
                //        throw new ChoConfigurationConstructionException(String.Format(Resources.ConfigConstructValidationExceptionMsg, ChoString.ToString(ChoType.GetMemberValue(configObject, memberInfo.Name))), ex);
                //}
            }

            _configSection.ConfigurationChangeWatcher.StartWatching();

            //Print the output to file
            TraceOutput(configObject);
        }

        public virtual void TraceOutput(object configObject)
        {
            ChoFileProfile.Clean(TraceOutputDirectory, _traceOutputFileName);

            if (!AutoTrace)
            {
                ChoFileProfile.WriteLine(TraceOutputDirectory, _traceOutputFileName, "AutoTrace is turned off.");
                return;
            }

            try
            {
                if (ErrMsg != null)
                {
                    if (this.DisplayOnConsole) Console.WriteLine(ErrMsg);
                    ChoFileProfile.WriteLine(TraceOutputDirectory, _traceOutputFileName, ErrMsg);
                }

                string msg = ChoConfigurationManagementFactory.ToString(configObject);
                if (this.DisplayOnConsole) Console.WriteLine(msg);
                ChoFileProfile.WriteLine(TraceOutputDirectory, _traceOutputFileName, msg);
            }
            catch (Exception ex)
            {
                ChoTrace.Write(ex);
                throw;
            }
        }

        public void SetError(object target, string errMsg)
        {
            ChoType.SetAttributeNameParameterValue(target.GetType(), typeof(ChoConfigurationElementMapAttribute), "ErrMsg", errMsg);
        }

        public void SetMemberError(object target, string memberName, string errMsg)
        {
            ChoType.SetAttributeNameParameterValue(target.GetType(), memberName, typeof(ChoMemberInfoAttribute), "ErrMsg", errMsg);
        }

        #endregion Instance Members
    }

    #endregion ChoConfigurationElementMapAttribute Class
}
