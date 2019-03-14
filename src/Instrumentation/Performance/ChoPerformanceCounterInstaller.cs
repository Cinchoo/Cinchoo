namespace Cinchoo.Core.Instrumentation
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Configuration.Install;
	using System.Diagnostics;

	#endregion NameSpaces

	public static class ChoPerformanceCounterInstaller
	{
		public static bool Install(Type type)
		{
			return Install(type, false);
		}

		public static bool Install(Type type, bool forceCreate)
		{
			ChoGuard.ArgumentNotNull(type, "type");
			return Install(new Type[] { type }, forceCreate);
		}

		public static bool Install(Type[] types)
		{
			return Install(types, false);
		}

		public static bool Install(Type[] types, bool forceCreate)
		{
			ChoGuard.ArgumentNotNull(types, "types");

			ChoPerformanceCounterInstallerBuilder builder = new ChoPerformanceCounterInstallerBuilder(types);
			PerformanceCounterInstaller installer = builder.CreateInstaller();

			if (PerformanceCounterCategory.Exists(installer.CategoryName))
			{
				if (forceCreate)
					PerformanceCounterCategory.Delete(installer.CategoryName);
				else
					return false;
			}

			PerformanceCounterCategory.Create(installer.CategoryName, installer.CategoryHelp, installer.CategoryType, installer.Counters);
			return true;
		}

		public static void Uninstall(Type type)
		{
			ChoGuard.ArgumentNotNull(type, "type");

			ChoPerformanceCounterInstallerBuilder builder = new ChoPerformanceCounterInstallerBuilder(type);
			PerformanceCounterInstaller installer = builder.CreateInstaller();

			if (PerformanceCounterCategory.Exists(installer.CategoryName))
				PerformanceCounterCategory.Delete(installer.CategoryName);
		}

		public static void Uninstall(Type[] types)
		{
			ChoGuard.ArgumentNotNullOrEmpty(types, "types");

			foreach (Type type in types)
			{
				if (type == null)
					continue;

				Uninstall(type);
			}
		}
	}
}
