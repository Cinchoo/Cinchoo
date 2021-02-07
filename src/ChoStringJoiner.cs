namespace Cinchoo.Core
{
	#region NameSpaces

	using System;
	using System.Text;
	using System.Collections.Generic;

	using Cinchoo.Core;
	using Cinchoo.Core.Configuration;

	#endregion NameSpaces

	[ChoTypeFormatter("String Joiner Settings")]
	[ChoObjectFactoryAttribute(ChoObjectConstructionType.Prototype)]
	[ChoConfigurationSection("cinchoo/stringJoinerSettings")]
	public class ChoStringJoinerSettings
	{
        private static readonly ChoStringJoinerSettings _instance = new ChoStringJoinerSettings();

		#region Instance Data Members (Public)

		[ChoPropertyInfo("Separator")]
		public string Separator = ",";

		[ChoPropertyInfo("Prefix")]
		public string Prefix;

		[ChoPropertyInfo("Postfix")]
		public string Postfix;

		#endregion

		#region Constructors

		public ChoStringJoinerSettings()
		{
		}

		public ChoStringJoinerSettings(string separator, string prefix, string postFix)
		{
			Separator = separator;
			Prefix = prefix;
			Postfix = postFix;
		}

		#endregion Constructors

		#region Shared Properties

		public static ChoStringJoinerSettings Me
		{
            get { return _instance; }
		}

		#endregion
	}

	public class ChoStringJoiner : IDisposable
	{
		#region Instance Data Members (Private)

		private readonly ChoStringJoinerSettings _charJoinerSettings;

		#endregion Instance Data Members (Private)

		#region Constructors

		public ChoStringJoiner()
		{
			_charJoinerSettings = ChoStringJoinerSettings.Me;
		}

		public ChoStringJoiner(string separator) : this()
		{
			_charJoinerSettings.Separator = separator;
		}

		public ChoStringJoiner(ChoStringJoinerSettings stringJoinerSettings)
		{
			if (stringJoinerSettings == null)
				throw new ArgumentNullException("stringJoinerSettings");

			_charJoinerSettings = stringJoinerSettings;
		}

		#endregion Constructors

		public string Join(char[] inChars)
		{
			StringBuilder joinedChars = new StringBuilder();
			foreach (char inChar in inChars)
				joinedChars.Append(joinedChars.Length == 0 ?
					String.Format(@"{0}{1}{2}", _charJoinerSettings.Prefix, inChar.ToString(), _charJoinerSettings.Postfix) :
					String.Format(@"{0}{1}{2}{3}", _charJoinerSettings.Separator, _charJoinerSettings.Prefix, inChar.ToString(), _charJoinerSettings.Postfix));

			return joinedChars.ToString();
		}

		public string Join(string[] inStrings)
		{
			StringBuilder result = new StringBuilder();
			foreach (string inString in inStrings)
				result.Append(result.Length == 0 ?
					String.Format(@"{0}{1}{2}", _charJoinerSettings.Prefix, inString.ToString(), _charJoinerSettings.Postfix) :
					String.Format(@"{0}{1}{2}{3}", _charJoinerSettings.Separator, _charJoinerSettings.Prefix, inString.ToString(), _charJoinerSettings.Postfix));

			return result.ToString();
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
