namespace Cinchoo.Core.Win32
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Management;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.ComponentModel;
	using System.Threading;
	using System.IO;
	using Microsoft.Win32;
	using System.Reflection;

	using Cinchoo.Core;
	using Cinchoo.Core.Win32;

	#endregion NameSpaces

	[DebuggerDisplay("Value = {ToString()}")]
	public class ChoRegistryChangeMonitor : ChoSyncDisposableObject
	{
		#region P/Invoke

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int samDesired, out IntPtr phkResult);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegNotifyChangeKeyValue(IntPtr hKey, bool bWatchSubtree, ChoRegistryChangeNotifyFilter dwNotifyFilter, IntPtr hEvent, bool fAsynchronous);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegCloseKey(IntPtr hKey);

		private const int KEY_QUERY_VALUE = 0x0001;
		private const int KEY_NOTIFY = 0x0010;
		private const int STANDARD_RIGHTS_READ = 0x00020000;

		[Description("HKEY_CLASSES_ROOT")]
		private static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(unchecked((int)0x80000000));
		[Description("HKEY_CURRENT_USER")]
		private static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(unchecked((int)0x80000001));
		[Description("HKEY_LOCAL_MACHINE")]
		private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(unchecked((int)0x80000002));
		[Description("HKEY_USERS")]
		private static readonly IntPtr HKEY_USERS = new IntPtr(unchecked((int)0x80000003));
		[Description("HKEY_PERFORMANCE_DATA")]
		private static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(unchecked((int)0x80000004));
		[Description("HKEY_CURRENT_CONFIG")]
		private static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(unchecked((int)0x80000005));
		[Description("HKEY_DYN_DATA")]
		private static readonly IntPtr HKEY_DYN_DATA = new IntPtr(unchecked((int)0x80000006));

		#endregion P/Invoke

		#region Instance Data Members (Private)

		private object _padLock = new object();
		private IntPtr _registryHivePtr;
		private string _keyPath;
		private string _fullyQualifiedRegistryKey;
		private ManualResetEvent _eventTerminate = new ManualResetEvent(false);
		private Thread _thread;
		private ChoRegistryChangeNotifyFilter _regFilter = ChoRegistryChangeNotifyFilter.Key | ChoRegistryChangeNotifyFilter.Attribute |
			ChoRegistryChangeNotifyFilter.Value | ChoRegistryChangeNotifyFilter.Security;

		#endregion Instance Data Members (Private)

		#region Events

		/// <summary>
		/// Occurs when the registry changed
		/// </summary>
		public event EventHandler RegistryChanged;

		/// <summary>
		/// Occurs when the access to the registry fails.
		/// </summary>
		public event ErrorEventHandler Error;


		#endregion Events

		#region Constructors

		public ChoRegistryChangeMonitor(string registryKeyString)
			: this(registryKeyString, false)
		{
		}

		public ChoRegistryChangeMonitor(string registryKeyString, bool autoStart)
		{
			Init(registryKeyString);
			_fullyQualifiedRegistryKey = registryKeyString;

			if (autoStart)
				Start();
		}

		public ChoRegistryChangeMonitor(RegistryHive registryHive, string keyPath)
			: this(registryHive, keyPath, false)
		{
		}

		public ChoRegistryChangeMonitor(RegistryHive registryHive, string keyPath, bool autoStart)
		{
			ChoGuard.ArgumentNotNullOrEmpty(keyPath, "KeyPath");

			Init(registryHive);
			_fullyQualifiedRegistryKey = String.Format(@"{0}\{1}", registryHive.ToDescription(), _keyPath);

			if (autoStart)
				Start();
		}

		#endregion Constructors

		#region Instance Members (Public)

		public bool IsStarted
		{
			get { return _thread != null; }
		}

		public void Start()
		{
			ChoGuard.NotDisposed(this);

			if (IsStarted)
				return;

			lock (_padLock)
			{
				if (IsStarted)
					return;

				_eventTerminate.Reset();
				_thread = new Thread(new ThreadStart(MonitorThread));
				_thread.IsBackground = true;
				_thread.Start();
			}
		}

		public void Stop()
		{
			ChoGuard.NotDisposed(this);

			lock (_padLock)
			{
				Thread thread = _thread;
				if (thread != null)
				{
					_eventTerminate.Set();
					thread.Join();
				}
			}
		}

		#endregion Instance Members (Public)

		#region Instance Members (Private)

		private void MonitorThread()
		{
			try
			{
				ThreadLoop();
			}
			catch (Exception e)
			{
				OnError(e);
			}
			_thread = null;
		}

		private void ThreadLoop()
		{
			IntPtr registryKey;
			int result = RegOpenKeyEx(_registryHivePtr, _keyPath, 0, STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_NOTIFY, out registryKey);
			if (result != 0)
				throw new ChoWin32Exception(result, "Can't find '{0}' registry key.".FormatString(_fullyQualifiedRegistryKey));

			try
			{
				AutoResetEvent _eventNotify = new AutoResetEvent(false);
				WaitHandle[] waitHandles = new WaitHandle[] {_eventNotify, _eventTerminate};
				while (!_eventTerminate.WaitOne(0, true))
				{
					result = RegNotifyChangeKeyValue(registryKey, true, _regFilter, _eventNotify.SafeWaitHandle.DangerousGetHandle(), true);

					if (result != 0)
						throw new ChoWin32Exception(result, "Error getting notification for '{0}' registry key.".FormatString(_fullyQualifiedRegistryKey));

					if (WaitHandle.WaitAny(waitHandles) == 0)
						RegistryChanged.Raise(this, null);
				}
			}
			finally
			{
				if (registryKey != IntPtr.Zero)
					RegCloseKey(registryKey);
			}
		}

		/// <summary>
		/// Raises the <see cref="Error"/> event.
		/// </summary>
		/// <param name="e">The <see cref="Exception"/> which occured while watching the registry.</param>
		/// <remarks>
		/// <p>
		/// <b>OnError</b> is called when an exception occurs while watching the registry.
		/// </p>
		/// <note type="inheritinfo">
		/// When overriding <see cref="OnError"/> in a derived class, be sure to call
		/// the base class's <see cref="OnError"/> method.
		/// </note>
		/// </remarks>
		protected virtual void OnError(Exception e)
		{
			ErrorEventHandler handler = Error;
			if (handler != null)
				handler(this, new ErrorEventArgs(e));
		}

		private void Init(RegistryHive registryHive)
		{
			switch (registryHive)
			{
				case RegistryHive.ClassesRoot:
					_registryHivePtr = HKEY_CLASSES_ROOT;
					break;
				case RegistryHive.CurrentConfig:
					_registryHivePtr = HKEY_CURRENT_CONFIG;
					break;
				case RegistryHive.CurrentUser:
					_registryHivePtr = HKEY_CURRENT_USER;
					break;
				case RegistryHive.DynData:
					_registryHivePtr = HKEY_DYN_DATA;
					break;
				case RegistryHive.LocalMachine:
					_registryHivePtr = HKEY_LOCAL_MACHINE;
					break;
				case RegistryHive.PerformanceData:
					_registryHivePtr = HKEY_PERFORMANCE_DATA;
					break;
				case RegistryHive.Users:
					_registryHivePtr = HKEY_USERS;
					break;
				default:
					throw new InvalidEnumArgumentException("registryHive", (int)registryHive, typeof(RegistryHive));
			}
		}

		private void Init(string registryKeyString)
		{
			string[] nameParts = registryKeyString.Split('\\');

			switch (nameParts[0])
			{
				case "HKEY_CLASSES_ROOT":
				case "HKCR":
					_registryHivePtr = HKEY_CLASSES_ROOT;
					break;
				case "HKEY_CURRENT_USER":
				case "HKCU":
					_registryHivePtr = HKEY_CURRENT_USER;
					break;
				case "HKEY_LOCAL_MACHINE":
				case "HKLM":
					_registryHivePtr = HKEY_LOCAL_MACHINE;
					break;
				case "HKEY_USERS":
					_registryHivePtr = HKEY_USERS;
					break;
				case "HKEY_CURRENT_CONFIG":
					_registryHivePtr = HKEY_CURRENT_CONFIG;
					break;
				default:
					_registryHivePtr = IntPtr.Zero;
					throw new ArgumentException("The registry hive '" + nameParts[0] + "' is not supported", "value");
			}

			_keyPath = String.Join("\\", nameParts, 1, nameParts.Length - 1);
		}
		#endregion Instance Members (Private)

		#region Disposable Members

		protected override void Dispose(bool finalize)
		{
			Stop();
		}

		#endregion Disposable Members

		#region Object Overrides

		public override string ToString()
		{
			return _fullyQualifiedRegistryKey;
		}

		#endregion Object Overrides
	}

	#region ChoRegistryChangeNotifyFilter Enum

	/// <summary>
	/// Filter for notifications reported by <see cref="RegistryMonitor"/>.
	/// </summary>
	[Flags]
	public enum ChoRegistryChangeNotifyFilter
	{
		/// <summary>Notify the caller if a subkey is added or deleted.</summary>
		Key = 1,
		/// <summary>Notify the caller of changes to the attributes of the key,
		/// such as the security descriptor information.</summary>
		Attribute = 2,
		/// <summary>Notify the caller of changes to a value of the key. This can
		/// include adding or deleting a value, or changing an existing value.</summary>
		Value = 4,
		/// <summary>Notify the caller of changes to the security descriptor
		/// of the key.</summary>
		Security = 8,
	}

	#endregion ChoRegistryChangeNotifyFilter Enum
}
