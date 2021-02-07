namespace Cinchoo.Core.Runtime.Remoting
{
	#region NameSpaces

    using System;
    using System.Reflection;
    using Cinchoo.Core;

    #endregion NameSpaces

    public class ChoSingletonProxy : ChoRealProxy
	{
		#region Constructors

        //public ChoSingletonProxy(Type type)
        //    : base(type)
        //{
        //}

        public ChoSingletonProxy(MarshalByRefObject target, Type type)
            : base(target, type)
        {
        }

		#endregion Constructors

		public override object DoObjectInitialize(object target)
		{
            if (target == null)
                return null;

			base.DoObjectInitialize(target);

			Type objType = target.GetType();
			Action<object> _objectInitializers;
			foreach (MethodInfo methodInfo in ChoType.GetMethods(objType, typeof(ChoSingletonInstanceInitializerAttribute)))
			{
				try
				{
					_objectInitializers = methodInfo.CreateDelegate<Action<object>>(target);
					_objectInitializers(target);
				}
				catch (Exception ex)
				{
					ChoApplication.WriteToEventLog(ChoApplicationException.ToString(ex));
				}
			}

			return target;
		}

		protected override void Dispose(bool finalize)
		{
			base.Dispose(finalize);
		}

        //protected override ChoRealProxy CreateInstance()
        //{
        //    ChoSingletonProxy sp = new ChoSingletonProxy(GetProxiedType());
        //    sp.InitializeServerObject(null);
        //    return sp;
        //}
    }
}
