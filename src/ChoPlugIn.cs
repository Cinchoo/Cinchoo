using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinchoo.Core.Diagnostics;
using System.Threading.Tasks;

namespace Cinchoo.Core
{
    public class ChoPlugIns<T>
    {
        private readonly List<IChoPlugIn<T>> _plugInsObjs = new List<IChoPlugIn<T>>();

        public ChoPlugIns()
        {
        }

        public void Load(string types)
        {
            Load(new string[] { types });
        }

        public void Load(string[] types)
        {
            if (types == null) return;

            foreach (string typeString in types)
            {
                if (typeString.IsNullOrWhiteSpace()) continue;

                foreach (string typeText in typeString.SplitNTrim())
                {
                    try
                    {
                        Type type = ChoType.GetType(typeText);
                        if (type == null) continue;

                        IChoPlugIn<T> plugIn = ChoActivator.CreateInstance<IChoPlugIn<T>>();
                        if (plugIn != null)
                            _plugInsObjs.Add(plugIn);
                    }
                    catch (Exception ex)
                    {
                        ChoTrace.Write(ex);
                    }
                }
            }
        }
        
        public void Load(Type type)
        {
            Load(new Type[] { type });
        }

        public void Load(Type[] types)
        {
            if (types == null) return;

            foreach (Type type in types)
            {
                if (type == null) continue;
                try
                {
                    IChoPlugIn<T> plugIn = ChoActivator.CreateInstance<IChoPlugIn<T>>();
                    if (plugIn != null)
                        _plugInsObjs.Add(plugIn);
                }
                catch (Exception ex)
                {
                    ChoTrace.Write(ex);
                }
            }
        }

        public void Invoke(T value)
        {
            foreach (IChoPlugIn<T> plugIn in _plugInsObjs)
                plugIn.Invoke(value);
        }

        public void InvokeAsync(T value)
        {
            Task.Factory.StartNew(() =>
            {
                Invoke(value);
            });
        }
    }
}
