namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Runtime.Serialization;

    #endregion NameSpaces

    [Serializable]
    public class ChoConfigurationConstructionException : ChoApplicationException
    {
        public ChoConfigurationConstructionException()
            : base()
        {
        }

        public ChoConfigurationConstructionException(string message)
            : base(message)
        {
        }

        public ChoConfigurationConstructionException(string message, Exception e)
            : base(message, e)
        {
        }

		protected ChoConfigurationConstructionException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}
