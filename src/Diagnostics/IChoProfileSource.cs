using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Factory;

namespace Cinchoo.Core.Diagnostics
{
	public interface IChoProfileBackingStore
	{
		void Start();
		void Stop();
		void Write(string msg);
	}
}
