using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Util
{
    public class StringStream : Stream
    {
        private readonly MemoryStream _memory;
        public StringStream(string text)
        {
            _memory = new MemoryStream(Encoding.UTF8.GetBytes(text));
        }
        public StringStream()
        {
            _memory = new MemoryStream();
        }
        public StringStream(int capacity)
        {
            _memory = new MemoryStream(capacity);
        }
        public override void Flush()
        {
            _memory.Flush();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _memory.Read(buffer, offset, count);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _memory.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _memory.SetLength(value);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            _memory.Write(buffer, offset, count);
            return;
        }
        public override bool CanRead => _memory.CanRead;
        public override bool CanSeek => _memory.CanSeek;
        public override bool CanWrite => _memory.CanWrite;
        public override long Length => _memory.Length;
        public override long Position
        {
            get => _memory.Position;
            set => _memory.Position = value;
        }
        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(_memory.GetBuffer(), 0, (int)_memory.Length);
        }
        public override int ReadByte()
        {
            return _memory.ReadByte();
        }
        public override void WriteByte(byte value)
        {
            _memory.WriteByte(value);
        }
    }
}
