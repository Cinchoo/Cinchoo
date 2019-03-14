namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Diagnostics;
	using System.Reflection;
	using Cinchoo.Core.Diagnostics;

	#endregion NameSpaces

	public class ChoPerformanceCounterFactory
	{
		#region Instance Data Members (Private)

		private readonly string CategoryName;
		private readonly string CategoryHelp;
		private readonly PerformanceCounterCategoryType CategoryType;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoPerformanceCounterFactory(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");

			ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>();
			if (performanceCounterCategoryAttribute == null)
				throw new ApplicationException(String.Format("Type '{0}' is not performance counter type.", type.FullName));
				
			CategoryName = performanceCounterCategoryAttribute.CategoryName;
			CategoryHelp = performanceCounterCategoryAttribute.CaregoryHelp;
			CategoryType = performanceCounterCategoryAttribute.CategoryType;
		}

		#endregion Constructors

		#region Instance Members (Public)

		/// <summary>
		/// Gets an instance of the specified performance counter in this category.
		/// </summary>
		/// <param name="counterName">Performance counter name.</param>
		/// <param name="instanceName">Performance counter instance name.</param>
		/// <param name="readOnly">If true, the returned counter is readonly. If false, the returned counter is writable.</param>
		/// <returns>Performance counter instance.</returns>
		public ChoPerformanceCounter GetCounter(string counterName, PerformanceCounterType counterType, string instanceName, bool readOnly)
		{
			return GetCounter(CategoryName, counterName, counterType, instanceName, readOnly);
		}

		/// <summary>
		/// Gets a readonly instance of the specified performance counter in this category.
		/// </summary>
		/// <param name="counterName">Performance counter name.</param>
		/// <param name="instanceName">Performance counter instance name.</param>
		/// <returns>Performance counter instance.</returns>
		public ChoPerformanceCounter GetCounter(string counterName, PerformanceCounterType counterType, string instanceName)
		{
			return GetCounter(counterName, counterType, instanceName, false);
		}

		/// <summary>
		/// Gets an instance of the specified performance counter in this category.
		/// </summary>
		/// <param name="counterName">Performance counter name.</param>
		/// <param name="readOnly">If true, the returned counter is readonly. If false, the returned counter is writable.</param>
		/// <returns>Performance counter instance.</returns>
		public ChoPerformanceCounter GetCounter(string counterName, PerformanceCounterType counterType, bool readOnly)
		{
			return GetCounter(CategoryName, counterName, counterType, string.Empty, readOnly);
		}

		/// <summary>
		/// Gets a readonly instance of the specified performance counter in this category.
		/// </summary>
		/// <param name="counterName">Performance counter name</param>
		/// <returns>Performance counter instance.</returns>
		public ChoPerformanceCounter GetCounter(string counterName, PerformanceCounterType counterType)
		{
			return GetCounter(counterName, counterType, false);
		}

		#endregion Instance Members (Public)

		#region Shared Members (Public)

		/// <summary>
		/// Creates instances for all performance counter members in the given type.
		/// </summary>q
		/// <remarks>
		/// The type must have the PerformanceCounterCategory attribute set. Each performance counter
		/// member must be static and tagged with a PerformanceCounter attribute.
		/// </remarks>
		/// <param name="type">Type to instantiate counters</param>
		/// <returns><b>True</b> if counters were created successfully, <b>false</b> otherwise.</returns>
		public static bool CreateCounters(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			return CreateCounters(type, null);
		}

		public static bool CreateCounters(object instance)
		{
			ChoGuard.ArgumentNotNull(instance, "Instance");
			return CreateCounters(instance.GetType(), instance);
		}

		public static bool Exists(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "Type");
			return Exists(type, null);
		}

		public static bool Exists(object instance)
		{
			ChoGuard.ArgumentNotNull(instance, "Instance");
			return Exists(instance.GetType(), instance);
		}

		/// <summary>
		/// Gets a performance counter instance for a given name and category.
		/// </summary>
		/// <remarks>
		/// Gets a performance counter instance for a given name and category.
		/// </remarks>
		/// <param name="categoryName">Performance counter category.</param>
		/// <param name="counterName">Performance counter name.</param>
		/// <param name="instanceName">Performance counter instance name.</param>
		/// <param name="readOnly">ReadOnly</param>
		/// <returns>Performance counter instance.</returns>
		public static ChoPerformanceCounter GetCounter(string categoryName, string counterName, PerformanceCounterType counterType, string instanceName, bool readOnly)
		{
			return new ChoPerformanceCounter(categoryName, counterName, counterType, instanceName, readOnly);
		}

		#endregion Shared Members (Public)

		#region Shared Members (Private)

		private static bool Exists(Type type, object instance)
		{
			// get category attribute
			ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>(true);

			// we don't have performance counter category, we are done
			if (performanceCounterCategoryAttribute == null)
				return false;

			return PerformanceCounterCategory.Exists(performanceCounterCategoryAttribute.CategoryName);
		}

		/// <summary>
		/// Creates instances for all performance counter members in the given type.
		/// </summary>
		/// <remarks>
		/// The type must have the PerformanceCounterCategory attribute set. Each performance counter
		/// member must be static and tagged with a PerformanceCounter attribute.
		/// </remarks>
		/// <param name="type">Type to instantiate counters</param>
		/// <param name="instance">Instance to assign performance counters to.</param>
		/// <returns><b>True</b> if counters were created successfully, <b>false</b> otherwise.</returns>
		private static bool CreateCounters(Type type, object instance)
		{
			// get category attribute
			ChoPerformanceCounterCategoryAttribute performanceCounterCategoryAttribute = type.GetCustomAttribute<ChoPerformanceCounterCategoryAttribute>(true);

			// we don't have performance counter category, we are done
			if (performanceCounterCategoryAttribute == null)
				return false;

			string categoryName = performanceCounterCategoryAttribute.CategoryName;

			bool result = false;
			try
			{
				if (PerformanceCounterCategory.Exists(categoryName))
				{
					// get the category type
					PerformanceCounterCategory category = new PerformanceCounterCategory(categoryName);

					foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
					{
						if (fieldInfo.FieldType != typeof(ChoPerformanceCounter))
							continue;

                        try
                        {
                            ChoPerformanceCounterAttribute performanceCounterAttribute = fieldInfo.GetCustomAttribute<ChoPerformanceCounterAttribute>();
                            if (performanceCounterAttribute == null)
                                continue;

                            if (performanceCounterAttribute.MachineName.IsNullOrEmpty() ||
                                (!performanceCounterAttribute.MachineName.IsNullOrEmpty() && String.Compare(performanceCounterAttribute.MachineName, Environment.MachineName, true) == 0))
                            {
                                if (fieldInfo.IsStatic && fieldInfo.GetValue(instance) != null)
                                    continue;

                                string instanceName = ChoPerformanceCounter.DefaultInstanceName;
                                // use a default instance name if the the counter does not have one and the category is marked MultiInstance
                                if (category.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                                    instanceName = performanceCounterAttribute.CounterInstanceName.IsNullOrEmpty() ? instanceName : performanceCounterAttribute.CounterInstanceName;

                                // assign the performance counter
                                fieldInfo.SetValue(instance, new ChoPerformanceCounter(categoryName, performanceCounterAttribute.CounterName, performanceCounterAttribute.CounterType, instanceName, false));
                            }
                        }
                        catch (Exception innerEx)
                        {
                            ChoTrace.Error(innerEx);
                        }
					}

					foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
					{
						if (propertyInfo.PropertyType != typeof(ChoPerformanceCounter))
							continue;

                        try
                        {
                            if (!propertyInfo.CanWrite || propertyInfo.GetIndexParameters().Length > 0)
                                continue;

                            ChoPerformanceCounterAttribute performanceCounterAttribute = propertyInfo.GetCustomAttribute<ChoPerformanceCounterAttribute>();
                            if (performanceCounterAttribute == null)
                                continue;

                            if (performanceCounterAttribute.MachineName.IsNullOrEmpty() ||
                                (!performanceCounterAttribute.MachineName.IsNullOrEmpty() && String.Compare(performanceCounterAttribute.MachineName, Environment.MachineName, true) == 0))
                            {
                                if (propertyInfo.GetSetMethod(true).IsStatic && propertyInfo.GetValue(instance, null) != null)
                                    continue;

                                // use a default instance name if the the counter does not have one and the category is marked MultiInstance
                                string instanceName = ChoPerformanceCounter.DefaultInstanceName;
                                // use a default instance name if the the counter does not have one and the category is marked MultiInstance
                                if (category.CategoryType == PerformanceCounterCategoryType.MultiInstance)
                                    instanceName = performanceCounterAttribute.CounterInstanceName.IsNullOrEmpty() ? instanceName : performanceCounterAttribute.CounterInstanceName;

                                // assign the performance counter
                                propertyInfo.SetValue(instance, new ChoPerformanceCounter(categoryName, performanceCounterAttribute.CounterName, performanceCounterAttribute.CounterType, instanceName, false), null);
                            }
                        }
                        catch (Exception innerEx)
                        {
                            ChoTrace.Error(innerEx);
                        }
					}
					result = true;
				}
			}
			catch (Exception outerEx)
			{
                ChoTrace.Error(outerEx);
			}

			return result;
		}

		#endregion Shared Members (Private)
	}
}
