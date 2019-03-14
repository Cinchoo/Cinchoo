namespace Cinchoo.Core.Xml
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[Serializable]
	public abstract class ChoXmlDocumentEventArgs : EventArgs
	{
		private readonly string _topXmlFilePath;

 		public ChoXmlDocumentEventArgs(string topXmlFilePath)
        {
            this._topXmlFilePath = topXmlFilePath;
        }

        public string TopXmlFilePath
        {
            get { return _topXmlFilePath; }
        }
	}
}
