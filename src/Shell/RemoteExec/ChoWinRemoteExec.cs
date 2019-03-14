namespace Cinchoo.Core.Shell
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Cinchoo.Core.Net;
	using System.Net;
	using System.IO;
	using System.Management;

	#endregion

	public class ChoWinRemoteExec : IDisposable
	{
		#region Instance Data Members (Private)

		private readonly string _remoteMachine;
		private readonly NetworkCredential _networkCredential;

		#endregion

		#region Constructors

		public ChoWinRemoteExec(string remoteMachine, NetworkCredential networkCredential)
		{
			ChoGuard.ArgumentNotNullOrEmpty(remoteMachine, "RemoteMachine");
			ChoGuard.ArgumentNotNull(networkCredential, "NetworkCredential");

			_remoteMachine = remoteMachine;
			_networkCredential = networkCredential;
		}

		#endregion

		#region Instnce Members (Public)

		public ChoWinRemoteExecState RunCmd(string cmd)
		{
			ChoGuard.ArgumentNotNullOrEmpty(cmd, "Command");

			return RunCmd(new string[] { cmd });
		}

		public ChoWinRemoteExecState RunCmd(string[] cmds)
		{
			ChoGuard.ArgumentNotNullOrEmpty(cmds, "Commands");

			ChoWinRemoteExecState winRemoteExecState = new ChoWinRemoteExecState(_remoteMachine, _networkCredential);
			using (ChoNetworkShare networkShare = new ChoNetworkShare(winRemoteExecState.RemoteShareName, _networkCredential))
			{
				CreateScriptOnRemote(cmds, winRemoteExecState);
				RunSriptOnRemote(winRemoteExecState);
				return winRemoteExecState;
			}
		}

		#endregion

		#region Instance Members (Private)

		private void RunSriptOnRemote(ChoWinRemoteExecState winRemoteExecState)
		{
			ConnectionOptions connOptions = new ConnectionOptions();
			connOptions.Username = @"{0}\{1}".FormatString(_networkCredential.Domain, _networkCredential.UserName);
			connOptions.Password = _networkCredential.Password;
			//connOptions.Impersonation = ImpersonationLevel.Impersonate;
			connOptions.EnablePrivileges = true;
			
			ManagementScope manScope = new ManagementScope(@"\\{0}\ROOT\CIMV2".FormatString(_remoteMachine), connOptions);
			manScope.Connect();

			ObjectGetOptions objectGetOptions = new ObjectGetOptions();
			ManagementPath managementPath = new ManagementPath("Win32_Process");

			ManagementClass processClass = new ManagementClass(manScope, managementPath, objectGetOptions);
			ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
			inParams["CommandLine"] = winRemoteExecState.RemoteScriptFilePath; 

			ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);

			if (outParams != null)
			{
				winRemoteExecState.ProcessId = (uint)outParams["processId"];
				winRemoteExecState.ReturnValue = (uint)outParams["returnValue"];
			}
		}

		private void CreateScriptOnRemote(string[] cmds, ChoWinRemoteExecState winRemoteExecState)
		{
			winRemoteExecState.Clean();

			using (StreamWriter sw = new StreamWriter(winRemoteExecState.RemoteScriptFilePath))
				sw.WriteLine("@CALL {0} 1> {1} 2> {2}".FormatString(winRemoteExecState.RemoteScriptInnerFilePath,
					winRemoteExecState.RemoteScriptOutputFilePath, winRemoteExecState.RemoteScriptErrorFilePath));

			using (StreamWriter sw = new StreamWriter(winRemoteExecState.RemoteScriptInnerFilePath))
			{
				foreach (string cmd in cmds)
				{
					if (cmd.IsNullOrWhiteSpace()) continue;
					if (String.Compare(cmd, "cls", true) == 0) continue;

					sw.WriteLine(cmd);
				}
			}
		}

		#endregion
		
		#region IDisposable Overrides

		public void Dispose()
		{
		}

		#endregion
	}
}
