namespace Cinchoo.Core.Shell
{
	#region NameSpaces

	using System;
	using System.Collections.Generic;
	using System.Text;

	#endregion NameSpaces

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoBeforeCommandLineArgLoadedHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoAfterCommandLineArgLoadedHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoCommandLineArgLoadErrorHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoBeforeCommandLineArgObjectLoadedHandlerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoAfterCommandLineArgObjectLoadedHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ChoCommandLineArgObjectLoadErrorHandlerAttribute : Attribute
	{
	}

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoCommandLineArgMemberNotFoundHandlerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ChoUnrecognizedCommandLineArgFoundHandlerAttribute : Attribute
    {
    }
}
