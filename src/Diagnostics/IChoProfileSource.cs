using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Factory;
using System.Diagnostics;

namespace Cinchoo.Core.Diagnostics
{
	public interface IChoProfileBackingStore
	{
		void Start(string actionCmds);
        void Stop(string actionCmds);
		void Write(string msg, object tag);
	}
}
