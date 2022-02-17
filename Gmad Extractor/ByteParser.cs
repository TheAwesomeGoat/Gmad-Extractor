using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmad_Extractor
{
    class ByteParser
    {
        private readonly byte[] _bytes;

        public ByteParser(byte[] bytes)
        {
            _bytes = bytes;
        }

        public byte GetByte()
        {
            return _bytes[CurrentPosition++];
        }

        public bool BytesLeft
        {
            get { return _bytes.Length - 1 >= CurrentPosition; }
        }

        public string GetStringToTermination()
        {
            var buffer = new List<byte>();
            for (; CurrentPosition < _bytes.Length; CurrentPosition++)
            {
                var b = _bytes[CurrentPosition];
                if (b != 0x00)
                {
                    buffer.Add(b);
                }
                else
                {
                    CurrentPosition++;
                    break;
                }
            }
            return Encoding.UTF8.GetString(buffer.ToArray());
        }

        public float GetFloat()
        {
            return BitConverter.ToSingle(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }

        public ushort GetUShort()
        {
            return BitConverter.ToUInt16(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }
        public uint GetUInt()
        {
            return BitConverter.ToUInt32(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }
        public ulong GetULong()
        {
            return BitConverter.ToUInt64(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }
        public short GetShort()
        {
            return BitConverter.ToInt16(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }
        public int GetInt()
        {
            return BitConverter.ToInt32(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }
        public long GetLong()
        {
            return BitConverter.ToInt64(new[]
            {
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++],
                _bytes[CurrentPosition++]
            }, 0);
        }

        public char GetChar()
        {
            return (char)_bytes[CurrentPosition++];
        }
        public string GetStringOfByte()
        {
            return Encoding.ASCII.GetString(new[] { _bytes[CurrentPosition++] });
        }

        public int CurrentPosition { get; set; }

    }
}
