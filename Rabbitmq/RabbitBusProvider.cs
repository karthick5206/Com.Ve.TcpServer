using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.Rabbitmq
{
    public static class RabbitBusProvider
    {
        private static IBus Bus { get; set; }
        public static IBus InitializeBus(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = "host=localhost;username=vibhav;password=server@vibhav1";

            return RabbitHutch.CreateBus(connectionString);
        }

        public static void AddToQueue(GpsData message, string parserType)
        {
            if (Bus == null)
                Bus = InitializeBus(string.Empty);

            Bus.PubSub.Publish<GpsData>(message, parserType);

            Bus.SendReceive.Send("Gps", message, typeof(GpsData));
        }

        public static void Subscribe(string parserType, Action<GpsData> dataReceiver)
        {
            if (Bus == null)
                Bus = InitializeBus(string.Empty);

            Bus.SendReceive.Receive("Gps", dataReceiver);

            Bus.PubSub.Subscribe<GpsData>(parserType, dataReceiver);
        }      
    }

    [Queue("Gps")]
    public class GpsData
    {
        public string Message { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string DeviceName { get; internal set; }
    }
}
