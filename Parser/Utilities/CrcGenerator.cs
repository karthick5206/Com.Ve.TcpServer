using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Ve.Parser.Utilities
{
    public static class CrcGenerator
    {
        static public UInt16 crc_bytes(byte[] data)
        {
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(Reflect(data[i], 8) << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            crc = Reflect(crc, 16);
            crc = (ushort)~crc;
            return crc;
        }
        static public ushort Reflect(ushort data, int size)
        {
            ushort output = 0;
            for (int i = 0; i < size; i++)
            {
                int lsb = data & 0x01;
                output = (ushort)((output << 1) | lsb);
                data >>= 1;
            }
            return output;
        }
    }
}
