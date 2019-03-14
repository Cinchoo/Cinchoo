namespace Cinchoo.Core
{
    #region NameSpaces

    #endregion NameSpaces

    [ChoAppDomainEventsRegisterableType]
    public static class ChoFramworkInitializer
    {
        #region Instance Members (Private)

        [ChoAppDomainLoadMethod("Truncating profile contents....")]
        private static void CleanProfilesWithTruncateMode()
        {
			//foreach (Type type in ChoType.GetTypes(typeof(ChoBufferProfileAttribute)))
			//{
			//    ChoBufferProfileAttribute bufferProfileAttribute = ChoType.GetAttribute<ChoBufferProfileAttribute>(type);
			//    bufferProfileAttribute.Initialize();
			//}
			//foreach (Type type in ChoType.GetTypes(typeof(ChoStreamProfileAttribute)))
			//{
			//    ChoStreamProfileAttribute streamProfileAttribute = ChoType.GetAttribute<ChoStreamProfileAttribute>(type);
			//    streamProfileAttribute.Initialize();
			//}
			//foreach (Type type in ChoType.GetTypes(typeof(ChoEnlistProfileAttribute)))
			//{
			//    ChoEnlistProfileAttribute enlistProfileAttribute = ChoType.GetAttribute<ChoEnlistProfileAttribute>(type);
			//    enlistProfileAttribute.Initialize();
			//}
        }

        #endregion Instance Members (Private)
    }
}
