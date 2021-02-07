using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cinchoo.Core.Threading;

namespace Cinchoo.Core.IO
{
    public class ChoMemoryMappedFileWriter<T> : ChoDisposableObject
    {
        private readonly string _name;
        private readonly ChoMsgFormatter _msgFormatter;
        private readonly MemoryMappedFile mf = null;
        private readonly EventWaitHandle _event;

        public ChoMemoryMappedFileWriter(string name, ChoMsgFormatter msgFormatter = ChoMsgFormatter.Binary)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _msgFormatter = msgFormatter;

            mf = MemoryMappedFile.CreateOrOpen(_name, 4096, MemoryMappedFileAccess.ReadWrite);
            _event = new EventWaitHandle(true, EventResetMode.AutoReset, ChoMutexHelper.GetName(_name));
        }

        public void WriteData(T target)
        {
            ChoGuard.ArgumentNotNullOrEmpty(target, "Target");
            ChoGuard.NotDisposed(this);

            //using (Mutex mutex = new Mutex(false, ChoMutexHelper.GetName(_name)))
            //{
            //    mutex.WaitOne();
                using (MemoryMappedViewStream stream = mf.CreateViewStream())
                {
                    if (_msgFormatter == ChoMsgFormatter.Binary)
                        ChoObject.Serialize(stream, target);
                    else
                        ChoObject.XmlSerialize(stream, target);
                }
                _event.Set();
            //}
        }

        protected override void Dispose(bool finalize)
        {
        }
    }
}
