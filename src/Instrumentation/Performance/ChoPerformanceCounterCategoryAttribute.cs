namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
    using System.Diagnostics;

	#endregion NameSpaces
	
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class ChoPerformanceCounterCategoryAttribute : Attribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// PerformanceCounterCategoryType defaults to Unknown
		/// </remarks>
		/// <param name="name">Performance counter category name.</param>
		public ChoPerformanceCounterCategoryAttribute(string categoryName)
			: this(categoryName, null, PerformanceCounterCategoryType.Unknown)
		{
			// nothing to do
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// PerformanceCounterCategoryType defaults to Unknown
		/// </remarks>
		/// <param name="name">Performance counter category name.</param>
		/// <param name="help">Performance counter category help.</param>
		public ChoPerformanceCounterCategoryAttribute(string categoryName, string categoryHelp)
			: this(categoryName, categoryHelp, PerformanceCounterCategoryType.Unknown)
		{
			// nothing to do
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Constructor
		/// </remarks>
		/// <param name="name">Performance counter category name.</param>
		/// <param name="categoryType">Performance counter category type.</param>
		public ChoPerformanceCounterCategoryAttribute(string categoryName, PerformanceCounterCategoryType categoryType)
			: this(categoryName, null, categoryType)
		{
			// nothing to do
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// Constructor
		/// </remarks>
		/// <param name="name">Performance counter category name.</param>
		/// <param name="help">Performance counter category help.</param>
		/// <param name="categoryType">Performance counter category type.</param>
		public ChoPerformanceCounterCategoryAttribute(string categoryName, string categoryHelp, PerformanceCounterCategoryType categoryType)
		{
			ChoGuard.ArgumentNotNullOrEmpty(categoryName, "categoryName");
			if (categoryHelp.IsNullOrEmpty())
				categoryHelp = categoryName;

			CategoryName = categoryName;
			CaregoryHelp = categoryHelp;
			CategoryType = categoryType;
		}

		/// <summary>
		/// Gets the performance counter category.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter category.
		/// </remarks>
		public string CategoryName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the performance counter category help.
		/// </summary>
		/// <remarks>
		/// Gets the performance counter category help.
		/// </remarks>
		public string CaregoryHelp
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the performance counter category type.
		/// </summary>
		/// <remarks>
		/// Default is Unknown.
		/// </remarks>
		public PerformanceCounterCategoryType CategoryType
		{
			get;
			private set;
		}
	}
}
