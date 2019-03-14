using System;
using System.Collections.Generic;
using System.Text;

namespace Cinchoo.Core.Threading
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections.Specialized;

    using Cinchoo.Core.Configuration;

    #endregion NameSpaces
        
    public sealed class ChoThreadPoolSettings //: IConfigSettingsHandler
    {
        #region Shared Data Members (Public)
		
		public static int MaxWorkerThreads = 25;
		public static int MinWorkerThreads = 5;
		public static int IdleTimeout = 60 * 1000;
		public static int WorkItemTimeout = 300;
		public static bool DisposeOfStateObjects = true;
		public static bool UseCallerContext = true;
		public static bool BlockIfPoolBusy = false;
		public static CallToPostExecute CallToPostExecute = CallToPostExecute.Always;
		public static PostExecuteWorkItemCallback PostExecuteWorkItemCallback = null; //Not used now.

		#endregion Shared Data Members (Public)

		#region Shared Constructors

        static ChoThreadPoolSettings()
		{
            //RITConfigSettings.Initialize("riskIt/threadPoolSettings", new ChoThreadPoolSettings());
		}

		#endregion

		#region Shared Member Functions (Public)
		
		private static void Load(NameValueCollection nameValues)
		{
			if (nameValues != null)
			{
				try
				{
					MaxWorkerThreads = Int32.Parse(nameValues["MaxWorkerThreads"]);
				}
				catch
				{
				}

				try
				{
					MinWorkerThreads = Int32.Parse(nameValues["MinWorkerThreads"]);
				}
				catch
				{
				}
		
				try
				{
					int idleTimeout = Int32.Parse(nameValues["IdleTimeoutInSeconds"]);
					if (idleTimeout >= 0)
						IdleTimeout = idleTimeout * 1000;
				}
				catch
				{
				}
				
				try
				{
					int workItemTimeout = Int32.Parse(nameValues["WorkItemTimeout"]);
					if (workItemTimeout > 0)
						WorkItemTimeout = workItemTimeout;
					else
						WorkItemTimeout = -1;
				}
				catch
				{
				}

				try
				{
					DisposeOfStateObjects = Boolean.Parse(nameValues["DisposeOfStateObjects"]);
				}
				catch
				{
				}
				
				try
				{
					UseCallerContext = Boolean.Parse(nameValues["UseCallerContext"]);
				}
				catch
				{
				}
				
				try
				{
					BlockIfPoolBusy = Boolean.Parse(nameValues["BlockIfPoolBusy"]);
				}
				catch
				{
				}
				
				try
				{
					if (nameValues["CallToPostExecute"] != null)
						CallToPostExecute = (CallToPostExecute)Enum.Parse(typeof(CallToPostExecute), nameValues["CallToPostExecute"]);
				}
				catch
				{
				}

				Validate();
			}
		}

		public static void Validate()
		{
			if (MinWorkerThreads < 0)
			{
				throw new ArgumentOutOfRangeException(
					"MinWorkerThreads", "MinWorkerThreads cannot be negative");
			}

			if (MaxWorkerThreads <= 0)
			{
				throw new ArgumentOutOfRangeException(
					"MaxWorkerThreads", "MaxWorkerThreads must be greater than zero");
			}

			if (MinWorkerThreads > MaxWorkerThreads)
			{
				throw new ArgumentOutOfRangeException(
					"MinWorkerThreads, MaxWorkerThreads", 
					"MaxWorkerThreads must be greater or equal to MinWorkerThreads");
			}

			if (IdleTimeout < 0)
				IdleTimeout = 60 * 1000;
		}

		#endregion

        //#region IConfigSettingsHandler Members

        //public void HandleConfigSettings(NameValueCollection nameValues)
        //{
        //    Load(nameValues);
        //}

        //public override string ToString()
        //{
        //    StringBuilder msg = new StringBuilder();

        //    msg.AppendFormat(Environment.NewLine);

        //    msg.AppendFormat("-- RITThreadPool Settings --{0}", Environment.NewLine);
        //    msg.AppendFormat("\tMaxWorkerThreads: {0}{1}", MaxWorkerThreads, Environment.NewLine);
        //    msg.AppendFormat("\tMinWorkerThreads: {0}{1}", MinWorkerThreads, Environment.NewLine);
        //    msg.AppendFormat("\tIdleTimeout: {0} (seconds){1}", IdleTimeout / 1000, Environment.NewLine);
        //    if (WorkItemTimeout == -1)
        //        msg.AppendFormat("\tWorkItemTimeout: Infinite{0}", Environment.NewLine);
        //    else
        //        msg.AppendFormat("\tWorkItemTimeout: {0} (seconds){1}", WorkItemTimeout, Environment.NewLine);
        //    msg.AppendFormat("\tDisposeOfStateObjects: {0}{1}", DisposeOfStateObjects, Environment.NewLine);
        //    msg.AppendFormat("\tUseCallerContext: {0}{1}", UseCallerContext, Environment.NewLine);
        //    msg.AppendFormat("\tBlockIfPoolBusy: {0}{1}", BlockIfPoolBusy, Environment.NewLine);
        //    msg.AppendFormat("\tCallToPostExecute: {0}{1}", CallToPostExecute, Environment.NewLine);
			
        //    msg.AppendFormat(Environment.NewLine);

        //    return msg.ToString();
        //}

        //#endregion
}
}
