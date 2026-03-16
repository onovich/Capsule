using System;

namespace MortiseFrame.Capsule {

    internal static class Crc32Helper {

        static readonly uint[] table = BuildTable();

        static uint[] BuildTable() {
            var t = new uint[256];
            for (uint i = 0; i < 256; i++) {
                uint c = i;
                for (int j = 0; j < 8; j++) {
                    c = (c & 1) != 0 ? (0xEDB88320u ^ (c >> 1)) : (c >> 1);
                }
                t[i] = c;
            }
            return t;
        }

        internal static uint Compute(byte[] data, int offset, int length) {
            uint crc = 0xFFFFFFFFu;
            for (int i = offset; i < offset + length; i++) {
                crc = table[(crc ^ data[i]) & 0xFF] ^ (crc >> 8);
            }
            return crc ^ 0xFFFFFFFFu;
        }

        internal static void Append(byte[] buffer, int dataLength, uint crc) {
            buffer[dataLength]     = (byte)(crc & 0xFF);
            buffer[dataLength + 1] = (byte)((crc >> 8) & 0xFF);
            buffer[dataLength + 2] = (byte)((crc >> 16) & 0xFF);
            buffer[dataLength + 3] = (byte)((crc >> 24) & 0xFF);
        }

        internal static uint Read(byte[] buffer, int dataLength) {
            return (uint)(buffer[dataLength]
                | (buffer[dataLength + 1] << 8)
                | (buffer[dataLength + 2] << 16)
                | (buffer[dataLength + 3] << 24));
        }

    }

}
