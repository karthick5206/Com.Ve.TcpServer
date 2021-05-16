using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.Parser.Utilities
{
    public static class SplitData
    {
        public static string ReadData(string hexMessage, int start, int length, int dataType)
        {
            string value = string.Empty;
            string covertType = string.Empty;
            try
            {
                if (hexMessage.Length > start)
                {
                    switch (dataType)
                    {
                        case 1:
                            covertType = "Decimal";
                            value = hexMessage.Substring(start, length);
                            break;
                        case 2:
                            value = hexMessage.Substring(start, length);
                            value = ReverseBytes(Convert.ToUInt16(value, 8)).ToString();
                            break;
                        case 9:

                            value = hexMessage.Substring(start, length);
                            value = GetDecimalValueFromHexString(value).ToString();
                            if (int.Parse(value) > short.MaxValue)
                            {
                            a: int v = int.Parse(value) - (short.MaxValue);
                                if (v > short.MaxValue)
                                {
                                    value = v.ToString();
                                    goto a;
                                }
                                byte[] bt = BitConverter.GetBytes(short.Parse(v.ToString()));
                                Array.Reverse(bt);
                                value = BitConverter.ToInt16(bt, 0).ToString();
                            }
                            else
                            {
                                byte[] bt = BitConverter.GetBytes(short.Parse(value));
                                Array.Reverse(bt);
                                value = BitConverter.ToInt16(bt, 0).ToString();
                            }

                            break;
                        case 3:
                            value = hexMessage.Substring(start, length);
                            break;
                        case 4:
                            value = hexMessage.Substring(start, length);
                            value = ReverseBytes(Convert.ToUInt32(value, 16)).ToString();
                            break;

                        case 5:
                            value = hexMessage.Substring(start, length);
                            value = ReverseBytes(Convert.ToUInt64(value, 32)).ToString();
                            break;

                        case 7:
                            //value = hexMessage.Substring(start, length);
                            int end = 0;
                            covertType = "Decimal";
                            end = start + 2;

                            value = hexMessage.Substring(end + 4, 2);
                            // k = up;

                            value += hexMessage.Substring(end + 2, 2);
                            //k = k + "" + up;

                            value += hexMessage.Substring(end, 2);
                            // k = k + "" + up;

                            value += hexMessage.Substring(start, 2);
                            // k = k + "" + up;
                            break;
                        case 6:
                            //value = hexMessage.Substring(start, length);
                            end = 0;
                            covertType = "Decimal";
                            end = start + 2;

                            //value = hexMessage.Substring(end + 4, 2);
                            // k = up;

                            value += hexMessage.Substring(end + 2, 2);
                            //k = k + "" + up;

                            value += hexMessage.Substring(end, 2);
                            // k = k + "" + up;

                            value += hexMessage.Substring(start, 2);
                            // k = k + "" + up;
                            break;
                        case 8:
                            //value = hexMessage.Substring(start, length);
                            end = 0;
                            covertType = "Decimal";
                            end = start + 2;

                            //value = hexMessage.Substring(end + 4, 2);
                            // k = up;

                            //value += hexMessage.Substring(end + 2, 2);
                            //k = k + "" + up;

                            value += hexMessage.Substring(end, 2);
                            // k = k + "" + up;

                            value += hexMessage.Substring(start, 2);
                            // k = k + "" + up;
                            break;
                    }

                    if (covertType == "Decimal")
                    {
                        value = GetDecimalValueFromHexString(value).ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"public string ReadData(string hexMessage, int start, int end, int dataType) {ex}");
            }
            return value;
        }

        public static string GetHexValueFromByteArray(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            try
            {
                foreach (byte b in ba)
                {
                    hex.AppendFormat("{0:X2}", b);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetHexValueFromByteArray(byte[] ba) {ex}");
            }
            return hex.ToString();
        }
        private static Decimal GetDecimalValueFromHexString(string hexNumber)
        {
            hexNumber = hexNumber.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexNumber, System.Globalization.NumberStyles.HexNumber, null, out result);
            return result;
        }

        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
