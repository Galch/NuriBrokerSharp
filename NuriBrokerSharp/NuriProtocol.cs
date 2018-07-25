using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuriBrokerSharp
{
    public enum NuriMode
    {
        NURI_SET_POS_VEL = 0x01,
        NURI_SET_POS_RAMP,
        NURI_SET_VEL_RAMP,
        NURI_SET_POS_PID,
        NURI_SET_VEL_PID,
        NURI_SET_INFO,
        NURI_FACTORY_RESET,
    }
    public static class NuriProtocol
    {

        public static byte HEADER_1 = 0xFF;
        public static byte HEADER_2 = 0xFE;

        public static int GetDataSize(NuriMode m)
        {
            switch (m)
            {
                case NuriMode.NURI_SET_POS_VEL:
                    return 0x06;
                case NuriMode.NURI_SET_POS_RAMP:
                    return 0x05;
                case NuriMode.NURI_SET_VEL_RAMP:
                    return 0x04;
                case NuriMode.NURI_SET_POS_PID:
                    return 0x04;
                case NuriMode.NURI_SET_VEL_PID:
                    return 0x04;
                case NuriMode.NURI_SET_INFO:
                    return 0x05;
                case NuriMode.NURI_FACTORY_RESET:
                    return 0x00;
            }

            return 0;
        }
        public static int GetParamCount(NuriMode m)
        {
            switch (m)
            {
                case NuriMode.NURI_SET_POS_VEL:
                    return 3;
                case NuriMode.NURI_SET_POS_RAMP:
                    return 3;
                case NuriMode.NURI_SET_VEL_RAMP:
                    return 3;
                case NuriMode.NURI_SET_POS_PID:
                    return 4;
                case NuriMode.NURI_SET_VEL_PID:
                    return 4;
                case NuriMode.NURI_SET_INFO:
                    return 5;
                case NuriMode.NURI_FACTORY_RESET:
                    return 0;
            }

            return 0;
        }

        
        public static byte[] GetPacket(int id, NuriMode mode, params double[] numbers)
        {
            int dataSize = GetDataSize(mode);
            int paramCount = GetParamCount(mode);

            List<byte> rawByteList = new List<byte>();

            rawByteList.Add((byte)id);
            rawByteList.Add((byte)mode);
            rawByteList.Add((byte)dataSize);

            if (numbers.Length != paramCount)
            {
                Console.WriteLine("Data Count is Wrong required : {0} / input : {1}", paramCount, numbers.Length);
                return null;
            }


            switch (mode)
            {
                case NuriMode.NURI_SET_POS_VEL:
                    {
                        int direction = (int)numbers[0];
                        int pos = Math.Min((int)numbers[1], 65533);
                        int pos_frac = (int)(numbers[1] * 100) % 100;
                        int vel = Math.Min((int)numbers[2], 30);
                        int vel_frac = (int)(numbers[2] * 100) % 100;

                        rawByteList.Add((byte)direction);  // Direction 0 : Right, 1 : Left

                        rawByteList.Add((byte)((pos >> 8) & 0xFF));
                        rawByteList.Add((byte)(pos & 0xFF));
                        rawByteList.Add((byte)pos_frac);

                        rawByteList.Add((byte)((vel >> 8) & 0xFF));
                        rawByteList.Add((byte)(vel & 0xFF));
                        rawByteList.Add((byte)vel_frac);
                        break;
                    }
                case NuriMode.NURI_SET_POS_RAMP:
                    {
                        int direction = (int)numbers[0];
                        int pos = Math.Min((int)numbers[1], 65533);     // *[1 Deg]
                        int pos_frac = (int)(numbers[1] * 100) % 100;   // *[0.01 Deg]
                        int time = Math.Min((int)numbers[2], 250);  // *[0.1s]

                        rawByteList.Add((byte)direction);  // Direction 0 : Right, 1 : Left

                        rawByteList.Add((byte)((pos >> 8) & 0xFF));
                        rawByteList.Add((byte)(pos & 0xFF));
                        rawByteList.Add((byte)pos_frac);
                        
                        rawByteList.Add((byte)time);
                        break;
                    }
                case NuriMode.NURI_SET_VEL_RAMP:
                    {
                        int direction = (int)numbers[0];
                        int vel = Math.Min((int)numbers[1], 30);     // *[1 RPM]
                        int vel_frac = (int)(numbers[1] * 100) % 100;   // *[0.01 RPM]
                        int time = Math.Min((int)numbers[2], 250);  // *[0.1s]

                        rawByteList.Add((byte)direction);  // Direction 0 : Right, 1 : Left

                        rawByteList.Add((byte)((vel >> 8) & 0xFF));
                        rawByteList.Add((byte)(vel & 0xFF));
                        rawByteList.Add((byte)vel_frac);

                        rawByteList.Add((byte)time);
                        break;
                    }
                case NuriMode.NURI_SET_POS_PID:
                    {
                        int Kp = Math.Min((int)numbers[0], 250);
                        int Ki = Math.Min((int)numbers[1], 250);
                        int Kd = Math.Min((int)numbers[2], 250);
                        int Imax = Math.Min((int)numbers[3], 80);   // *[100mA]

                        rawByteList.Add((byte)Kp);
                        rawByteList.Add((byte)Ki);
                        rawByteList.Add((byte)Kd);
                        rawByteList.Add((byte)Imax);
                        break;
                    }
                case NuriMode.NURI_SET_VEL_PID:
                    {
                        int Kp = Math.Min((int)numbers[0], 250);
                        int Ki = Math.Min((int)numbers[1], 250);
                        int Kd = Math.Min((int)numbers[2], 250);
                        int Imax = Math.Min((int)numbers[3], 80);   // *[100mA]

                        rawByteList.Add((byte)Kp);
                        rawByteList.Add((byte)Ki);
                        rawByteList.Add((byte)Kd);
                        rawByteList.Add((byte)Imax);
                        break;
                    }
                case NuriMode.NURI_SET_INFO:
                    {
                        int deviceID = Math.Min((int)numbers[0], 253);            
                        int baud = Math.Min((int)numbers[1], 10);           // 0x0A = 115200
                        int encReset = Math.Min((int)numbers[2], 1);        // 1: Reset
                        int ctrlOnOff = Math.Min((int)numbers[3], 1);       // 0:on , 1:off
                        int responseTime = Math.Min((int)numbers[4], 250);  // *[100us]

                        rawByteList.Add((byte)deviceID);
                        rawByteList.Add((byte)baud);
                        rawByteList.Add((byte)encReset);
                        rawByteList.Add((byte)ctrlOnOff);
                        rawByteList.Add((byte)responseTime);
                        break;
                    }
            }


            return PacketFinishing(rawByteList.ToArray());
        }

        public static byte[] PacketFinishing(byte[] RawData)
        {
            byte checksum = 0;

            foreach (byte b in RawData)
            {
                checksum += b;
            }

            checksum = (byte)~(checksum);

            byte[] packet = new byte[RawData.Length + 3];
            Buffer.BlockCopy(RawData, 0, packet, 2, RawData.Length);

            packet[0] = HEADER_1;
            packet[1] = HEADER_2;
            packet[RawData.Length + 2] = checksum;

            return packet;
        }
    }
}
