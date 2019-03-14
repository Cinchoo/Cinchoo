using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Runtime.ConstrainedExecution;
using System;

#if NET_2_0
using System.Runtime.ConstrainedExecution;
#endif

namespace Cinchoo.Core
{
#if NET_2_0
	[ComVisible (true)]
#endif
	[Serializable]
	public struct ChoRuntimeMethodHandle //: ISerializable
	{
		IntPtr value;

		public ChoRuntimeMethodHandle(IntPtr v)
		{
			value = v;
		}

		//protected RuntimeMethodHandle(SerializationInfo info, StreamingContext context)
		//{
		//}

		public IntPtr Value
		{
			get
			{
				return value;
			}
		}

		// This is from ISerializable
		//public void GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//}

		[MethodImpl(MethodImplOptions.InternalCall)]
		static extern IntPtr GetFunctionPointer(IntPtr m);

		[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
		public IntPtr GetFunctionPointer()
		{
			return GetFunctionPointer(value);
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public override bool Equals (object obj)
		{
			if (obj == null || GetType () != obj.GetType ())
				return false;

			return value == ((ChoRuntimeMethodHandle)obj).Value;
		}

		[ReliabilityContractAttribute (Consistency.WillNotCorruptState, Cer.Success)]
		public bool Equals (ChoRuntimeMethodHandle handle)
		{
			return value == handle.Value;
		}

		public override int GetHashCode ()
		{
			return value.GetHashCode ();
		}

		public static bool operator == (ChoRuntimeMethodHandle left, ChoRuntimeMethodHandle right)
		{
			return left.Equals (right);
		}

		public static bool operator != (ChoRuntimeMethodHandle left, ChoRuntimeMethodHandle right)
		{
			return !left.Equals (right);
		}
	}
}