namespace Cinchoo.Core.Diagnostics
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Reflection;

    #endregion NameSpaces

    public static class ChoStackTrace
    {
        #region Shared Members (Public)

        public static string GetCallerName()
        {
            return GetMemberName(2);
        }

        public static string GetMemberName(int frameNo)
        {
            StackFrame stackFrame = new StackTrace(true).GetFrame(frameNo);
            if (stackFrame == null) return "Unknown";

            return stackFrame.GetMethod().Name;
        }

        public static Type GetCallerType()
        {
            return GetCallerType(2);
        }

        public static Type GetCallerType(int frameNo)
        {
            StackFrame stackFrame = GetStackFrame(frameNo);
            return stackFrame != null ? stackFrame.GetMethod().ReflectedType : null;
        }

        public static Type GetCallerType(Type type)
        {
            StackFrame stackFrame = GetStackFrame(type);
            return stackFrame != null ? stackFrame.GetMethod().ReflectedType : null;
        }

        public static StackFrame GetStackFrame()
        {
            return GetStackFrame(2);
        }

        public static StackFrame GetStackFrame(int frameNo)
        {
            return new StackTrace(true).GetFrame(frameNo);
        }

        public static StackFrame GetStackFrame(Type type)
        {
            return null;

            StackFrame[] stackFrames = new StackTrace(true).GetFrames();

            int foundIndex = 0;
            for (int index = 0; index < stackFrames.Length; index++)
            {
                if (stackFrames[index].GetMethod().DeclaringType.FullName == type.FullName)
                {
                    foundIndex = index;
                    break;
                }
            }
            for (int index = foundIndex; index < stackFrames.Length; index++)
            {
                if (stackFrames[index].GetMethod().DeclaringType.FullName != type.FullName)
                {
                    foundIndex = index;
                    break;
                }
            }

            return stackFrames[foundIndex];
        }

        public static StackFrame GetStackFrame(string nameSpace)
        {
            return null;

            StackFrame[] stackFrames = new StackTrace(true).GetFrames();

            int foundIndex = 0;
            for (int index = 0; index < stackFrames.Length; index++)
            {
                if (stackFrames[index].GetMethod().DeclaringType.FullName.StartsWith(nameSpace))
                {
                    foundIndex = index;
                    break;
                }
            }
            for (int index = foundIndex; index < stackFrames.Length; index++)
            {
				if (stackFrames[index].GetMethod().DeclaringType.FullName.StartsWith("System.Lazy"))
					continue;

                if (!stackFrames[index].GetMethod().DeclaringType.FullName.StartsWith(nameSpace))
                {
                    foundIndex = index;
                    break;
                }
            }

            return stackFrames[foundIndex];
        }

        public static StackFrame GetParentStackFrame(StackFrame stackFrame)
        {
            return GetParentStackFrame(stackFrame.GetMethod());
        }

        public static StackFrame GetParentStackFrame(MethodBase methodBase)
        {
            return null;

            StackFrame[] stackFrames = new StackTrace(true).GetFrames();

            int foundIndex = 0;
            for (int index = 0; index < stackFrames.Length; index++)
            {
                if (stackFrames[index].GetMethod() == methodBase)
                    foundIndex = index;

                if (foundIndex > 0 && stackFrames[index].GetMethod() != methodBase)
                    break;
            }

            return foundIndex + 1 < stackFrames.Length ? stackFrames[foundIndex + 1] : null;
        }

        #endregion
    }
}
