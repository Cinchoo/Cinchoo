using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cinchoo.Core
{
    public class ChoPreApplicationStartCode
    {
        public static void Start()
        {
            ChoApplicationHost host = ChoApplication.Host;
            if (host != null)
                host.OnStartService(null);
        }
    }

    public class ChoHttpModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
            ChoApplication.Host.OnStopService();
        }

        public void Init(HttpApplication context)
        {
            ChoApplication.Host.OnStartService(null);
        }

        #endregion
    }
}
