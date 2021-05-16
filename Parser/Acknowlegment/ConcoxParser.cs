using Com.Ve.Parser.Utilities;
using System;
using System.Linq;
using System.Text;

namespace Com.Ve.Parser
{
    public static class ConcoxParser
    {
        static byte[] heartbeatResponse = { 0x78, 0x78, 0x05, 0x13, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x0A };
        static byte[] loginResponse = { 0x78, 0x78, 0x05, 0x01, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x0A };
        static byte[] alarmResponse = { 0x78, 0x78, 0x05, 0x16, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x0A };
        public static string BuildAcknowledgeMessage(string message, out string IMEI)
        {
            IMEI = string.Empty;
            string acknowledge = string.Empty;
            try
            {
                string packetType = message.Substring(6, 2);

                byte[] receivedMessageArray = SplitData.StringToByteArray(message);

                int messageLength = receivedMessageArray[2];
                var serialNumber = receivedMessageArray.Skip(2 + 1 + messageLength - 4).Take(2).ToArray();

                switch (packetType)
                {
                    case "01":
                        serialNumber.CopyTo(loginResponse, 4);

                        var sendCRC = CrcGenerator.crc_bytes(loginResponse.Skip(2).Take(loginResponse.Length - 6).ToArray());

                        loginResponse[loginResponse.Length - 4] = (byte)((sendCRC >> 8) & 0xFF);
                        loginResponse[loginResponse.Length - 3] = (byte)((sendCRC) & 0xFF);

                        IMEI = Encoding.ASCII.GetString(receivedMessageArray.Skip(4).Take(messageLength - 5).ToArray());

                        acknowledge = SplitData.GetHexValueFromByteArray(loginResponse);

                        Console.WriteLine("Imei : '{0}'", IMEI);

                        Console.WriteLine("Acknowledge Location Data : '{0}'", acknowledge);

                        break;

                    case "13":
                        serialNumber.CopyTo(heartbeatResponse, 4);
                        sendCRC = CrcGenerator.crc_bytes(heartbeatResponse.Skip(2).Take(heartbeatResponse.Length - 6).ToArray());


                        heartbeatResponse[heartbeatResponse.Length - 4] = (byte)((sendCRC >> 8) & 0xFF);
                        heartbeatResponse[heartbeatResponse.Length - 3] = (byte)((sendCRC) & 0xFF);

                        acknowledge = SplitData.GetHexValueFromByteArray(heartbeatResponse);

                        Console.WriteLine("Send Message : '{0}'", acknowledge);                        

                        break;

                    case "16":
                        int alarmPacketLen = alarmResponse.Length - 5;
                        alarmResponse[2] = (byte)(alarmPacketLen & 0xFF);

                        serialNumber.CopyTo(alarmResponse, alarmPacketLen - 1);

                        sendCRC = CrcGenerator.crc_bytes(alarmResponse.Skip(2).Take(alarmPacketLen - 1).ToArray());

                        alarmResponse[alarmPacketLen + 1] = (byte)((sendCRC >> 8) & 0xFF);
                        alarmResponse[alarmPacketLen + 2] = (byte)((sendCRC) & 0xFF);

                        acknowledge = SplitData.GetHexValueFromByteArray(alarmResponse);

                        Console.WriteLine("Acknowledge Location Data : '{0}'", acknowledge);
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return acknowledge;
        }
    }
}
