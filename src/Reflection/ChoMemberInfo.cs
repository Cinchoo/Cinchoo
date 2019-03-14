namespace Cinchoo.Core.Reflection
{
	#region NameSpaces

	using System;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;

    #endregion NameSpaces

    public class ChoMemberInfo
	{
		private IMethodMessage _methodMsg;
		internal IMethodMessage MethodMsg
		{
			set 
			{ 
				if (_methodMsg.MethodName != "FieldSetter" 
					&& !_methodMsg.MethodName.StartsWith("set_"))
					_methodMsg = value;

                if (value is System.Runtime.Remoting.Messaging.ReturnMessage)
                {
                    _exception = ((ReturnMessage)value).Exception;
                }
			}
		}
        internal ReturnMessage ReturnMessage
        {
            get;
            set;
        }
		private object _target;
		public object Target
		{
			get { return _target; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
		}

		MemberTypes _memberType;
		public MemberTypes MemberType
		{
			get { return _memberType; }
		}

		private bool _dirtyOperation;
		public bool DirtyOperation
		{
			get { return _dirtyOperation; }
		}

        private Exception _exception;
        public Exception Exception
        {
            get { return _exception; }
        }

		public object Value
		{
			get 
			{
				if (_methodMsg is IMethodCallMessage)
				{
					IMethodCallMessage methodCallMsg = _methodMsg as IMethodCallMessage;
					if (Info.MemberType == MemberTypes.Field)
						return methodCallMsg.ArgCount > 2 ? methodCallMsg.Args[2] : null;
					else if (Info.MemberType == MemberTypes.Property)
						return methodCallMsg.ArgCount > 0 ? methodCallMsg.Args[0] : null;
					else
						return methodCallMsg.Args;
				}
				else
				{
					IMethodReturnMessage methodReturnMsg = _methodMsg as IMethodReturnMessage;
					if (Info.MemberType == MemberTypes.Field)
						return methodReturnMsg.ArgCount > 0 ? methodReturnMsg.OutArgs[0] : null;
					else if (Info.MemberType == MemberTypes.Property)
						return methodReturnMsg.ArgCount > 0 ? methodReturnMsg.OutArgs[0] : null;
					else
						return methodReturnMsg.OutArgs;
				}
			}
			set
			{
				if (_methodMsg is IMethodCallMessage)
				{
					if (Info.MemberType == MemberTypes.Field)
						_methodMsg.Args[2] = value;
					else if (Info.MemberType == MemberTypes.Property)
						_methodMsg.Args[0] = value;
					else
						throw new InvalidOperationException("Can't set value to Method parameter through Value property.");
				}
				else
				{
					IMethodReturnMessage methodReturnMsg = _methodMsg as IMethodReturnMessage;
					if (Info.MemberType == MemberTypes.Field)
						methodReturnMsg.OutArgs[0] = value;
					else if (Info.MemberType == MemberTypes.Property)
						methodReturnMsg.OutArgs[0] = value;
					else
						throw new InvalidOperationException("Can't set value to Method parameter through Value property.");
				}
			}
		}

		private MemberInfo _memberInfo;
		public MemberInfo Info
		{
			get { return _memberInfo; }
		}

        public MethodCallMessageWrapper MethodCallMsg;

		internal ChoMemberInfo(object target, Type type, IMethodMessage methodCallMsg)
		{
			_methodMsg = methodCallMsg;
			_target = target;
            MethodCallMsg = _methodMsg as MethodCallMessageWrapper;

			if (_methodMsg.MethodName == "FieldSetter" || (_methodMsg.MethodName.StartsWith("set_")
				&& _methodMsg.MethodName != "set_Dirty"))
			{
				_dirtyOperation = true;
			}

			if (_methodMsg.MethodName == "FieldSetter" || _methodMsg.MethodName == "FieldGetter")
			{
				_name = _methodMsg.Args[1] as string;
				_memberType = MemberTypes.Field;
			}
			else if (_methodMsg.MethodName.StartsWith("get_"))
			{
				_name = _methodMsg.MethodName.Replace("get_", String.Empty);
				_memberType = MemberTypes.Property;
			}
			else if (_methodMsg.MethodName.StartsWith("set_"))
			{
				_name = _methodMsg.MethodName.Replace("set_", String.Empty);
				_memberType = MemberTypes.Property;
			}
			else
				_name = _methodMsg.MethodName;

			//_memberInfo = ChoType.GetMember(_target.GetType(), Name);
			_memberInfo = ChoType.GetMemberInfo(type, Name);

			if (_dirtyOperation)
			{
				//Set Converted value
				Value = ChoConvert.ConvertFrom(_target, Value, ChoType.GetMemberType(_memberInfo),
                    ChoTypeDescriptor.GetTypeConverters(_memberInfo), ChoTypeDescriptor.GetTypeConverterParams(_memberInfo));
				//Value = ChoConvert.ConvertFrom(_target, Value, ChoType.GetMemberType(_target.GetType(), _name),
				//    ChoType.GetTypeConverters(ChoType.GetMember(_target.GetType(), _name), typeof(ChoTypeConverterAttribute)));
			}
		}
	}
}
