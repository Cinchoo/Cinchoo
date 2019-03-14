namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Reflection;
    using System.Collections.Generic;

    #endregion NameSpaces

    #region Delegates (Public)

    public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler)
      where E : EventArgs;

    #endregion Delegates (Public)

    public static class ChoWeakEventHandler
    {
        #region IChoEventHandler Interface

        private interface IChoEventHandler<E>
          where E : EventArgs
        {
            EventHandler<E> Handler { get; }
        }

        #endregion IChoEventHandler Interface
        
        private class ChoWeakEventHandlerState<T, E> : IChoEventHandler <E>
            where T : class
            where E : EventArgs
        {
            #region Delegates (Private)

            private delegate void OpenEventHandler(T @this, object sender, E e);

            #endregion Delegates (Private)

            #region Instance Data Members (Private)

            private WeakReference _target;
            private OpenEventHandler _openHandler;
            private EventHandler<E> _eventHandler;
            private UnregisterCallback<E> _unregister;

            #endregion Instance Data Members (Private)

            #region Constructors

            public ChoWeakEventHandlerState(EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
            {
                _target = new WeakReference(eventHandler.Target);
                _openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
                _eventHandler = Invoke;
                _unregister = unregister;
            }

            #endregion Constructors

            #region Instance Members (Public)

            public void Invoke(object sender, E e)
            {
                T target = (T)_target.Target;

                if (target != null)
                    _openHandler.Invoke(target, sender, e);
                else if (_unregister != null)
                {
                    _unregister(_eventHandler);
                    _unregister = null;
                }
            }

            #endregion

            #region Instance Properties (Public)

            public EventHandler<E> Handler
            {
                get { return _eventHandler; }
            }

            #endregion Instance Properties (Public)

            #region Operators (Public)

            public static implicit operator EventHandler<E>(ChoWeakEventHandlerState<T, E> eventHandler)
            {
                return eventHandler._eventHandler;
            }

            #endregion Operators (Public)
        }


        #region Shared Members (Public)

        public static EventHandler<E> New<E>(EventHandler<E> eventHandler) where E : EventArgs
        {
            return New(eventHandler, null);
        }

        public static EventHandler<E> New<E>(EventHandler<E> eventHandler, UnregisterCallback<E> unregister) where E : EventArgs
        {
            if (eventHandler == null)
                throw new ArgumentNullException("eventHandler");
            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
                throw new ArgumentException("Only instance methods are supported.", "eventHandler");

            Type wehType = typeof(ChoWeakEventHandlerState<,>).MakeGenericType(eventHandler.Method.DeclaringType, typeof(E));
            ConstructorInfo wehConstructor = wehType.GetConstructor(new Type[] { typeof(EventHandler<E>), typeof(UnregisterCallback<E>) });

            IChoEventHandler<E> weh = (IChoEventHandler<E>)wehConstructor.Invoke(new object[] { eventHandler, unregister });

            return weh.Handler;
        }

        #endregion
    }
}
