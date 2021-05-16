using Com.Ve.Parser.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.Parser
{
    public static class TeltonikaParser
    {
        public static string BuildAcknowledgeMessage(string receiveMessage)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("4d43475004");
                //sb.Append(ReceiveMessage.Substring(8, 2));
                sb.Append(receiveMessage.Substring(10, 8));


                for (int k = 11; k <= 15; k++)
                {
                    sb.Append("00");
                }
                sb.Append(receiveMessage.Substring(22, 2));
                for (int k = 17; k <= 27; k++)
                {
                    sb.Append("00");
                }

                int res = 0;
                for (int y = 8; y < sb.ToString().Length; y++)
                {
                    res += Convert.ToInt32(SplitData.ReadData(sb.ToString(), y, 2, 1));
                    y++;
                }

                string Errorchecksumack = res.ToString("x");
                if (Errorchecksumack.Length == 1)
                {
                    Errorchecksumack = "0" + Errorchecksumack;
                }
                int o = Errorchecksumack.Length;

                string cs = Errorchecksumack.Substring(o - 2, 2);
                sb.Append(cs);
            }
            catch (Exception)
            {

                throw;
            }

            return sb.ToString();
        }
    }
}
