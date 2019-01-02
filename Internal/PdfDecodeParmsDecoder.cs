using System;
using System.IO;

namespace Jannesen.FileFormat.Pdf.Internal
{
    class PdfDecodeParmsDecoder: Stream
    {
        private                 Stream          _inputStream;
        private                 int             _predictor;
        private                 int             _columns;
        private                 int             _colors;
        private                 int             _bpc;
        private                 int             _bytesPerPixel;
        private                 int             _bytesPerRow;
        private                 byte[]          _prior;
        private                 byte[]          _curr;
        private                 int             _curr_pos;

        public      override    long            Position    { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public      override    long            Length      { get => throw new NotSupportedException(); }
        public      override    bool            CanWrite    { get => false; }
        public      override    bool            CanSeek     { get => false; }
        public      override    bool            CanRead     { get => true; }

        public                                  PdfDecodeParmsDecoder(PdfDictionary decodeParms, Stream inputStream)
        {
            PdfInteger  v;

            _inputStream = inputStream;

            _predictor = decodeParms.ValueByName<PdfInteger>("Predictor", resolve:false).Value;

            v = decodeParms.ValueByName<PdfInteger>("Columns", mandatory:false, resolve:false);
            _columns = (v != null) ? v.Value : 1;

            v = decodeParms.ValueByName<PdfInteger>("Colors", mandatory:false, resolve:false);
            _colors = (v != null) ? v.Value : 1;

            v = decodeParms.ValueByName<PdfInteger>("BitsPerComponent", mandatory:false, resolve:false);
            _bpc = (v != null) ? v.Value : 8;

            if (!(10 <=_predictor && _predictor <= 15))
                throw new NotImplementedException("PdfDecodeParmsDecoder Predictor=" + _predictor + " not implemented.");

            _bytesPerPixel = _colors * _bpc / 8;
            _bytesPerRow   = (_colors * _columns * _bpc + 7)/8;
            _curr  = new byte[_bytesPerRow];
            _prior = new byte[_bytesPerRow];
            _curr_pos = _curr.Length;
        }
        protected   override    void            Dispose(bool disposing)
        {
            if (disposing) {
                _inputStream.Dispose();
            }
        }
        public      override    void            Flush()
        {
        }
        public      override    int             Read(byte[] buffer, int offset, int count)
        {
            int     sz = 0;

            while (count > 0) {
                if (_curr_pos >= _curr.Length) {
                    if (!_fill_curr_png())
                        break;
                }

                buffer[offset++] = _curr[_curr_pos++];
                ++sz;
                --count;
            }

            return sz;
        }
        public      override    long            Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }
        public      override    void            SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public      override    void            Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private                 bool            _fill_curr_png()
        {
            int filter = _inputStream.ReadByte();
            if (filter < 0)
                return false;

            Array.Copy(_curr, _prior, _curr.Length);
            int rs = 0;
            while (rs < _bytesPerRow) {
                int n = _inputStream.Read(_curr, rs, _bytesPerRow - rs);
                if (n <= 0)
                    throw new PdfExceptionReader("PdfDecodeParmsDecoder EOF While reading inpiut stream.");
                rs += n;
            }

            switch (filter) {
            case 0: //PNG_FILTER_NONE
                break;

            case 1: //PNG_FILTER_SUB
                for (int i = _bytesPerPixel; i < _bytesPerRow; i++)
                    _curr[i] += _curr[i - _bytesPerPixel];
                break;
            case 2: //PNG_FILTER_UP
                for (int i = 0; i < _bytesPerRow; i++)
                    _curr[i] += _prior[i];
                break;

            case 3: //PNG_FILTER_AVERAGE
                for (int i = 0; i < _bytesPerPixel; i++)
                    _curr[i] += (byte)(_prior[i] / 2);

                for (int i = _bytesPerPixel; i < _bytesPerRow; i++)
                    _curr[i] += (byte)(((_curr[i - _bytesPerPixel] & 0xff) + (_prior[i] & 0xff))/2);
                break;

            case 4: //PNG_FILTER_PAETH
                for (int i = 0; i < _bytesPerPixel; i++) {
                    _curr[i] += _prior[i];
                }

                for (int i = _bytesPerPixel; i < _bytesPerRow; i++) {
                    int a = _curr[i - _bytesPerPixel] & 0xff;
                    int b = _prior[i] & 0xff;
                    int c = _prior[i - _bytesPerPixel] & 0xff;

                    int p = a + b - c;
                    int pa = Math.Abs(p - a);
                    int pb = Math.Abs(p - b);
                    int pc = Math.Abs(p - c);

                    int ret;

                    if ((pa <= pb) && (pa <= pc)) {
                        ret = a;
                    } else if (pb <= pc) {
                        ret = b;
                    } else {
                        ret = c;
                    }
                    _curr[i] += (byte)(ret);
                }
                break;

            default:
                throw new PdfExceptionReader("PdfDecodeParmsDecoder png.filter #" + filter +" unknown.");
            }

            _curr_pos = 0;
            return true;
        }
    }
}
