namespace Cinchoo.Core.Runtime.Remoting
{
	#region NameSpaces

    using System;
    using System.Globalization;
    using Cinchoo.Core.Configuration;
    using Cinchoo.Core.Properties;

	#endregion NameSpaces

	public class ChoStandardConfigurationObjectProxy : ChoRealProxy
	{
		#region Constructors

        //public ChoStandardConfigurationObjectProxy(Type type)
        //    : base(type)
        //{
        //}

        public ChoStandardConfigurationObjectProxy(MarshalByRefObject target, Type type)
            : base(target, type)
        {
        }

        #endregion Constructors

		public override object DoObjectInitialize(object target)
		{
			base.DoObjectInitialize(target);

			Type configObjType = target.GetType();
			ChoConfigurationSectionAttribute configurationElement = ChoType.GetAttribute(configObjType, typeof(ChoConfigurationSectionAttribute)) as ChoConfigurationSectionAttribute;
			if (configurationElement == null || configurationElement.GetMe(configObjType) == null)
				throw new ChoConfigurationException(String.Format(CultureInfo.InvariantCulture, Resources.ES1001, configObjType.Name));

			return configurationElement.GetMe(configObjType).Construct(target);
		}

		protected override void Dispose(bool finalize)
		{
			base.Dispose(finalize);
		}
    }
}