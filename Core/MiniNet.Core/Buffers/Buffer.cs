using MiniNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Buffers
{
    public class Buffer : IBuffer
    {
        public byte[] Bytes { get; private set; }
        public int Offset { get; private set; }
        public int Size { get; internal set; }
        public int Capacity { get; private set; }

        public Buffer(byte[] bytes, int offset, int capacity)
        {
            Ensure.NotNull(bytes, "bytes");
            if (offset + capacity > bytes.Length)
                throw new ArgumentOutOfRangeException("offset", offset, "Offset + Capacity must be less than the buffer length.");

            this.Size = 0;
            this.Bytes = bytes;
            this.Offset = offset;
            this.Capacity = capacity;
        }

        public byte GetByte(int pos)
        {
            var index = pos + Offset;
            return Bytes[index];
        }

        public short GetShort(int pos, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                return (short)(
                        (Bytes[index] << 8) |
                        (Bytes[index + 1] & 0xff));
            }
            else
            {
                return (short)(
                        (Bytes[index + 1] << 8) |
                        (Bytes[index] & 0xff));
            }
        }

        public int GetInt(int pos, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                return
                    (Bytes[index] << 24) |
                    (Bytes[index + 1] << 16) |
                    (Bytes[index + 2] << 8) |
                    (Bytes[index + 3]);
            }
            else
            {
                return
                    (Bytes[index + 3] << 24) |
                    (Bytes[index + 2] << 16) |
                    (Bytes[index + 1] << 8) |
                    (Bytes[index]);
            }
        }

        public long GetLong(int pos, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                return
                    ((long)Bytes[index] << 56) |
                    ((long)Bytes[index + 1] << 48) |
                    ((long)Bytes[index + 2] << 40) |
                    ((long)Bytes[index + 3] << 32) |
                    ((long)Bytes[index + 4] << 24) |
                    ((long)Bytes[index + 5] << 16) |
                    ((long)Bytes[index + 6] << 8) |
                    ((long)Bytes[index + 7]);
            }
            else
            {
                return
                    ((long)Bytes[index + 7] << 56) |
                    ((long)Bytes[index + 6] << 48) |
                    ((long)Bytes[index + 5] << 40) |
                    ((long)Bytes[index + 4] << 32) |
                    ((long)Bytes[index + 3] << 24) |
                    ((long)Bytes[index + 2] << 16) |
                    ((long)Bytes[index + 1] << 8) |
                    ((long)Bytes[index]);
            }
        }

        public string GetString(int pos, int length, ByteOrder endian = ByteOrder.BigEndian)
        {
            return GetString(pos, length, Encoding.Unicode, endian);
        }

        public string GetString(int pos, int length, Encoding encoding, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            return encoding.GetString(Bytes, index, length);
        }

        public IBuffer SetByte(int pos, byte value)
        {
            var index = pos + Offset;
            Bytes[index] = value;
            Size++;
            return this;
        }

        public IBuffer SetShort(int pos, short value, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                Bytes[index] = (byte)((value >> 8) & 0xff);
                Bytes[index + 1] = (byte)(value & 0xff);
            }
            else
            {
                Bytes[index + 1] = (byte)((value >> 8) & 0xff);
                Bytes[index] = (byte)(value & 0xff);
            }
            Size += 2;
            return this;
        }

        public IBuffer SetInt(int pos, int value, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                Bytes[index] = (byte)((value >> 24) & 0xff);
                Bytes[index + 1] = (byte)((value >> 16) & 0xff);
                Bytes[index + 2] = (byte)((value >> 8) & 0xff);
                Bytes[index + 3] = (byte)(value & 0xff);
            }
            else
            {
                Bytes[index + 3] = (byte)((value >> 24) & 0xff);
                Bytes[index + 2] = (byte)((value >> 16) & 0xff);
                Bytes[index + 1] = (byte)((value >> 8) & 0xff);
                Bytes[index] = (byte)(value & 0xff);
            }
            Size += 4;
            return this;
        }

        public IBuffer SetLong(int pos, long value, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            if (endian == ByteOrder.BigEndian)
            {
                Bytes[index] = (byte)(((value) >> 56) & 0xff);
                Bytes[index + 1] = (byte)((value >> 48) & 0xff);
                Bytes[index + 2] = (byte)((value >> 40) & 0xff);
                Bytes[index + 3] = (byte)((value >> 32) & 0xff);
                Bytes[index + 4] = (byte)((value >> 24) & 0xff);
                Bytes[index + 5] = (byte)((value >> 16) & 0xff);
                Bytes[index + 6] = (byte)((value >> 8) & 0xff);
                Bytes[index + 7] = (byte)(value & 0xff);
            }
            else
            {
                Bytes[index + 7] = (byte)((value >> 56) & 0xff);
                Bytes[index + 6] = (byte)((value >> 48) & 0xff);
                Bytes[index + 5] = (byte)((value >> 40) & 0xff);
                Bytes[index + 4] = (byte)((value >> 32) & 0xff);
                Bytes[index + 3] = (byte)((value >> 24) & 0xff);
                Bytes[index + 2] = (byte)((value >> 16) & 0xff);
                Bytes[index + 1] = (byte)((value >> 8) & 0xff);
                Bytes[index] = (byte)(value & 0xff);
            }
            Size += 8;
            return this;
        }

        public IBuffer SetString(int pos, string value, ByteOrder endian = ByteOrder.BigEndian)
        {
            return SetString(pos, value, Encoding.Unicode, endian);
        }

        public IBuffer SetString(int pos, string value, Encoding encoding, ByteOrder endian = ByteOrder.BigEndian)
        {
            var index = pos + Offset;
            var srcBytes = encoding.GetBytes(value);
            System.Buffer.BlockCopy(srcBytes, 0, Bytes, index, srcBytes.Length);
            Size = pos + srcBytes.Length;
            return this;
        }

        public void Reset()
        {
            Size = 0;
        }

        public string DumpHex(int lengthLimit)
        {
            return DumpHex(Bytes, Offset, Size, lengthLimit);
        }

        public static string DumpHex(byte[] bytes, int pos, int size, int lengthLimit)
        {
            // Prepare
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] high = new char[256];
            char[] low = new char[256];

            for (var i = 0; i < 256; i++)
            {
                high[i] = digits[i >> 4];
                low[i] = digits[i & 0x0F];
            }

            // Calculate
            var truncate = size > lengthLimit;
            var sizeLimit = truncate ? lengthLimit : size;

            if (sizeLimit == 0)
                return "empty";

            var sb = new StringBuilder(sizeLimit * 3 + 3);

            // fill the first
            var byteValue = bytes[pos++] & 0xFF;
            sb.Append((char)high[byteValue]);
            sb.Append((char)low[byteValue]);
            sizeLimit--;

            // and the others, too
            for (; sizeLimit > 0; sizeLimit--)
            {
                sb.Append(' ');
                byteValue = bytes[pos++] & 0xFF;
                sb.Append((char)high[byteValue]);
                sb.Append((char)low[byteValue]);
            }

            if (truncate)
                sb.Append("...");

            return sb.ToString();
        }

        public static byte[] Combine(params byte[][] arrays)
        {
            var ret = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                System.Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }

        public static IBuffer Create(int capacity)
        {
            var bytes = new byte[capacity];
            return new Buffer(bytes, 0, capacity);
        }
    }
}
