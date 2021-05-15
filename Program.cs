using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using Newtonsoft.Json;

namespace Com.Ve.ServerDataReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsers = JsonConvert.DeserializeObject<ParserList>(File.ReadAllText($@"{Directory.GetCurrentDirectory()}/Config/ParsersList.json"));
            Console.WriteLine($"Parsers Count :{parsers.Parser.Count}");

            foreach (var parser in parsers.Parser)
            {
                TcpDataReceiver tcpServer = new TcpDataReceiver(IPAddress.Any, parser.Port);
                tcpServer.Start();
                tcpServer.OptionReuseAddress = true;

                Console.WriteLine($"Tcp server started Ip:{IPAddress.Any} Port:{parser.Port}");
            }

            Console.Read();
        }
    }

    public class ParserList
    {
        public List<Parser> Parser { get; set; }
    }

    public class Parser
    {
        public string ParserName { get; set; }
        public ParserType ParserType { get; set; }
        public int Port { get; set; }
        public string RedirectIp { get; set; }
        public int RedirectPort { get; set; }
    }

    public enum ParserType
    {
        String = 0,
        Hex
    }

    public class TcpDataReceiver : TcpServer
    {
        public TcpDataReceiver(IPAddress iPAddress, int port) : base(iPAddress, port)
        {

        }

        public override void OnReceived(TcpSession tcpSession, byte[] buffer, long offset, long size)
        {
            var receivedDataByteArray = new byte[size];
            Array.Copy(buffer, receivedDataByteArray, size);
            var receivedData = Encoding.UTF8.GetString(receivedDataByteArray);
            Console.WriteLine($"{receivedData}");
            RavenDB.RavenDbConnector.Add(new RavenDB.GpsData { Data = receivedData });
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }
}
