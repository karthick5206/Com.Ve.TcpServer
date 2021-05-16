using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.ServerDataReceiver.DataForwarder
{
    public class TcpForwarder : TcpClient
    {
        public TcpForwarder(System.Net.IPAddress iPAddress, int port) : base(iPAddress, port)
        {
            IPAddress = iPAddress;
            Port = port;
            base.Connect();
        }

        public void ForwardData(byte[] data)
        {
            if (base.IsConnected)
                base.Send(data);
            else
            {
                base.Connect();
                base.Send(data);
            }
        }

        public System.Net.IPAddress IPAddress { get; }
        public int Port { get; }


        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            base.OnError(error);
            Console.WriteLine(error.ToString());
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            base.Connect();
        }
    }
}
