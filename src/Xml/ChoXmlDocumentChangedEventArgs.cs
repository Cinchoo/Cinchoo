namespace Cinchoo.Core.Xml
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[Serializable]
	public class ChoXmlDocumentChangedEventArgs : ChoXmlDocumentEventArgs
	{
		private readonly string[] _xmlFilePaths;

		public ChoXmlDocumentChangedEventArgs(string topXmlFilePath, string[] xmlFilePaths)
			: base(topXmlFilePath)
        {
			this._xmlFilePaths = xmlFilePaths;
        }

		public string[] XmlFilePaths
        {
			get { return _xmlFilePaths; }
        }
	}
}
