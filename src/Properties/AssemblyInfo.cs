using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using Cinchoo.Core;
using System.Web;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Cinchoo.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Cinchoo.Core")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("88bb2918-62b6-42ae-97e8-b7fb588111b7")]
[assembly: ChoTypeDiscoverableAssembly]
[assembly: CLSCompliant(true)]
[assembly: PreApplicationStartMethod(typeof(ChoPreApplicationStartCode), "Start")]