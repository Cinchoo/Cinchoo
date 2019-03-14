namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Cinchoo.Core.Services;
	using System.Diagnostics;
	using Cinchoo.Core.Pattern;
	using System.Reflection;
	using System.Configuration.Install;
	using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	public class ChoPerformanceCounterInstallerBuilder
	{
		#region Instance Data Members (Private)

		private ChoPerformanceCounterCategoryAttribute _performanceCounterCategoryAttribute = null;
		private readonly Dictionary<string, CounterCreationData> _counterCreationData = new Dictionary<string, CounterCreationData>();

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoPerformanceCounterInstallerBuilder(Type reflectedType) : this(new Type[] { reflectedType })
		{
		}

		public ChoPerformanceCounterInstallerBuilder(Type[] reflectedTypes)
		{
			AddCounters(reflectedTypes, false);
		}

		#endregion Constructors

		#region Instance Members (Public)

		public void AddCounters(Type reflectedType)
		{
			ChoGuard.ArgumentNotNull(reflectedType, "reflectedType");
			AddCounters(new Type[] { reflectedType });
		}
		
		public void AddCounters(Type[] reflectedTypes)
		{
			AddCounters(reflectedTypes, true);
		}

		public PerformanceCounterInstaller CreateInstaller()
		{
			PerformanceCounterInstaller performanceCounterInstaller = new System.Diagnostics.PerformanceCounterInstaller();
			performanceCounterInstaller.CategoryName = _performanceCounterCategoryAttribute.CategoryName;
			performanceCounterInstaller.CategoryType = _performanceCounterCategoryAttribute.CategoryType;
			performanceCounterInstaller.CategoryHelp = _performanceCounterCategoryAttribute.CaregoryHelp;
			performanceCounterInstaller.Counters.AddRange(_counterCreationData.Values.ToArray());

			return performanceCounterInstaller;
		}

		public void Fill(Installer installer)
		{
			ChoGuard.ArgumentNotNull(installer, "installer");

			installer.Installers.Add(CreateInstaller());
		}

		/// <summary>
		/// Adds a performance counter of the given type to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a performance counter of the given type to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="type">Performance counter type.</param>
		/// <param name="help">Performance counter help text.</param>
		public bool AddCounter(string name, PerformanceCounterType type, string help)
		{
			if (_counterCreationData.ContainsKey(name))
				return false;

			CounterCreationData counter = new CounterCreationData();
			counter.CounterName = name;
			counter.CounterType = type;
			if (help.IsNullOrEmpty()) help = name;
			counter.CounterHelp = help;

			_counterCreationData.Add(name, counter);

			return true;
		}

		/// <summary>
		/// Adds a performance counter of the given type to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a performance counter of the given type to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="type">Performance counter type.</param>
		public bool AddCounter(string name, PerformanceCounterType type)
		{
			return AddCounter(name, type, null);
		}

		/// <summary>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public bool AddNumberCounter(string name, string help)
		{
			return AddCounter(name, PerformanceCounterType.NumberOfItems64, help);
		}

		/// <summary>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public bool AddNumberCounter(string name)
		{
			return AddCounter(name, PerformanceCounterType.NumberOfItems64, null);
		}

		/// <summary>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public bool AddRateCounter(string name, string help)
		{
			return AddCounter(name, PerformanceCounterType.RateOfCountsPerSecond64, help);
		}

		/// <summary>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public bool AddRateCounter(string name)
		{
			return AddCounter(name, PerformanceCounterType.RateOfCountsPerSecond64);
		}

		/// <summary>
		/// Adds a AverageCount64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public bool AddAverageCounter(string name, string help)
		{
			// add average counter
			bool retValue = AddCounter(name, PerformanceCounterType.AverageCount64, help);
			// add the corresponding base counter
			if (retValue)
				AddCounter(ChoPerformanceCounter.GetBaseCounterName(name), PerformanceCounterType.AverageBase, null);

			return retValue;
		}

		/// <summary>
		/// Adds a AverageCount64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public bool AddAverageCounter(string name)
		{
			return AddAverageCounter(name, null);
		}

		/// <summary>
		/// Adds a RawFraction performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public bool AddFractionCounter(string name, string help)
		{
			// add average counter
			bool retValue = AddCounter(name, PerformanceCounterType.RawFraction, help);
			// add the corresponding base counter
			if (retValue)
				AddCounter(ChoPerformanceCounter.GetBaseCounterName(name), PerformanceCounterType.RawBase, null);

			return retValue;
		}

		/// <summary>
		/// Adds a RawFraction performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public bool AddFractionCounter(string name)
		{
			return AddFractionCounter(name, null);
		}

		/// <summary>
		/// Indicates if the performance counter category exists.
		/// </summary>
		/// <remarks>
		/// Indicates if the performance counter category exists.
		/// </remarks>
		/// <returns>True if it does exist, false otherwise.</returns>
		public bool Exists()
		{
			if (_performanceCounterCategoryAttribute == null)
				return false;

			return Exists(_performanceCounterCategoryAttribute.CategoryName);
		}

		#endregion Instance Members (Public)

		#region Shared Members (Public)

		/// <summary>
		/// Checks if a performance counter category exists.
		/// </summary>
		/// <param name="categoryName">Performance counter category name.</param>
		/// <returns>True if the category exists, false otherwise.</returns>
		public static bool Exists(string categoryName)
		{
			return PerformanceCounterCategory.Exists(categoryName);
		}

		#endregion Shared Members (Public)

		#region Instance Members (Private)

		private void AddCounters(Type[] reflectedTypes, bool silent)
		{
			ChoGuard.ArgumentNotNullOrEmpty(reflectedTypes, "reflectedTypes");

			string performanceCounterCategoryName = null;
			foreach (Type type in reflectedTypes)
			{
				if (type == null)
					continue;

				ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>(true);

				if (performanceCounterCategoryAttribute == null && !silent)
					throw new ApplicationException("Missing performance counter category attribute.");

				if (_performanceCounterCategoryAttribute == null)
					_performanceCounterCategoryAttribute = performanceCounterCategoryAttribute;

				if (performanceCounterCategoryName.IsNullOrEmpty())
					performanceCounterCategoryName = performanceCounterCategoryAttribute.CategoryName;
				else if (performanceCounterCategoryName != performanceCounterCategoryAttribute.CategoryName)
					continue;

				if (performanceCounterCategoryAttribute != null)
					AddCountersFromType(type);
			}
		}

		/// <summary>
		/// Populates the CounterCreationDataCollection for a given type.
		/// </summary>
		/// <param name="type">Type to search for performance counters.</param>
		private void AddCountersFromType(Type type)
		{
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
			{
				// ignore member if it is not a performance counter
				if (fieldInfo.FieldType != typeof(ChoPerformanceCounter))
					continue;

				// get the performance counter attribute
				ChoPerformanceCounterAttribute performanceCounterAttribute = fieldInfo.GetCustomAttribute<ChoPerformanceCounterAttribute>(false);
				// ignore it if it has no performance counter attribute set
				if (performanceCounterAttribute == null)
					continue;

				// only create a counter with multiple instances once 
				if (!performanceCounterAttribute.CounterInstanceName.IsNullOrEmpty() && _counterCreationData.ContainsKey(performanceCounterAttribute.CounterName))
					continue;

				if (_counterCreationData.ContainsKey(performanceCounterAttribute.CounterName))
					continue;

				foreach (CounterCreationData counterCreationData in performanceCounterAttribute.CreateCounters())
					_counterCreationData.Add(counterCreationData.CounterName, counterCreationData);
			}

			foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
			{
				if (!propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Length > 0)
					continue;

				// ignore member if it is not a performance counter
				if (propertyInfo.PropertyType != typeof(ChoPerformanceCounter))
					continue;

				// get the performance counter attribute
				ChoPerformanceCounterAttribute performanceCounterAttribute = propertyInfo.GetCustomAttribute<ChoPerformanceCounterAttribute>(false);
				// ignore it if it has no performance counter attribute set
				if (performanceCounterAttribute == null)
					continue;

				// only create a counter with multiple instances once 
				if (!performanceCounterAttribute.CounterInstanceName.IsNullOrEmpty() && _counterCreationData.ContainsKey(performanceCounterAttribute.CounterName))
					continue;

				if (_counterCreationData.ContainsKey(performanceCounterAttribute.CounterName))
					continue;

				foreach (CounterCreationData counterCreationData in performanceCounterAttribute.CreateCounters())
					_counterCreationData.Add(counterCreationData.CounterName, counterCreationData);
			}
		}

		#endregion Instance Members (Private)
	}
}
