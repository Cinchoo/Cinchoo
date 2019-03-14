namespace Cinchoo.Core.Factory
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using System.Reflection;

    #endregion NameSpaces

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class ChoObjectConstructorAttribute : ChoObjectNameableAttribute
    {
        #region Instance Properties (Public)

        private object[] _constructorArgs;
        public object[] ConstructorArgs
        {
            get { return _constructorArgs; }
        }

        private Type[] _constructorArgsTypes;
        public Type[] ConstructorArgsTypes
        {
            get { return _constructorArgsTypes; }
        }

        public string Id
        {
            get { return Name; }
            set { Name = value; }
        }

        #endregion Instance Properties (Public)

        #region Constructors

        public ChoObjectConstructorAttribute(string constructorArgs)
        {
            if (String.IsNullOrEmpty(constructorArgs)) return;

            Init(constructorArgs);
        }

        public ChoObjectConstructorAttribute(Type type, string methodName)
        {
            ChoGuard.ArgumentNotNull(type, "Type");
            ChoGuard.ArgumentNotNull(methodName, "MethodName");

            MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (methodInfo != null)
            {
                if (!methodInfo.IsStatic || methodInfo.ReturnType != typeof(string) ||
                    methodInfo.GetParameters().Length != 0)
                    throw new ChoApplicationException(String.Format("The {0} method should be of type `public static string MethodName()`", methodName));

                Init(methodInfo.Invoke(null, new object[0]) as string);
            }
        }

        #endregion Constructors

        #region Instance Members (Private)

        public void Init(string constructorArgs)
        {
            if (String.IsNullOrEmpty(constructorArgs))
            {
                _constructorArgs = new object[0];
                _constructorArgsTypes = new Type[0];
            }
            else
            {
                _constructorArgs = ChoString.Split2Objects(constructorArgs);
                _constructorArgsTypes = ChoType.ConvertToTypes(_constructorArgs);
            }
        }

        #endregion Instance Members (Private)
    }
}
