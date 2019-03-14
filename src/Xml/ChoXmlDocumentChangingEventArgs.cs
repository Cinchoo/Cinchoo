namespace Cinchoo.Core.Xml
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[Serializable]
	public class ChoXmlDocumentChangingEventArgs : ChoXmlDocumentChangedEventArgs
	{
		private bool _ignoreLoadDocument;

		public ChoXmlDocumentChangingEventArgs(string topXmlFilePath, string[] xmlFilePaths)
			: base(topXmlFilePath, xmlFilePaths)
		{
		}

		public bool IgnoreLoadDocument
		{
			get { return _ignoreLoadDocument; }
			set { _ignoreLoadDocument = value; }
		}
	}
}
