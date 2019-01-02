using System;
using System.IO;

namespace Jannesen.FileFormat.Pdf.Internal
{
    sealed class PdfXrefStreamReader: IDisposable
    {
        private                 Stream          _inputStream;
        private                 byte[]          _buf;
        private                 int             _buf_pos;
        private                 int             _buf_length;

        public                  bool            EOF
        {
            get {
                _fillbuf();
                return _buf_length == -1;
            }
        }

        public                                  PdfXrefStreamReader(Stream inputStream)
        {
            _inputStream = inputStream;
            _buf = new byte[512];
            _buf_pos    = 0;
            _buf_length = 0;
        }
        public                  void            Dispose()
        {
            _inputStream.Dispose();
        }

        public                  int             ReadValue(int s)
        {
            int r = 0;

            while (s-- > 0)
                r = (r<<8) | _readbyte();

            return r;
        }

        private                 int             _readbyte()
        {
            if (_buf_pos >= _buf_length) {
                if (EOF)
                    throw new PdfExceptionReader("EOF while reading PdfXrefStream.");
            }

            return _buf[_buf_pos ++] & 0xFF;
        }

        private                 void            _fillbuf()
        {
            if (_buf_length >= 0 && _buf_pos >= _buf_length) {
                _buf_length = _inputStream.Read(_buf, 0, _buf.Length);
                _buf_pos = 0;

                if (_buf_length == 0)
                    _buf_length = -1;
            }
        }
    }
}
