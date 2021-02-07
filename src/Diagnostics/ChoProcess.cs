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

namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management;
    using System.IO;
    using System.Security;
    using System.Threading.Tasks;

    #endregion NameSpaces

    public class ChoProcessResult
    {
        public string StdOut
        {
            get;
            internal set;
        }

        public string StdErr
        {
            get;
            internal set;
        }

        public int ExitCode
        {
            get;
            internal set;
        }

        public Process Process
        {
            get;
            internal set;
        }
        public Task Task
        {
            get;
            internal set;
        }

        public void Kill()
        {
            if (Process != null)
                Process.Kill();
        }
    }

    public class ChoProcess : ChoSyncDisposableObject
    {
        #region Instance Data Members (Private)

        public readonly string FileName;
        public readonly string Arguments;
        public string UserName;
        public SecureString Password;
        public string Domain;
        public string WorkingDirectory;
        public int Timeout;
        public bool UseShellExecute;
        public string Verb;

        private System.Diagnostics.Process _process;
        private StringBuilder _stdout = new StringBuilder();
        private StringBuilder _stderr = new StringBuilder();

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoProcess(string fileName, string arguments = null)
        {
            ChoGuard.ArgumentNotNullOrEmpty(fileName, "FileName");

            FileName = fileName;
            Arguments = arguments;
        }

        #endregion Constructors

        #region Instance Members (Public)

        public ChoProcessResult ExecuteAsync()
        {
            ChoProcessResult result = new ChoProcessResult();

            Task task = Task.Factory.StartNew((r) =>
                {
                    Execute(r as ChoProcessResult);
                }, result);

            result.Task = task;

            return result;
        }

        public ChoProcessResult Execute()
        {
            ChoProcessResult result = new ChoProcessResult();
            Execute(result);
            return result;
        }

        public ChoProcessResult Execute(ProcessStartInfo processStartInfo)
        {
            ChoProcessResult result = new ChoProcessResult();
            Execute(processStartInfo, result);
            return result;
        }

        #endregion Instance Members (Public)

        #region Events

        public event EventHandler<DataReceivedEventArgs> OutputDataReceived;
        public event EventHandler<DataReceivedEventArgs> ErrorDataReceived;

        #endregion Events

        #region Instance Members (Private)

        private void Execute(ChoProcessResult result)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.UseShellExecute = UseShellExecute;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.Arguments = Arguments;
            processStartInfo.FileName = FileName;
            if (!Verb.IsNullOrWhiteSpace())
                processStartInfo.Verb = Verb;
            processStartInfo.RedirectStandardOutput = !UseShellExecute;
            processStartInfo.RedirectStandardError = !UseShellExecute;

            if (!WorkingDirectory.IsNullOrWhiteSpace())
                processStartInfo.WorkingDirectory = WorkingDirectory;
            else if (!Path.GetDirectoryName(FileName).IsNullOrWhiteSpace())
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(FileName);

            if (!Domain.IsNullOrWhiteSpace())
                processStartInfo.Domain = Domain;

            if (!UserName.IsNullOrWhiteSpace())
                processStartInfo.UserName = UserName;

            if (Password != null)
                processStartInfo.Password = Password;

            Execute(processStartInfo, result);
        }

        private void Execute(ProcessStartInfo processStartInfo, ChoProcessResult result)
        {
            _process = result.Process = new System.Diagnostics.Process();
            _process.StartInfo = processStartInfo;
            if (processStartInfo.RedirectStandardOutput)
                _process.OutputDataReceived += (sender, e) => { RaiseOutputDataReceived(e); };
            if (processStartInfo.RedirectStandardError)
                _process.ErrorDataReceived += (sender, e) => { RaiseErrorDataReceived(e); };

            _process.EnableRaisingEvents = true;
            _process.Start();

            if (processStartInfo.RedirectStandardOutput)
                _process.BeginOutputReadLine();
            if (processStartInfo.RedirectStandardError)
                _process.BeginErrorReadLine();

            if (Timeout > 0)
            {
                if (!_process.WaitForExit(Timeout))
                    throw new ChoTimeoutException("Process timedout [{0}ms].".FormatString(Timeout));
            }
            else
                _process.WaitForExit();

            result.ExitCode = _process.ExitCode;
            result.StdOut = _stdout.ToString();
            result.StdErr = _stderr.ToString();

            if (result.ExitCode != 0)
                throw new ChoProcessException("Process exited with '{0}' error code.".FormatString(result.ExitCode));
            else if (!result.StdErr.IsNullOrWhiteSpace())
                throw new ChoProcessException(result.StdErr);
        }

        #endregion Instance Members (Private)

        #region Other members (protected)

        private void RaiseOutputDataReceived(DataReceivedEventArgs e)
        {
            if (e == null) return;

            OutputDataReceived.Raise(this, e);
            OnOutputDataReceived(e.Data);
        }

        protected virtual void OnOutputDataReceived(string data)
        {
            if (data != null)
                _stdout.Append(data);
        }

        private void RaiseErrorDataReceived(DataReceivedEventArgs e)
        {
            if (e == null) return;

            ErrorDataReceived.Raise(this, e);
            OnErrorDataReceived(e.Data);
        }

        protected virtual void OnErrorDataReceived(string data)
        {
            if (data != null)
                _stderr.Append(data);
        }

        #endregion

        #region Shared Members (Public)

        public static bool Kill(int pid)
        {
            bool killAnyProcess = false;
            Process[] procs = Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                killAnyProcess = GetParentProcess(procs[i].Id) == pid && Kill(procs[i].Id);
            }
         
            Process myProc = Process.GetProcessById(pid);
            if (myProc != null)
            {
                myProc.Kill();
                return true;
            }
            return killAnyProcess;
        }

        private static int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
            {
                mo.Get();
                parentPid = Convert.ToInt32(mo["ParentProcessId"]);
            }
            return parentPid;
        }

        #endregion Shared Members (Public)

        #region IDisposable Overrides

        protected override void Dispose(bool finalize)
        {
        }

        #endregion IDisposable Overrides
    }
}
