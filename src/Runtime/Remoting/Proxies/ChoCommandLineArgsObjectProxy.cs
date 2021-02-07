namespace Cinchoo.Core.Runtime.Remoting
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Cinchoo.Core.Shell;

    #endregion NameSpaces

    public class ChoCommandLineArgsObjectProxy : ChoRealProxy
    {
		#region Constructors

        //public ChoCommandLineArgsObjectProxy(Type type)
        //    : base(type)
        //{
        //}

        public ChoCommandLineArgsObjectProxy(MarshalByRefObject target, Type type)
            : base(target, type)
        {
        }

		#endregion Constructors

		public override object DoObjectInitialize(object target)
		{
			base.DoObjectInitialize(target);

            Type objType = target.GetType();
            ChoCommandLineArgObjectAttribute commandLineArgumentsObjectAttribute = objType.GetCustomAttribute(typeof(ChoCommandLineArgObjectAttribute)) as ChoCommandLineArgObjectAttribute;
            if (commandLineArgumentsObjectAttribute == null || commandLineArgumentsObjectAttribute.GetMe(objType) == null)
                throw new ApplicationException("Missing ChoCommandLineArgObjectAttribute attribute in {0} type.".FormatString(objType.FullName));

            return commandLineArgumentsObjectAttribute.GetMe(objType).Construct(target);
		}

		protected override void Dispose(bool finalize)
		{
			base.Dispose(finalize);
		}
    }
}
