using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.Rabbitmq
{
    public static class QueueHandler
    {
        public static void AddToQueue(byte[] message, string parserType)
        {
            RabbitBusProvider.AddToQueue(new GpsData
            {
                DeviceName = parserType,
                CreatedDateTime = DateTime.Now,
                Message = Encoding.UTF8.GetString(message)
            }, parserType);
        }

        public static void AddToQueue(string type, string parserType, byte[] message)
        { 
            RabbitBusProvider.AddToQueue(new GpsData
            {
                DeviceName = parserType,
                CreatedDateTime = DateTime.Now,
                Message = Encoding.UTF8.GetString(message)
            }, parserType);
        }

        public static void AddToSubscriber(string parserType, Action<GpsData> dataReceiver)
        {
            RabbitBusProvider.Subscribe(parserType, dataReceiver);
        }
    }
}
