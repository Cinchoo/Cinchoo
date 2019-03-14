namespace eSquare.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    #endregion NameSpaces

    //http://www.interact-sw.co.uk/iangblog/2004/06/06/weakeventhandler
    public class ChoWeakEventHandler<DT, T> where T : EventArgs, DT : delegate
    {
        private WeakReference weakRefToOriginalDelegate;
        public ChoWeakEventHandler(EventHandler<T> originalDelegate)
        {
            weakRefToOriginalDelegate = new WeakReference(originalDelegate);
        }

        private void DoInvoke(object sender, T args)
        {
            EventHandler<T> originalDelegate = (EventHandler<T>)weakRefToOriginalDelegate.Target;
            if (originalDelegate != null) originalDelegate(sender, args);
        }

        public static implicit operator DT(ChoWeakEventHandler<DT, T> wd)
        {
            object o = Delegate.CreateDelegate(typeof(DT), wd, "DoInvoke");
            return (DT)o;
        }
    }
}
