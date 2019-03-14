#region NameSpaces
 
using System;
using System.IO;
using System.Text;
using System.Security;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Configuration;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace eSquare.Core
{
	#region Enum (Public)

	public enum Severity { Debug, Info, Warning, Error, Critical, Unclassified }

	#endregion

	/// <summary>
	/// Base Application Exception Class. You can use this as the base exception object from
	/// which to derive your applications exception hierarchy.
	/// </summary>
	[Serializable]
	public class ChoApplicationException : ApplicationException
	{
		#region Declare Member Variables

		private string _machineName; 
		private string _appDomainName;
		private string _threadIdentity; 
		private string _windowsIdentity; 
		private DateTime _createdDateTime = DateTime.Now;
		private Severity _severity = Severity.Unclassified;

		private static ResourceManager _resourceManager = new ResourceManager(
			typeof(ChoAppSettings).Namespace + ".ExceptionManagerText", 
			Assembly.GetAssembly(typeof(ChoAppSettings))
			);
		
		// Collection provided to store any extra information associated with the exception.
		private NameValueCollection _additionalInformation = new NameValueCollection();
		private const string _textSeperator = "*********************************************";

		#endregion

		#region Constructors
		/// <summary>
		/// Constructor with no params.
		/// </summary>
		public ChoApplicationException() : base()
		{
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ChoApplicationException(string message) : base(message) 
		{
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor allowing the Message property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		public ChoApplicationException(string message, Severity severity) : base(message) 
		{
			InitializeEnvironmentInformation();
			_severity = severity;
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ChoApplicationException(string message, Exception inner) : base(message, inner)
		{
			InitializeEnvironmentInformation();
		}
		/// <summary>
		/// Constructor allowing the Message and InnerException property to be set.
		/// </summary>
		/// <param name="message">String setting the message of the exception.</param>
		/// <param name="inner">Sets a reference to the InnerException.</param>
		public ChoApplicationException(string message, Exception inner, Severity severity) : 
			base(message, inner)
		{
			InitializeEnvironmentInformation();
			_severity = severity;
		}
		/// <summary>
		/// Constructor used for deserialization of the exception class.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		protected ChoApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_machineName = info.GetString("_machineName");
			_appDomainName = info.GetString("_appDomainName");
			_threadIdentity = info.GetString("_threadIdentity");
			_windowsIdentity = info.GetString("_windowsIdentity");
			_createdDateTime = info.GetDateTime("_createdDateTime");
			_severity = (Severity)info.GetInt32("_severity");

			_additionalInformation = (NameValueCollection)info.GetValue("_additionalInformation",typeof(NameValueCollection));
		}
		#endregion

		#region Serializable Override

		/// <summary>
		/// Override the GetObjectData method to serialize custom values.
		/// </summary>
		/// <param name="info">Represents the SerializationInfo of the exception.</param>
		/// <param name="context">Represents the context information of the exception.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData( SerializationInfo info, StreamingContext context ) 
		{
			info.AddValue("_machineName", _machineName, typeof(string));
			info.AddValue("_appDomainName", _appDomainName, typeof(string));
			info.AddValue("_threadIdentity", _threadIdentity, typeof(string));
			info.AddValue("_windowsIdentity", _windowsIdentity, typeof(string));
			info.AddValue("_createdDateTime", _createdDateTime);
			info.AddValue("_severity", _severity, typeof(int));

			info.AddValue("_additionalInformation", _additionalInformation, typeof(NameValueCollection));
			base.GetObjectData(info,context);
		}

		#endregion

		#region Instance Properties (Public)

		/// <summary>
		/// Machine name where the exception occurred.
		/// </summary>
		public string MachineName
		{
			get
			{
				return _machineName;
			}
		}

		/// <summary>
		/// Date and Time the exception was created.
		/// </summary>
		public DateTime CreatedDateTime
		{
			get
			{
				return _createdDateTime;
			}
		}

		/// <summary>
		/// AppDomain name where the exception occurred.
		/// </summary>
		public string AppDomainName
		{
			get
			{
				return _appDomainName;
			}
		}

		/// <summary>
		/// Identity of the executing thread on which the exception was created.
		/// </summary>
		public string ThreadIdentityName
		{
			get
			{
				return _threadIdentity;
			}
		}

		/// <summary>
		/// Windows identity under which the code was running.
		/// </summary>
		public string WindowsIdentityName
		{
			get
			{
				return _windowsIdentity;
			}
		}

		/// <summary>
		/// Collection allowing additional information to be added to the exception.
		/// </summary>
		public NameValueCollection AdditionalInformation
		{
			get
			{
				return _additionalInformation;
			}
		}

		public Severity Severity
		{
			get
			{
				return _severity;
			}
		}

		#endregion

		#region Instance Memeber functions (Private)

		/// <summary>
		/// Initialization function that gathers environment information safely.
		/// </summary>
		private void InitializeEnvironmentInformation()
		{									
			try
			{				
				_machineName = Environment.MachineName;
			}
			catch(SecurityException)
			{
				_machineName = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_PERMISSION_DENIED");
				
			}
			catch
			{
				_machineName = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_INFOACCESS_EXCEPTION");
			}
					
			try
			{
				_threadIdentity = Thread.CurrentPrincipal.Identity.Name;
			}
			catch(SecurityException)
			{
				_threadIdentity = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_PERMISSION_DENIED");
			}
			catch
			{
				_threadIdentity = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_INFOACCESS_EXCEPTION");
			}			
			
			try
			{
				_windowsIdentity = WindowsIdentity.GetCurrent().Name;
			}
			catch(SecurityException)
			{
				_windowsIdentity = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_PERMISSION_DENIED");
			}
			catch
			{
				_windowsIdentity = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_INFOACCESS_EXCEPTION");
			}
			
			try
			{					
				_appDomainName = AppDomain.CurrentDomain.FriendlyName;
			}
			catch(SecurityException)
			{
				_appDomainName = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_PERMISSION_DENIED");
			}
			catch
			{
				_appDomainName = _resourceManager.GetString("RES_EXCEPTIONMANAGEMENT_INFOACCESS_EXCEPTION");
			}			
		}

		#endregion

		#region Shared Members (Public)

		public static string SerializeException(Exception exception)
		{
			MemoryStream stream = new MemoryStream();
			new BinaryFormatter().Serialize(stream, exception);
			stream.Position = 0;
			return Convert.ToBase64String(stream.ToArray());
		}

		public static string ToString(Exception exception, NameValueCollection additionalInfo)
		{
			// Create StringBuilder to maintain publishing information.
			StringBuilder info = new StringBuilder();

			#region Record the contents of the AdditionalInfo collection
			// Record the contents of the AdditionalInfo collection.
			if (additionalInfo != null)
			{
				// Record General information.
				info.AppendFormat("{0}General Information {0}{1}{0}Additional Info:", Environment.NewLine, _textSeperator);

				foreach (string i in additionalInfo)
				{
					info.AppendFormat("{0}{1}: {2}", Environment.NewLine, i, additionalInfo.Get(i));
				}
			}
			#endregion

			if (exception == null)
			{
				info.AppendFormat("{0}{0}No Exception object has been provided.{0}", Environment.NewLine);
			}
			else
			{
				#region Loop through each exception class in the chain of exception objects
				// Loop through each exception class in the chain of exception objects.
				Exception currentException = exception;	// Temp variable to hold InnerException object during the loop.
				int intExceptionCount = 1;				// Count variable to track the number of exceptions in the chain.
				do
				{
					// Write title information for the exception object.
					info.AppendFormat("{0}{0}{1}) Exception Information{0}{2}", Environment.NewLine, intExceptionCount.ToString(), _textSeperator);
					info.AppendFormat("{0}Exception Type: {1}", Environment.NewLine, currentException.GetType().FullName);
				
					#region Loop through the public properties of the exception object and record their value
					// Loop through the public properties of the exception object and record their value.
					PropertyInfo[] aryPublicProperties = currentException.GetType().GetProperties();
					NameValueCollection currentAdditionalInfo;
					foreach (PropertyInfo p in aryPublicProperties)
					{
						// Do not log information for the InnerException or StackTrace. This information is 
						// captured later in the process.
						if (p.Name != "InnerException" && p.Name != "StackTrace")
						{
							if (p.GetValue(currentException,null) == null)
							{
								info.AppendFormat("{0}{1}: NULL", Environment.NewLine, p.Name);
							}
							else
							{
								// Loop through the collection of AdditionalInformation if the exception type is a ChoApplicationException.
								if (p.Name == "AdditionalInformation" && currentException is ChoApplicationException)
								{
									// Verify the collection is not null.
									if (p.GetValue(currentException,null) != null)
									{
										// Cast the collection into a local variable.
										currentAdditionalInfo = (NameValueCollection)p.GetValue(currentException,null);

										// Check if the collection contains values.
										if (currentAdditionalInfo.Count > 0)
										{
											info.AppendFormat("{0}AdditionalInformation:", Environment.NewLine);

											// Loop through the collection adding the information to the string builder.
											for (int i = 0; i < currentAdditionalInfo.Count; i++)
											{
												info.AppendFormat("{0}{1}: {2}", Environment.NewLine, currentAdditionalInfo.GetKey(i), currentAdditionalInfo[i]);
											}
										}
									}
								}
									// Otherwise just write the ToString() value of the property.
								else
								{
									info.AppendFormat("{0}{1}: {2}", Environment.NewLine, p.Name, p.GetValue(currentException,null));
								}
							}
						}
					}
					#endregion
					#region Record the Exception StackTrace
					// Record the StackTrace with separate label.
					if (currentException.StackTrace != null)
					{
						info.AppendFormat("{0}{0}StackTrace Information{0}{1}", Environment.NewLine, _textSeperator);
						info.AppendFormat("{0}{1}", Environment.NewLine, currentException.StackTrace);
					}
					#endregion

					// Reset the temp exception object and iterate the counter.
					currentException = currentException.InnerException;
					intExceptionCount++;
				} while (currentException != null);
				#endregion
			}

			return info.ToString();
		}

		public static void SetSource(Exception ex, string prefixString)
		{
			if (prefixString == null || prefixString.Length == 0) return;

			prefixString = String.Format("[{0}]:", prefixString.ToUpper());
			if (ex != null)
			{
				if (ex.Source == null)
					ex.Source = prefixString;
				else if (!ex.Source.StartsWith(prefixString))
					ex.Source = String.Format("{0}{1}", prefixString, ex.Source);
			}
		}

		public static void SetProcessed(Exception ex)
		{
			if (ex != null)
			{
				if (ex.Source == null)
					ex.Source = "[PROCESSED]:";
				else if (!ex.Source.StartsWith("[PROCESSED]:"))
					ex.Source = String.Format("[PROCESSED]:{0}", ex.Source);
			}
		}

		public static bool IsSourceStartWith(Exception ex, string prefixString)
		{
			if (prefixString == null || prefixString.Length == 0) return false;

			prefixString = String.Format("[{0}]:", prefixString.ToUpper());
			
			if (ex != null)
			{
				Exception innerException = ex;
				while (true)
				{
					if (innerException == null) break;
					if (innerException.Source != null && innerException.Source.StartsWith(prefixString))
						return true;
					else
						innerException = innerException.InnerException;
				}
			}

			return false;
		}

		public static bool IsProcessed(Exception ex)
		{
			if (ex != null)
			{
				Exception innerException = ex;
				while (true)
				{
					if (innerException == null) break;
					if (innerException.Source != null && innerException.Source.StartsWith("[PROCESSED]:"))
						return true;
					else
						innerException = innerException.InnerException;
				}
			}

			return false;
		}

		public static Exception Reset(Exception ex)
		{
			if (ex != null)
			{
				Exception innerException = ex;
				while (true)
				{
					if (innerException == null) break;
					if (innerException.Source != null && innerException.Source.StartsWith("[PROCESSED]:"))
						innerException.Source = innerException.Source.Replace("[PROCESSED]:", String.Empty);
					else
						innerException = innerException.InnerException;
				}
			}
			return ex;
		}

		#endregion
	}	
}
