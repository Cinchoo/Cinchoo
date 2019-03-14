namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections;
    using System.ComponentModel;
    using Cinchoo.Core.Diagnostics;
    using System.IO;
    using Cinchoo.Core.IO;

    #endregion NameSpaces

    public interface IChoTypeConverter
    {
        string Help();
    }

    public abstract class ChoParameterizedTypeConverter : TypeConverter, IChoTypeConverter
    {
        #region Instance Data Members (Private)

        private object[] _parameters;
        private object _target;

        #endregion Instance Data Members (Private)

        #region Constructors

        public ChoParameterizedTypeConverter(object[] parameters)
        {
            _parameters = parameters;
            Validate();
        }

        #endregion Constructors

        #region TypeConverter Overrides

        protected virtual void Validate()
        {
            
        }

        #endregion

        #region Instance Properties (Public)

        internal object[] Parameters
        {
            get { return _parameters; }
        }

        internal object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string LogFileName
        {
            get 
            { 
                if (Target != null)
                    return Path.Combine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(Target.GetType().FullName, ChoReservedFileExt.Log)); 
                else
                    return Path.Combine(ChoReservedDirectoryName.Settings, ChoPath.AddExtension(typeof(ChoParameterizedTypeConverter).FullName, ChoReservedFileExt.Log)); 
            }
        }

        #endregion Instance Properties (Public)

        #region Instance Members (Public)

        public virtual string Help()
        {
            return null;
        }

        #endregion Instance Members (Public)

    }
}
