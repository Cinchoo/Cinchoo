namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
	using System.Runtime.ConstrainedExecution;
    using Cinchoo.Core.Pattern;

	#endregion NameSpaces

	// Summary:
	//     Represents a Windows NT performance counter component.
	//[InstallerType("System.Diagnostics.PerformanceCounterInstaller,System.Configuration.Install, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [DebuggerDisplay("CategoryName: {CategoryName}, CounterName: {CounterName}, InstanceName: {InstanceName}")]
    public sealed class ChoPerformanceCounter : Component, ISupportInitialize, IDisposable, IChoMetaDataObject<ChoPCMetaDataInfo>
	{
        #region Constants (Private)

        internal const string DefaultInstanceName = "__default__";

        #endregion Constants (Private)

		#region Constants (Private)

		private const string BaseCounterNameSuffix = "Base";

		#endregion Constants (Private)

		#region Instance Data Members (Private)

		private readonly PerformanceCounter _performanceCounter;
		private readonly PerformanceCounter _performanceCounterBase;
		private readonly List<ChoPerformanceCounter> _instancePerformanceCounters = new List<ChoPerformanceCounter>();
        private ChoPCMetaDataInfo _metaDataInfo;

		#endregion Instance Data Members (Private)

		#region Constructors

        //// Summary:
        ////     Initializes a new, read-only instance of the System.Diagnostics.PerformanceCounter
        ////     class, without associating the instance with any system or custom performance
        ////     counter.
        ////
        //// Exceptions:
        ////   System.PlatformNotSupportedException:
        ////     The platform is Windows 98 or Windows Millennium Edition (Me), which does
        ////     not support performance counters.
        //public ChoPerformanceCounter()
        //{
        //    _performanceCounter = new PerformanceCounter();
        //}

		//
		// Summary:
		//     Initializes a new, read-only instance of the System.Diagnostics.PerformanceCounter
		//     class and associates it with the specified system or custom performance counter
		//     on the local computer. This constructor requires that the category have a
		//     single instance.
		//
		// Parameters:
		//   categoryName:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		//   counterName:
		//     The name of the performance counter.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     categoryName is an empty string ("").  -or- counterName is an empty string
		//     ("").  -or- The category specified does not exist. -or- The category specified
		//     is marked as multi-instance and requires the performance counter to be created
		//     with an instance name.  -or- categoryName and counterName have been localized
		//     into different languages.
		//
		//   System.ArgumentNullException:
		//     categoryName or counterName is null.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType)
			: this(categoryName, counterName, counterType, true)
		{
		}

		//
		// Summary:
		//     Initializes a new, read-only or read/write instance of the System.Diagnostics.PerformanceCounter
		//     class and associates it with the specified system or custom performance counter
		//     on the local computer. This constructor requires that the category contain
		//     a single instance.
		//
		// Parameters:
		//   categoryName:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		//   counterName:
		//     The name of the performance counter.
		//
		//   readOnly:
		//     true to access the counter in read-only mode (although the counter itself
		//     could be read/write); false to access the counter in read/write mode.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The categoryName is an empty string ("").  -or- The counterName is an empty
		//     string ("").  -or- The category specified does not exist. (if readOnly is
		//     true). -or- The category specified is not a .NET Framework custom category
		//     (if readOnly is false). -or- The category specified is marked as multi-instance
		//     and requires the performance counter to be created with an instance name.
		//      -or- categoryName and counterName have been localized into different languages.
		//
		//   System.ArgumentNullException:
		//     categoryName or counterName is null.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType, bool readOnly)
			: this(categoryName, counterName, counterType, String.Empty, readOnly)
		{
		}

		//
		// Summary:
		//     Initializes a new, read-only instance of the System.Diagnostics.PerformanceCounter
		//     class and associates it with the specified system or custom performance counter
		//     and category instance on the local computer.
		//
		// Parameters:
		//   categoryName:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		//   counterName:
		//     The name of the performance counter.
		//
		//   instanceName:
		//     The name of the performance counter category instance, or an empty string
		//     (""), if the category contains a single instance.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     categoryName is an empty string ("").  -or- counterName is an empty string
		//     ("").  -or- The category specified is not valid. -or- The category specified
		//     is marked as multi-instance and requires the performance counter to be created
		//     with an instance name.  -or- instanceName is longer than 127 characters.
		//      -or- categoryName and counterName have been localized into different languages.
		//
		//   System.ArgumentNullException:
		//     categoryName or counterName is null.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType, string instanceName)
			: this(categoryName, counterName, counterType, instanceName, true)
		{
		}

		//
		// Summary:
		//     Initializes a new, read-only or read/write instance of the System.Diagnostics.PerformanceCounter
		//     class and associates it with the specified system or custom performance counter
		//     and category instance on the local computer.
		//
		// Parameters:
		//   categoryName:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		//   counterName:
		//     The name of the performance counter.
		//
		//   instanceName:
		//     The name of the performance counter category instance, or an empty string
		//     (""), if the category contains a single instance.
		//
		//   readOnly:
		//     true to access a counter in read-only mode; false to access a counter in
		//     read/write mode.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     categoryName is an empty string ("").  -or- counterName is an empty string
		//     ("").  -or- The read/write permission setting requested is invalid for this
		//     counter.  -or- The category specified does not exist (if readOnly is true).
		//     -or- The category specified is not a .NET Framework custom category (if readOnly
		//     is false). -or- The category specified is marked as multi-instance and requires
		//     the performance counter to be created with an instance name.  -or- instanceName
		//     is longer than 127 characters.  -or- categoryName and counterName have been
		//     localized into different languages.
		//
		//   System.ArgumentNullException:
		//     categoryName or counterName is null.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType, string instanceName, bool readOnly)
            : this(categoryName, counterName, counterType, instanceName, ".", readOnly, PerformanceCounterInstanceLifetime.Global)
		{
		}

		//
		// Summary:
		//     Initializes a new, read-only instance of the System.Diagnostics.PerformanceCounter
		//     class and associates it with the specified system or custom performance counter
		//     and category instance, on the specified computer.
		//
		// Parameters:
		//   categoryName:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		//   counterName:
		//     The name of the performance counter.
		//
		//   instanceName:
		//     The name of the performance counter category instance, or an empty string
		//     (""), if the category contains a single instance.
		//
		//   machineName:
		//     The computer on which the performance counter and its associated category
		//     exist.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     categoryName is an empty string ("").  -or- counterName is an empty string
		//     ("").  -or- The read/write permission setting requested is invalid for this
		//     counter.  -or- The counter does not exist on the specified computer. -or-
		//     The category specified is marked as multi-instance and requires the performance
		//     counter to be created with an instance name.  -or- instanceName is longer
		//     than 127 characters.  -or- categoryName and counterName have been localized
		//     into different languages.
		//
		//   System.ArgumentException:
		//     The machineName parameter is not valid.
		//
		//   System.ArgumentNullException:
		//     categoryName or counterName is null.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType, string instanceName, string machineName)
            : this(categoryName, counterName, counterType, instanceName, machineName, true, PerformanceCounterInstanceLifetime.Global)
		{
		}

        public ChoPerformanceCounter(string categoryName, string counterName, PerformanceCounterType counterType, string instanceName, string machineName, bool readOnly, PerformanceCounterInstanceLifetime instanceLifetime)
		{
            if (instanceName == ChoPerformanceCounter.DefaultInstanceName)
            {
                _performanceCounter = new PerformanceCounter(categoryName, counterName);
                _performanceCounter.MachineName = machineName;
            }
            else
                _performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName, machineName);

			_performanceCounter.ReadOnly = readOnly;
            _performanceCounter.InstanceLifetime = instanceLifetime;

			// create base counter if needed
			switch (counterType)
			{
				case PerformanceCounterType.AverageCount64:
				case PerformanceCounterType.AverageTimer32:
				case PerformanceCounterType.RawFraction:
				case PerformanceCounterType.CounterMultiTimer:
				case PerformanceCounterType.CounterMultiTimerInverse:
				case PerformanceCounterType.CounterMultiTimer100Ns:
				case PerformanceCounterType.CounterMultiTimer100NsInverse:
				case PerformanceCounterType.SampleCounter:
				case PerformanceCounterType.SampleFraction:
					{
                        if (instanceName == ChoPerformanceCounter.DefaultInstanceName)
                        {
                            _performanceCounterBase = new PerformanceCounter(categoryName, ChoPerformanceCounter.GetBaseCounterName(counterName));
                            _performanceCounterBase.MachineName = machineName;
                        }
                        else
                            _performanceCounterBase = new PerformanceCounter(categoryName, ChoPerformanceCounter.GetBaseCounterName(counterName), instanceName, machineName);
                        
						_performanceCounterBase.ReadOnly = ReadOnly;
                        _performanceCounterBase.InstanceLifetime = instanceLifetime;
                        break;
					}
				default:
					break;
			}

            _metaDataInfo = new ChoPCMetaDataInfo(CounterName, instanceName) { TurnOn = true };
            ChoPCMetaDataManager.Me.SetWatcher(this);
        }

		#endregion Constructors

		#region Shared Members (Internal)

		internal static string GetBaseCounterName(string name)
		{
			return String.Format("{0} {1}", name, BaseCounterNameSuffix);
		}

		#endregion Shared Members (Internal)

		#region Instance Properties (Public)

		// Summary:
		//     Gets or sets the name of the performance counter category for this performance
		//     counter.
		//
		// Returns:
		//     The name of the performance counter category (performance object) with which
		//     this performance counter is associated.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The System.Diagnostics.PerformanceCounter.CategoryName is null.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		[ReadOnly(true)]
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.CategoryValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindableAttribute(true)]
		public string CategoryName
		{
			get { return _performanceCounter.CategoryName; }
            //set { _performanceCounter.CategoryName = value; }
		}
		//
		// Summary:
		//     Gets the description for this performance counter.
		//
		// Returns:
		//     A description of the item or quantity that this performance counter measures.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The System.Diagnostics.PerformanceCounter instance is not associated with
		//     a performance counter. -or- The System.Diagnostics.PerformanceCounter.InstanceLifetime
		//     property is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process
		//     when using global shared memory.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		[MonitoringDescription("PC_CounterHelp")]
		[ReadOnly(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string CounterHelp
		{
			get { return _performanceCounter.CounterHelp; }
		}

		//
		// Summary:
		//     Gets or sets the name of the performance counter that is associated with
		//     this System.Diagnostics.PerformanceCounter instance.
		//
		// Returns:
		//     The name of the counter, which generally describes the quantity being counted.
		//     This name is displayed in the list of counters of the Performance Counter
		//     Manager MMC snap in's Add Counters dialog box.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The System.Diagnostics.PerformanceCounter.CounterName is null.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		[TypeConverter("System.Diagnostics.Design.CounterNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[ReadOnly(true)]
		[DefaultValue("")]
		[SettingsBindableAttribute(true)]
		public string CounterName
		{
			get { return _performanceCounter.CounterName; }
            //set { _performanceCounter.CounterName = value; }
		}

		//
		// Summary:
		//     Gets the counter type of the associated performance counter.
		//
		// Returns:
		//     A System.Diagnostics.PerformanceCounterType that describes both how the counter
		//     interacts with a monitoring application and the nature of the values it contains
		//     (for example, calculated or uncalculated).
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The instance is not correctly associated with a performance counter. -or-
		//     The System.Diagnostics.PerformanceCounter.InstanceLifetime property is set
		//     to System.Diagnostics.PerformanceCounterInstanceLifetime.Process when using
		//     global shared memory.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("PC_CounterType")]
		public PerformanceCounterType CounterType
		{
			get { return _performanceCounter.CounterType; }
		}

		//
		// Summary:
		//     Gets or sets the lifetime of a process.
		//
		// Returns:
		//     One of the System.Diagnostics.PerformanceCounterInstanceLifetime values.
		//     The default is System.Diagnostics.PerformanceCounterInstanceLifetime.Global.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The value set is not a member of the System.Diagnostics.PerformanceCounterInstanceLifetime
		//     enumeration.
		//
		//   System.InvalidOperationException:
		//     System.Diagnostics.PerformanceCounter.InstanceLifetime is set after the System.Diagnostics.PerformanceCounter
		//     has been initialized.
		public PerformanceCounterInstanceLifetime InstanceLifetime
		{
			get { return _performanceCounter.InstanceLifetime; }
            //set { _performanceCounter.InstanceLifetime = value; }
		}

		//
		// Summary:
		//     Gets or sets an instance name for this performance counter.
		//
		// Returns:
		//     The name of the performance counter category instance, or an empty string
		//     (""), if the counter is a single-instance counter.
		[TypeConverter("System.Diagnostics.Design.InstanceNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[ReadOnly(true)]
		[DefaultValue("")]
		[SettingsBindableAttribute(true)]
		public string InstanceName
		{
			get { return _performanceCounter.InstanceName; }
            //set { _performanceCounter.InstanceName = value; }
		}

		//
		// Summary:
		//     Gets or sets the computer name for this performance counter
		//
		// Returns:
		//     The server on which the performance counter and its associated category reside.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     The System.Diagnostics.PerformanceCounter.MachineName format is invalid.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		[Browsable(false)]
		[SettingsBindableAttribute(true)]
		public string MachineName
		{
			get { return _performanceCounter.MachineName; }
            //set { _performanceCounter.MachineName = value; }
		}

		//
		// Summary:
		//     Gets or sets the raw, or uncalculated, value of this counter.
		//
		// Returns:
		//     The raw value of the counter.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     You are trying to set the counter's raw value, but the counter is read-only.
		//      -or- The instance is not correctly associated with a performance counter.
		//     -or- The System.Diagnostics.PerformanceCounter.InstanceLifetime property
		//     is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process when
		//     using global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("PC_RawValue")]
		public long RawValue
		{
			get { return _performanceCounter.RawValue; }
			set 
            { 
                if (TurnOn)
                    _performanceCounter.RawValue = value; 
            }
		}

		//
		// Summary:
		//     Gets or sets a value indicating whether this System.Diagnostics.PerformanceCounter
		//     instance is in read-only mode.
		//
		// Returns:
		//     true, if the System.Diagnostics.PerformanceCounter instance is in read-only
		//     mode (even if the counter itself is a custom .NET Framework counter); false
		//     if it is in read/write mode. The default is the value set by the constructor.
		[MonitoringDescription("PC_ReadOnly")]
		[DefaultValue(true)]
		[Browsable(false)]
		public bool ReadOnly
		{
			get { return _performanceCounter.ReadOnly; }
            //set { _performanceCounter.ReadOnly = value; }
		}

		#endregion Instance Properties (Public)

		#region Instance Members (Public)

		// Summary:
		//     Begins the initialization of a System.Diagnostics.PerformanceCounter instance
		//     used on a form or by another component. The initialization occurs at runtime.
		public void BeginInit()
		{
			_performanceCounter.BeginInit();
			if (_performanceCounterBase != null)
				_performanceCounterBase.BeginInit();
		}

		//
		// Summary:
		//     Closes the performance counter and frees all the resources allocated by this
		//     performance counter instance.
		public void Close()
		{
			_performanceCounter.Close();
			if (_performanceCounterBase != null)
				_performanceCounterBase.Close();
		}

		//
		// Summary:
		//     Frees the performance counter library shared state allocated by the counters.
		public static void CloseSharedResources()
		{
			PerformanceCounter.CloseSharedResources();
		}

		//
		// Summary:
		//     Decrements the associated performance counter by one through an efficient
		//     atomic operation.
		//
		// Returns:
		//     The decremented counter value.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The counter is read-only, so the application cannot decrement it.  -or- The
		//     instance is not correctly associated with a performance counter. -or- The
		//     System.Diagnostics.PerformanceCounter.InstanceLifetime property is set to
		//     System.Diagnostics.PerformanceCounterInstanceLifetime.Process when using
		//     global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		public long Decrement()
		{
            if (TurnOn)
            {
                long retVal = _performanceCounter.Decrement();
                if (_performanceCounterBase != null)
                    _performanceCounterBase.Decrement();

                return retVal;
            }
            else
                return 0;
		}

		//		protected override void Dispose(bool disposing);

		//
		// Summary:
		//     Ends the initialization of a System.Diagnostics.PerformanceCounter instance
		//     that is used on a form or by another component. The initialization occurs
		//     at runtime.
		public void EndInit()
		{
			_performanceCounter.EndInit();
			if (_performanceCounterBase != null)
				_performanceCounterBase.EndInit();
		}

		//
		// Summary:
		//     Increments the associated performance counter by one through an efficient
		//     atomic operation.
		//
		// Returns:
		//     The incremented counter value.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The counter is read-only, so the application cannot increment it.  -or- The
		//     instance is not correctly associated with a performance counter. -or- The
		//     System.Diagnostics.PerformanceCounter.InstanceLifetime property is set to
		//     System.Diagnostics.PerformanceCounterInstanceLifetime.Process when using
		//     global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		public long Increment()
		{
            if (TurnOn)
            {
                long retVal = _performanceCounter.Increment();
                if (_performanceCounterBase != null)
                    _performanceCounterBase.Increment();

                return retVal;
            }
            else
                return 0;
		}

		//
		// Summary:
		//     Increments or decrements the value of the associated performance counter
		//     by a specified amount through an efficient atomic operation.
		//
		// Parameters:
		//   value:
		//     The value to increment by. (A negative value decrements the counter.)
		//
		// Returns:
		//     The new counter value.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The counter is read-only, so the application cannot increment it.  -or- The
		//     instance is not correctly associated with a performance counter. -or- The
		//     System.Diagnostics.PerformanceCounter.InstanceLifetime property is set to
		//     System.Diagnostics.PerformanceCounterInstanceLifetime.Process when using
		//     global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public long IncrementBy(long value)
		{
            if (TurnOn)
            {
                long retVal = _performanceCounter.IncrementBy(value);
                if (_performanceCounterBase != null)
                    _performanceCounterBase.Increment();

                return retVal;
            }
            else
                return 0;
		}

		//
		// Summary:
		//     Obtains a counter sample, and returns the raw, or uncalculated, value for
		//     it.
		//
		// Returns:
		//     A System.Diagnostics.CounterSample that represents the next raw value that
		//     the system obtains for this counter.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The instance is not correctly associated with a performance counter. -or-
		//     The System.Diagnostics.PerformanceCounter.InstanceLifetime property is set
		//     to System.Diagnostics.PerformanceCounterInstanceLifetime.Process when using
		//     global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public CounterSample NextSample()
		{
			return _performanceCounter.NextSample();
		}

		//
		// Summary:
		//     Obtains a counter sample and returns the calculated value for it.
		//
		// Returns:
		//     The next calculated value that the system obtains for this counter.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The instance is not correctly associated with a performance counter.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		//
		//   System.UnauthorizedAccessException:
		//     Code that is executing without administrative privileges attempted to read
		//     a performance counter.
		public float NextValue()
		{
			return _performanceCounter.NextValue();
		}

		//
		// Summary:
		//     Deletes the category instance specified by the System.Diagnostics.PerformanceCounter
		//     object System.Diagnostics.PerformanceCounter.InstanceName property.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     This counter is read-only, so any instance that is associated with the category
		//     cannot be removed.  -or- The instance is not correctly associated with a
		//     performance counter. -or- The System.Diagnostics.PerformanceCounter.InstanceLifetime
		//     property is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process
		//     when using global shared memory.
		//
		//   System.ComponentModel.Win32Exception:
		//     An error occurred when accessing a system API.
		//
		//   System.PlatformNotSupportedException:
		//     The platform is Windows 98 or Windows Millennium Edition (Me), which does
		//     not support performance counters.
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public void RemoveInstance()
		{
			_performanceCounter.RemoveInstance();
			if (_performanceCounterBase != null)
				_performanceCounterBase.RemoveInstance();
		}

		#endregion Instance Members (Public)

		#region Factory Methods

		public ChoPerformanceCounter NewInstanceCounter(string instanceName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(instanceName, "InstanceName");
			ChoPerformanceCounter instancePerformanceCounter = new ChoPerformanceCounter(CategoryName, CounterName, CounterType, instanceName, ReadOnly);

			_instancePerformanceCounters.Add(instancePerformanceCounter);
			return instancePerformanceCounter;
		}

		#endregion Factory Methods

		#region Destructors

		~ChoPerformanceCounter()
		{
			Dispose();
		}

		#endregion Destructors

		#region IDisposable Members

		public new void Dispose()
		{
			if (_performanceCounterBase != null)
				_performanceCounterBase.Dispose();
			if (_performanceCounter != null)
				_performanceCounter.Dispose();

			foreach (ChoPerformanceCounter pc in _instancePerformanceCounters)
			{
				if (pc != null)
					pc.Dispose();
			}

			_instancePerformanceCounters.Clear();
		}

		#endregion

        #region Object Overloads

        public override int GetHashCode()
        {
            return CategoryName.GetHashCode() ^ CounterName.GetHashCode();
        }

        #endregion Object Overloads

        public bool TurnOn
        {
            get;
            private set;
        }

        #region IChoMetaDataObject<ChoPCMetaDataInfo> Members

        public void SetMetaData(ChoPCMetaDataInfo metaDataInfo)
        {
            if (metaDataInfo == null)
                return;

            _metaDataInfo = metaDataInfo;
            TurnOn = metaDataInfo.TurnOn;
        }

        public string NodeLocateXPath
        {
            get
            {
                if (InstanceName.IsNullOrEmpty())
                    return "//PerformanceCounterCategory[@name='{0}']/PerformanceCounter[@name='{1}']".FormatString(CategoryName, CounterName);
                else
                    return "//PerformanceCounterCategory[@name='{0}']/PerformanceCounter[@name='{1}' and @instanceName='{2}']".FormatString(CategoryName, CounterName, InstanceName);
            }
        }

        public string NodeCreateXPath
        {
            get
            {
                if (InstanceName.IsNullOrEmpty())
                    return "//PerformanceCounterCategory[@name='{0}' and @selfInstall='true' and @recreate='true']/PerformanceCounter[@name='{1}']".FormatString(CategoryName, CounterName);
                else
                    return "//PerformanceCounterCategory[@name='{0}' and @selfInstall='true' and @recreate='true']/PerformanceCounter[@name='{1}' and @instanceName='{2}']".FormatString(CategoryName, CounterName, InstanceName);
            }
        }

        public ChoPCMetaDataInfo MetaDataInfo
        {
            get { return _metaDataInfo; }
        }

        public string Name
        {
            get
            {
                return "{0}/{1}/{2}".FormatString(CategoryName, CounterName, InstanceName);
            }
        }

        #endregion
    }
}
