namespace Cinchoo.Core.Configuration
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoBeforeConfigurationObjectMemberLoadedHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoBeforeConfigurationObjectMemberSetHandlerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoAfterConfigurationObjectMemberLoadedHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoAfterConfigurationObjectMemberSetHandlerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoConfigurationObjectMemberLoadErrorHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoConfigurationObjectMemberSetErrorHandlerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoBeforeConfigurationObjectLoadedHandlerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoAfterConfigurationObjectLoadedHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoBeforeConfigurationObjectPersistedHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoBeforeConfigurationObjectMemberPersistHandlerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoAfterConfigurationObjectMemberPersistHandlerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoAfterConfigurationObjectPersistedHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoConfigurationObjectLoadErrorHandlerAttribute : Attribute
	{
	}
}
