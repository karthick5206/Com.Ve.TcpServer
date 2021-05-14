using System;
using System.Net;
using System.Text;
using NetCoreServer;

namespace Com.Ve.ServerDataReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpDataReceiver tcpServer = new TcpDataReceiver(IPAddress.Any, 180);
            tcpServer.Start();

            Console.Read();
        }
    }

    public class TcpDataReceiver : TcpServer
    {
        public TcpDataReceiver(IPAddress iPAddress, int port) : base(iPAddress, port)
        {

        }

        public override void OnReceived(byte[] buffer, long offset, long size)
        {
            var receivedDataByteArray = new byte[size];
            Array.Copy(buffer, receivedDataByteArray, size);
            Console.WriteLine($"{Encoding.UTF8.GetString(receivedDataByteArray)}");
        }
    }
}
