using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;

namespace Cinchoo.Core.Shell
{
    public static class ChoShellExtension
    {
        private const string SHELL_EXT_CMD_DELIMITER = "#";

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public static void Register(string fileType, string shellKeyName, string menuText = null, string menuCommand = null, string icon = null)
        {
            if (fileType.IsNullOrWhiteSpace() && shellKeyName.IsNullOrWhiteSpace())
                return;

            if (menuText.IsNullOrWhiteSpace())
                menuText = shellKeyName;
            if (menuCommand.IsNullOrWhiteSpace())
                menuCommand = @"""{0}"" ""%1""".FormatString(ChoApplication.EntryAssemblyLocation);

            // create path to registry location
            string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);

                // add icon
                if (!icon.IsNullOrWhiteSpace())
                    key.SetValue("Icon", icon);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(string.Format(@"{0}\command", regPath)))
                key.SetValue(null, menuCommand);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
        public static void Unregister(string fileType, string shellKeyName)
        {
            if (fileType.IsNullOrWhiteSpace() && shellKeyName.IsNullOrWhiteSpace())
                return;

            // path to the registry location
            string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

            // remove context menu from the registry
            Registry.ClassesRoot.DeleteSubKeyTree(regPath, false);
        }

        public static bool ExecuteShellExtensionMethodIfAnySpecified(string[] args)
        {
            if (args.Length > 1)
            {
                if (IsShellExtensionMethod(args[0]))
                {
                    ExecuteShellExtensionMethod(args[0].Replace(SHELL_EXT_CMD_DELIMITER, String.Empty), ExpandCmdLineArgs(args.Skip(1)).ToArray());
                    return true;
                }
            }

            return false;
        }

        private static void ExecuteShellExtensionMethod(string command, string[] args)
        {
            ChoEnvironment.CommandLineArgs = args;
            foreach (MethodInfo methodInfo in GetShellExtensionMethods())
            {
                ChoShellExtensionContextMenuAttribute attr = methodInfo.GetCustomAttribute<ChoShellExtensionContextMenuAttribute>();
                if (attr == null) continue;
                if (methodInfo.Name == command)
                {
                    try
                    {
                        methodInfo.Invoke(null, new object[] { args });
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.WriteLine("Error while executing '{0}' shell extension command. \n {1}".FormatString(command, ChoApplicationException.ToString(ex)));
                        ChoApplication.WriteToEventLog("Error while executing '{0}' shell extension command. \n {1}".FormatString(command, ChoApplicationException.ToString(ex)));
                    }

                    return;
                }
            }

            ChoApplication.WriteToEventLog("'{0}' shell extension command not found.".FormatString(command));
        }

        private static bool IsShellExtensionMethod(string command)
        {
            if (command.IsNullOrWhiteSpace())
                return false;
            else
                return command.StartsWith(SHELL_EXT_CMD_DELIMITER) && command.EndsWith(SHELL_EXT_CMD_DELIMITER);
        }

        private static IEnumerable<string> ExpandCmdLineArgs(IEnumerable<string> args)
        {
            foreach (string arg in args)
                yield return arg.ExpandProperties();
        }

        public static void Unregister()
        {
            foreach (MethodInfo methodInfo in GetShellExtensionMethods())
            {
                ChoShellExtensionContextMenuAttribute attr = methodInfo.GetCustomAttribute<ChoShellExtensionContextMenuAttribute>();
                if (attr == null) continue;

                string methodName = methodInfo.Name;
                string fileType = attr.FileType;
                string shellKeyName = attr.ShellKeyName;

                shellKeyName = shellKeyName.IsNullOrWhiteSpace() ? methodName : shellKeyName;

                ChoShellExtension.Unregister(fileType, shellKeyName);
                ChoTrace.WriteLine("Shell Extensions unregistered successfully.");
            }
        }

        public static void Register()
        {
            foreach (MethodInfo methodInfo in GetShellExtensionMethods())
            {
                ChoShellExtensionContextMenuAttribute attr = methodInfo.GetCustomAttribute<ChoShellExtensionContextMenuAttribute>();
                if (attr == null) continue;

                string methodName = methodInfo.Name;
                string fileType = attr.FileType;
                string menuText = attr.MenuText;
                string shellKeyName = attr.ShellKeyName;
                string icon = attr.Icon;
                StringBuilder additionalCmdLineArgs = new StringBuilder();

                foreach (string addCmdLineArg in attr.AdditionalCommandLineArgs.NSplit('%', false))
                {
                    if (addCmdLineArg.StartsWith("%") && addCmdLineArg.EndsWith("%")
                        && !addCmdLineArg.StartsWith("%%") && !addCmdLineArg.EndsWith("%%"))
                        additionalCmdLineArgs.AppendFormat(@"%{0}%", addCmdLineArg);
                    else
                        additionalCmdLineArgs.AppendFormat(@"{0}", addCmdLineArg);
                }

                string z = additionalCmdLineArgs.ToString();
                additionalCmdLineArgs.Clear();
                foreach (string addCmdLineArg in z.SplitNTrim())
                {
                    //if (addCmdLineArg.StartsWith("%") && addCmdLineArg.EndsWith("%")
                    //    && !addCmdLineArg.StartsWith("%%") && !addCmdLineArg.EndsWith("%%"))
                    //    additionalCmdLineArgs.AppendFormat(@" ""%{0}%""", addCmdLineArg);
                    //else
                    additionalCmdLineArgs.AppendFormat(@" ""{0}""", addCmdLineArg);
                }
                string menuCommand = string.Format("\"{0}\" {3}{1}{3} {2} {4}\"%1\"", ChoPath.ToShortFileName(ChoApplication.EntryAssemblyLocation), methodName, additionalCmdLineArgs, SHELL_EXT_CMD_DELIMITER,
                    attr.DefaultArgPrefix);

                menuText = menuText.IsNullOrWhiteSpace() ? methodName : menuText;
                shellKeyName = shellKeyName.IsNullOrWhiteSpace() ? methodName : shellKeyName;
                if (icon.IsNullOrWhiteSpace())
                {
                    if (attr.IconResourceFilePath.IsNullOrWhiteSpace())
                        icon = "{0},{1}".FormatString(ChoPath.ToShortName(ChoApplication.EntryAssemblyLocation), attr.IconIndex);
                    else
                        icon = "{0},{1}".FormatString(ChoPath.ToShortName(attr.IconResourceFilePath), attr.IconIndex);
                }

                ChoShellExtension.Register(fileType, shellKeyName, menuText, menuCommand, icon);
                ChoTrace.WriteLine("Shell Extensions registered successfully.");
            }
        }

        private static IEnumerable<MethodInfo> GetShellExtensionMethods()
        {
            Type[] shellExtTypes = ChoType.GetTypes<ChoShellExtensionAttribute>();
            if (shellExtTypes.IsNullOrEmpty()) yield break;

            foreach (Type type in shellExtTypes)
            {
                foreach (MethodInfo methodInfo in ChoType.GetMethods(type, typeof(ChoShellExtensionContextMenuAttribute)))
                {
                    yield return methodInfo;
                }
            }
        }
    }
}
