using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet
{
    public interface IBuffer
    {
        int Offset { get; }
        int Size { get; }
        int Capacity { get; }
        byte[] Bytes { get; }

        byte GetByte(int pos);
        short GetShort(int pos, ByteOrder endian = ByteOrder.BigEndian);
        int GetInt(int pos, ByteOrder endian = ByteOrder.BigEndian);
        long GetLong(int pos, ByteOrder endian = ByteOrder.BigEndian);
        string GetString(int pos, int length, ByteOrder endian = ByteOrder.BigEndian);
        string GetString(int pos, int length, Encoding encoding, ByteOrder endian = ByteOrder.BigEndian);

        IBuffer SetByte(int pos, byte value);
        IBuffer SetShort(int pos, short value, ByteOrder endian = ByteOrder.BigEndian);
        IBuffer SetInt(int pos, int value, ByteOrder endian = ByteOrder.BigEndian);
        IBuffer SetLong(int pos, long value, ByteOrder endian = ByteOrder.BigEndian);
        IBuffer SetString(int pos, string value, ByteOrder endian = ByteOrder.BigEndian);
        IBuffer SetString(int pos, string value, Encoding encoding, ByteOrder endian = ByteOrder.BigEndian);

        void Reset();

        string DumpHex(int lengthLimit);
    }
}
