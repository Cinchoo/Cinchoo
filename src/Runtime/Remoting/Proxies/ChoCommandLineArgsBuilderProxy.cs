namespace Cinchoo.Core.Runtime.Remoting
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Shell;
    using Cinchoo.Core.Reflection;

    #endregion NameSpaces

    public class ChoCommandLineArgsBuilderProxy : ChoRealProxy
    {
		#region Constructors

        //public ChoCommandLineArgsBuilderProxy(Type type)
        //    : base(type)
        //{
        //}

        public ChoCommandLineArgsBuilderProxy(MarshalByRefObject target, Type type)
            : base(target, type)
        {
        }

		#endregion Constructors

		public override object DoObjectInitialize(object target)
		{
			base.DoObjectInitialize(target);

            Type objType = target.GetType();
            ChoCommandLineArgBuilderAttribute commandLineArgumentsObjectAttribute = objType.GetCustomAttribute(typeof(ChoCommandLineArgBuilderAttribute)) as ChoCommandLineArgBuilderAttribute;
            if (commandLineArgumentsObjectAttribute == null || commandLineArgumentsObjectAttribute.GetMe(objType) == null)
                throw new ApplicationException("Missing ChoCommandLineArgBuilderAttribute attribute in {0} type.".FormatString(objType.FullName));

            return commandLineArgumentsObjectAttribute.GetMe(objType).Construct(target);
		}

		protected override void Dispose(bool finalize)
		{
			base.Dispose(finalize);
		}
    }
}
