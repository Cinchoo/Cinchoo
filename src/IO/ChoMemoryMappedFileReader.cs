using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Cinchoo.Core.Threading;

namespace Cinchoo.Core.IO
{
    public enum ChoMsgFormatter { Binary, Xml }

    public static class ChoMemoryMappedFileHelper
    {
        public static string GetName(string name)
        {
            return "{0}_MM".FormatString(name);
        }
    }

    public class ChoMemoryMappedFileReader<T>
    {
        private readonly string _name;
        private readonly ChoMsgFormatter _msgFormatter;
        private MemoryMappedFile mf;
        private readonly EventWaitHandle _event;

        public ChoMemoryMappedFileReader(string name, ChoMsgFormatter msgFormatter = ChoMsgFormatter.Binary)
        {
            ChoGuard.ArgumentNotNullOrEmpty(name, "Name");

            _name = name;
            _msgFormatter = msgFormatter;
            mf = MemoryMappedFile.OpenExisting(_name, MemoryMappedFileRights.Read);
            _event = new EventWaitHandle(false, EventResetMode.AutoReset, ChoMutexHelper.GetName(_name));
        }

        public T GetData()
        {
            try
            {
                if (!_event.WaitOne(5000))
                    return default(T);

                using (MemoryMappedViewStream stream = mf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read))
                {
                    if (_msgFormatter == ChoMsgFormatter.Binary)
                        return ChoObject.Deserialize<T>(stream);
                    else
                        return ChoObject.XmlDeserialize<T>(stream);
                }
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return default(T);
            }
            catch (SerializationException)
            {
                return default(T);
            }
            catch (AbandonedMutexException)
            {
                return default(T);
            }
        }
    }
}
