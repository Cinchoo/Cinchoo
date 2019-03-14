#region Namespaces

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Specialized;

using Cinchoo.Core.IO;
using Cinchoo.Core.Reflection;
using Cinchoo.Core.Diagnostics;
using Cinchoo.Core.Configuration;

#endregion

namespace Cinchoo.Core.Runtime.Serialization
{
	[Serializable]
	[XmlRoot("liteSerializationSettings")]
	public sealed class LiteSerializationSettings
	{
		#region Shared Data Members (Private)

		private static LiteSerializationSettings _liteSerializationSettings = null;

		#endregion

		#region Instance Data Members (Public)

		[XmlAttribute("turnOn")]
		public bool TurnOn = false;

		[XmlAttribute("silent")]
		public bool Silent = true;

		[XmlAttribute("includeGlobalCodeBase")]
		public bool IncludeGlobalCodeBase = true;

		[XmlElement("liteSerializersPath", typeof(CodeBasePath))]
		public CodeBasePath[] CodeBasePaths;

		#endregion

		#region Instance Data Members (Private)

		private string[] _paths = null;

		#endregion

		#region Constructors

		static LiteSerializationSettings()
		{
			try
			{
                //_liteSerializationSettings = RITConfigurationSettings.GetConfig("liteSerializationSettings") as LiteSerializationSettings;
				//if (_liteSerializationSettings != null)
				//    ChoStreamProfile.WriteLine(ChoReservedDirectoryName.Settings, ChoType.GetLogFileName(_liteSerializationSettings.GetType()), _liteSerializationSettings.ToString(), true);
			}
			catch (Exception ex)
			{
				ChoTrace.WriteNThrow(ex);
			}
		}

		#endregion

		#region Instance Members (Public)

		public new string ToString()
		{
			StringBuilder msg = new StringBuilder();

			msg.AppendFormat(Environment.NewLine);
			msg.AppendFormat("-- LiteSerializationSettings Settings --{0}", Environment.NewLine);
			msg.AppendFormat("TurnOn: {0}{1}", TurnOn, Environment.NewLine);

			if (TurnOn)
			{
				msg.AppendFormat("Silent: {0}{1}", Silent, Environment.NewLine);
				msg.AppendFormat("IncludeGlobalCodeBase: {0}{1}", IncludeGlobalCodeBase, Environment.NewLine);
				if (Paths.Length > 0)
				{
					msg.AppendFormat("[{0}", Environment.NewLine);
					foreach (string path in Paths)
						msg.AppendFormat("\t{0}{1}", path, Environment.NewLine);
					msg.AppendFormat("]{0}", Environment.NewLine);
				}
			}
			return msg.ToString();
		}

		#endregion

		#region Shared Properties (Public)

		public static LiteSerializationSettings Me
		{
			get { return _liteSerializationSettings != null ? _liteSerializationSettings : new LiteSerializationSettings(); }
		}

		#endregion

		#region Instance Properties (Public)

		public string[] Paths
		{
			get 
			{ 
//                if (_paths == null)
//                {
//                    ArrayList paths = new ArrayList();

//                    //if (IncludeGlobalCodeBase)
//                    //    paths.AddRange(RITCodeBase.Me.Paths);

//                    if (CodeBasePaths != null && CodeBasePaths.Length > 0)
//                    {
//                        foreach (CodeBasePath codeBasePath in CodeBasePaths)
//                        {
//                            if (codeBasePath == null || codeBasePath.Path.Length == 0) continue;
//                            paths.AddRange(codeBasePath.GetDirectories());
//                        }
//                    }
//                    _paths = paths.ToArray(typeof(string)) as string[];
////					RITAssembly.AddPaths(_paths);
//                }

				return _paths;
			}
		}

		#endregion
	}
}
