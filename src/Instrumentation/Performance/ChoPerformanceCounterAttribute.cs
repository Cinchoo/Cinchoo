namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Diagnostics;
	using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	/// <summary>
	/// Defines a performance counter for the class that it is tagged with it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ChoPerformanceCounterAttribute : Attribute
	{
		#region Constructor

		/// <summary>
		/// Creates a new performance counter attribute with the given counter name.
		/// </summary>
		/// <remarks>
		/// Creates a new performance counter attribute with the given counter name.
		/// </remarks>
		/// <param name="name">Counter name.</param>
		public ChoPerformanceCounterAttribute(string counterName)
			: this(counterName, null, PerformanceCounterType.NumberOfItems64, null)
		{
			// nothing to do
		}

		/// <summary>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </summary>
		/// <remarks>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </remarks>
		/// <param name="name">Counter name.</param>
		/// <param name="type">Counter type.</param>
		public ChoPerformanceCounterAttribute(string counterName, PerformanceCounterType counterType)
			: this(counterName, null, counterType, null)
		{
			// nothing to do
		}

		/// <summary>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </summary>
		/// <remarks>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </remarks>
		/// <param name="name">Counter name.</param>
		/// <param name="type">Counter type.</param>
		/// <param name="instance">Counter instance name.</param>
		public ChoPerformanceCounterAttribute(string counterName, PerformanceCounterType counterType, string counterInstance)
			: this(counterName, null, counterType, counterInstance)
		{
			// nothing to do
		}

		/// <summary>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </summary>
		/// <remarks>
		/// Creates a new performance counter attribute with the given counter name and type.
		/// </remarks>
		/// <param name="name">Counter name.</param>
		/// <param name="help">Counter help.</param>
		/// <param name="type">Counter type.</param>
		public ChoPerformanceCounterAttribute(string counterName, string counterHelp, PerformanceCounterType counterType)
			: this(counterName, counterHelp, counterType, null)
		{
			// nothing to do
		}

		/// <summary>
		/// Creates a new performance counter attribute with the given counter name, help, member name and type.
		/// </summary>
		/// <remarks>
		/// Creates a new performance counter attribute with the given counter name, help, member name and type.
		/// </remarks>
		/// <param name="name">Counter name.</param>
		/// <param name="type">Counter type.</param>
		/// <param name="help">Counter help.</param>
		/// <param name="instance">Counter instance name.</param>
		public ChoPerformanceCounterAttribute(string counterName, string counterHelp, PerformanceCounterType counterType, string counterInstance)
			: this(counterName, counterHelp, counterType, counterInstance, null)
		{
		}

		public ChoPerformanceCounterAttribute(string counterName, string counterHelp, PerformanceCounterType counterType, string counterInstance, string machineName)
		{
			ChoGuard.ArgumentNotNullOrEmpty(counterName, "counterName");
			if (counterHelp.IsNullOrEmpty())
				counterHelp = counterName;

			CounterName = counterName;
			CounterInstanceName = counterInstance;
			CounterHelp = counterHelp;
			CounterType = counterType;
			MachineName = machineName;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the performance counter name.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter name.
		/// </remarks>
		public string CounterName { get; private set; }

		/// <summary>
		/// Gets the performance counter instance name.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter instance name.
		/// </remarks>
		public string CounterInstanceName { get; private set; }

		/// <summary>
		/// Gets the performance counter help.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter help.
		/// </remarks>
		public string CounterHelp { get; private set; }

		/// <summary>
		/// Gets the type of the performance counter.
		/// </summary>
		/// <remarks>
		/// Gets the type of the performance counter.
		/// </remarks>
		public PerformanceCounterType CounterType { get; private set; }

		/// <summary>
		/// Gets the performance counter name.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter name.
		/// </remarks>
		public string MachineName { get; private set; }

		#endregion

		#region Instance Members (Internal)

		internal CounterCreationData[] CreateCounters()
		{
			List<CounterCreationData> counters = new List<CounterCreationData>();

			if (MachineName.IsNullOrEmpty() || String.Compare(MachineName, Environment.MachineName, true) == 0)
			{
				counters.Add(new CounterCreationData(CounterName, CounterHelp, CounterType));

				// create base counter if needed
				switch (CounterType)
				{
					case PerformanceCounterType.AverageCount64:
					case PerformanceCounterType.AverageTimer32:
						counters.Add(new CounterCreationData(ChoPerformanceCounter.GetBaseCounterName(CounterName), String.Empty, PerformanceCounterType.AverageBase));
						break;
					case PerformanceCounterType.RawFraction:
						counters.Add(new CounterCreationData(ChoPerformanceCounter.GetBaseCounterName(CounterName), String.Empty, PerformanceCounterType.RawBase));
						break;
					case PerformanceCounterType.CounterMultiTimer:
					case PerformanceCounterType.CounterMultiTimerInverse:
					case PerformanceCounterType.CounterMultiTimer100Ns:
					case PerformanceCounterType.CounterMultiTimer100NsInverse:
						counters.Add(new CounterCreationData(ChoPerformanceCounter.GetBaseCounterName(CounterName), String.Empty, PerformanceCounterType.CounterMultiBase));
						break;
					case PerformanceCounterType.SampleCounter:
					case PerformanceCounterType.SampleFraction:
						counters.Add(new CounterCreationData(ChoPerformanceCounter.GetBaseCounterName(CounterName), String.Empty, PerformanceCounterType.SampleBase));
						break;
					default:
						break;
				}
			}

			return counters.ToArray();
		}

		#endregion Instance Members (Internal)
	}
}
