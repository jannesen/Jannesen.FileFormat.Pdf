using System;
using System.IO;

#pragma warning disable CA2213 // Disposable fields should be disposed

namespace Jannesen.FileFormat.Pdf.Internal
{
    public class PdfDataStreamReader: Stream
    {
        private readonly        Stream                  _stream;
        private readonly        int                     _startPosition;
        private readonly        int                     _length;
        private                 int                     _position;

        internal                                        PdfDataStreamReader(Stream stream, int startPosition, int length)
        {
            _stream        = stream;
            _startPosition = startPosition;
            _length        = length;
            _position      = 0;
        }

        public override             bool                CanRead             { get { return true;        } }
        public override             bool                CanSeek             { get { return true;        } }
        public override             bool                CanWrite            { get { return false;       } }
        public override             long                Length              { get { return _length;     } }
        public override             long                Position
        {
            get {
                return _position;
            }
            set {
                if (value < 0)          value = 0;
                if (value > _length)    value = _length;

                _position = (int)value;
            }
        }
        public override             void                Flush()             { }
        public override             long                Seek(long Offset, SeekOrigin Origin)
        {
            long    newPosition;

            switch(Origin) {
            case SeekOrigin.Begin:      newPosition = Offset;               break;
            case SeekOrigin.Current:    newPosition = _position + Offset;   break;
            case SeekOrigin.End:        newPosition = _length + Offset;     break;
            default:                    newPosition = _position;            break;
            }

            if (newPosition < 0)        newPosition = 0;
            if (newPosition > _length)  newPosition = _length;

            _position = (int)newPosition;

            return newPosition;
        }
        public override             void                SetLength(long Length)
        {
            throw new NotSupportedException("Can't set length of a readonly stream.");
        }
        public override             int                 Read(byte[] buf, int offset, int size)
        {
            if (size > _length - _position)
                size = (int)(_length - _position);

            if (size <= 0)
                return 0;

            _stream.Position = _startPosition + _position;
            int readSize = _stream.Read(buf, offset, size);
            _position += readSize;

            if (readSize < size)
                throw new PdfExceptionReader("EOF read on pdf stream.");

            return readSize;
        }
        public override             int                 ReadByte()
        {
            if (1 > _length - _position)
                return -1;

            _stream.Position = _startPosition + _position;
            int b = _stream.ReadByte();
            ++_position;

            return b;
        }
        public override             void                Write(byte[] Buf, int Offset, int Size)
        {
            throw new NotSupportedException("Can't write to a readonly stream.");
        }
        public override             void                WriteByte(byte b)
        {
            throw new NotSupportedException("Can't write to a readonly stream.");
        }
    }
}
