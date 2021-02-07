using System;
namespace Cinchoo.Core.Net.Sockets
{
    interface IChoNetClient
    {
        bool IsServer { get; }
        bool IsServerAvailable { get; }
        event EventHandler<global::Cinchoo.Core.Net.Sockets.ChoNetMessageEventArgs> MessageReceived;
        event EventHandler<global::Cinchoo.Core.Net.Sockets.ChoNetMessageEventArgs> RawMessageReceived;
        int Send(bool value);
        int Send(byte[] value);
        int Send(char value);
        int Send(double value);
        int Send(short value);
        int Send(int value);
        int Send(long value);
        int Send(object value);
        int Send(float value);
        int Send(string value);
        int Send(ushort value);
        int Send(uint value);
        int Send(ulong value);
        void StartAsServer();
    }
}
