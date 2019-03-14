using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cinchoo.Core
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ChoSingletonInstanceInitializerAttribute : Attribute
	{
	}
}
