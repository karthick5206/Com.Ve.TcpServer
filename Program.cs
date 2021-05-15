using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Com.Ve.Parser;
using Com.Ve.Parser.Utilities;
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
                TcpDataReceiver tcpServer = new TcpDataReceiver(parser);
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
        public ParserDataType ParserDataType { get; set; }
        public int Port { get; set; }
        public string RedirectIp { get; set; }
        public int RedirectPort { get; set; }
        public bool IsAknowledge { get; set; }
    }

    public enum ParserDataType
    {
        String = 0,
        Hex
    }

    public enum ParserType
    {
        Concox = 0,
        Teltonika,
        ITraingle
    }

    public class TcpDataReceiver : TcpServer
    {
        Parser ParserInfo { get; set; }
        public string IMEI { get; set; }
        public TcpDataReceiver(Parser parser) : base(IPAddress.Any, parser.Port)
        {
            ParserInfo = parser;
        }

        public override void OnReceived(TcpSession tcpSession, byte[] buffer, long offset, long size)
        {
            var receivedDataByteArray = new byte[size];
            Array.Copy(buffer, receivedDataByteArray, size);
            var receivedData = ParserInfo.ParserDataType == ParserDataType.Hex
                ? SplitData.GetHexValueFromByteArray(receivedDataByteArray)
                : Encoding.UTF8.GetString(receivedDataByteArray);
            Console.WriteLine($"{receivedData}");
            if (ParserInfo.IsAknowledge)
            {
                SendAcknowledgeMent(tcpSession, receivedData);
            }
            RavenDB.RavenDbConnector.Add(new RavenDB.GpsData { Data = receivedData });
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }

        public void SendAcknowledgeMent(TcpSession tcpSession, string message)
        {
            switch (ParserInfo.ParserType)
            {
                case ParserType.Concox:
                    SendAcknowledgeMentToDevice(tcpSession, ConcoxParser.BuildAcknowledgeMessage(message, out string IMEI));
                    this.IMEI = IMEI;
                    break;
                case ParserType.Teltonika:
                    SendAcknowledgeMentToDevice(tcpSession, TeltonikaParser.BuildAcknowledgeMessage(message));
                    break;
            }
        }

        public void SendAcknowledgeMentToDevice(TcpSession tcpSession, string msg)
        {
            try
            {
                Console.WriteLine("Acknowledgement message  : " + msg);
                if (tcpSession != null)
                {
                    tcpSession.Send(SplitData.StringToByteArray(msg.Trim()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"public void SendAcknowledgeMent(Socket _socket) {ex}");
            }
        }
    }
}
