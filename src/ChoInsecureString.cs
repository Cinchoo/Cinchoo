using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using Cinchoo.Core.Collections;
using System.Runtime.CompilerServices;
using Cinchoo.Core.Collections.Generic;
using System.Collections;
using System.Diagnostics;

namespace Cinchoo.Core
{
    [CLSCompliant(false)]
    public sealed class ChoInsecureString : IDisposable, IEnumerable
    {
        internal ChoInsecureString(SecureString secureString)
        {
            _secureString = secureString;
            Initialize();
        }

        public string Value { get; private set; }

        private SecureString _secureString;
        private GCHandle _gcHandle;

#if !DEBUG
        [DebuggerHidden]
#endif
        private void Initialize()
        {
            unsafe
            {
                // We are about to create an unencrypted version of our sensitive string and store it in memory.
                // Don't let anyone (GC) make a copy.
                // To do this, create a new gc handle so we can "pin" the memory.
                // The gc handle will be pinned and later, we will put info in this string.
                _gcHandle = new GCHandle();
                // insecurePointer will be temporarily used to access the SecureString
                var insecurePointer = IntPtr.Zero;
                System.Runtime.CompilerServices.RuntimeHelpers.TryCode code = delegate
                {
                    // create a new string of appropriate length that is filled with 0's
                    Value = new string((char)0, _secureString.Length);
                    // Even though we are in the ExecuteCodeWithGuaranteedCleanup, processing can be interupted.
                    // We need to make sure nothing happens between when memory is allocated and
                    // when _gcHandle has been assigned the value. Otherwise, we can't cleanup later.
                    // PrepareConstrainedRegions is better than a try/catch. Not even a threadexception will interupt this processing.
                    // A CER is not the same as ExecuteCodeWithGuaranteedCleanup. A CER does not have a cleanup.

                    Action alloc = delegate { _gcHandle = GCHandle.Alloc(Value, GCHandleType.Pinned); };
                    alloc.ExecuteInConstrainedRegion();

                    // Even though we are in the ExecuteCodeWithGuaranteedCleanup, processing can be interupted.
                    // We need to make sure nothing happens between when memory is allocated and
                    // when insecurePointer has been assigned the value. Otherwise, we can't cleanup later.
                    // PrepareConstrainedRegions is better than a try/catch. Not even a threadexception will interupt this processing.
                    // A CER is not the same as ExecuteCodeWithGuaranteedCleanup. A CER does not have a cleanup.
                    Action toBSTR = delegate { insecurePointer = Marshal.SecureStringToBSTR(_secureString); };
                    toBSTR.ExecuteInConstrainedRegion();

                    // get a pointer to our new "pinned" string
                    var value = (char*)_gcHandle.AddrOfPinnedObject();
                    // get a pointer to the unencrypted string
                    var charPointer = (char*)insecurePointer;
                    // copy
                    for (int i = 0; i < _secureString.Length; i++)
                    {
                        value[i] = charPointer[i];
                    }
                };
                System.Runtime.CompilerServices.RuntimeHelpers.CleanupCode cleanup = delegate
                {
                    // insecurePointer was temporarily used to access the securestring
                    // set the string to all 0's and then clean it up. this is important.
                    // this prevents sniffers from seeing the sensitive info as it is cleaned up.
                    if (insecurePointer != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeBSTR(insecurePointer);
                    }
                };
                // Better than a try/catch. Not even a threadexception will bypass the cleanup code
                RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(code, cleanup, null);
            }
        }

#if !DEBUG
        [DebuggerHidden]
#endif
        public void Dispose()
        {
            unsafe
            {
                // we have created an insecurestring
                if (_gcHandle.IsAllocated)
                {
                    // get the address of our gchandle and set all chars to 0's
                    var insecurePointer = (char*)_gcHandle.AddrOfPinnedObject();
                    for (int i = 0; i < _secureString.Length; i++)
                    {
                        insecurePointer[i] = (char)0;
                    }
#if DEBUG
var disposed = "¡DISPOSED¡";
disposed = disposed.Substring(0, Math.Min(disposed.Length, _secureString.Length));
for (int i = 0; i < disposed.Length; ++i)
{
insecurePointer[i] = disposed[i];
}
#endif
                    _gcHandle.Free();
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            unsafe
            {
                if (_gcHandle.IsAllocated)
                {
                    return Value.GetEnumerator();
                }
                else
                {
                    return ChoEmptyEnumerable.Instance.GetEnumerator();
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}