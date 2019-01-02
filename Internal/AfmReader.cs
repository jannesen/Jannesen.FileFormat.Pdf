/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections;
using System.IO;

namespace Jannesen.FileFormat.Pdf.Internal
{
#if DEBUG
    public class AfmReader
    {
        private static  readonly    Hashtable               _glyphNameList                  = _LoadGlyphNameList();

        protected           Stream          _stream;
        protected           bool            _atNewLine;
        protected           bool            _atSubKey;
        protected           char[]          _buffer;
        protected           int             _lineNo;

        public              int             LineNo
        {
            get {
                return _lineNo;
            }
        }

        public                              AfmReader(Stream stream)
        {
            _stream    = stream;
            _atNewLine = true;
            _buffer    = new char[512];
            _lineNo    = 1;
        }

        public  static      int             GlyphNameToUnicode(string name)
        {
            object Value = _glyphNameList[name];

            if (Value == null)
                return -1;

            return (int)Value;
        }
        public              string          ReadKey(bool subKey)
        {
            int     c;

            if (subKey) {
                while (!_atNewLine && !_atSubKey)
                    c = _ReadByte();

                if (_atNewLine)
                    return null;

                c = _ReadByte();

                if (c == ' ' || c == '\t' || c == '\r')
                    c = _SkipSpaces();

                if (_atNewLine)
                    return null;

                if (c < 0)
                    throw new PdfException("Missing sub-key.");
            }
            else {
                while (!_atNewLine)
                    c = _ReadByte();

                c = _ReadByte();

                if (c < 0)
                    return null;
            }

            int     pos = 0;

            while (c != -1 && c != ' ' && c != '\t' && c != '\n' && c != '\r') {
                if (pos < _buffer.Length)
                    _buffer[pos++] = (char)c;

                c = _ReadByte();
            }

            return new string(_buffer, 0, pos);
        }
        public              string          ReadString(bool eol)
        {
            int c = _SkipSpaces();
            if (c < 0)
                throw new PdfException("Missing string value.");

            int     pos = 0;

            while (c != -1 && c != '\n' && c != '\r') {
                if ((c == ' ' || c == '\t') && !eol)
                    break;

                if (pos < _buffer.Length)
                    _buffer[pos++] = (char)c;

                c = _ReadByte();
            }

            return new string(_buffer, 0, pos);
        }
        public              int             ReadInteger()
        {
            int c = _SkipSpaces();
            if (c < 0)
                throw new PdfException("Missing integer value.");

            bool            Sign          = false;
            Int32           IntegerValue  = 0;

            if (c == '-' || c == '+') {
                Sign = (c == '-');
                c = _ReadByte();
            }

            while ((c >= '0' && c <= '9') || c == '.') {
                IntegerValue = (IntegerValue * 10) + (c - '0');
                c = _ReadByte();
            }

            if (Sign)
                IntegerValue = -IntegerValue;

            return IntegerValue;
        }
        public              UInt32          ReadHex()
        {
            int c = _SkipSpaces();
            if (c < 0)
                throw new PdfException("Missing hex value.");

            UInt32          Value  = 0;

            while ((c >= '0' && c <= '9')
                || (c >= 'A' && c <= 'F')
                || (c >= 'a' && c <= 'f'))
            {
                Value = (Value << 4) | _CharToNible(c);
                c = _ReadByte();
            }

            return Value;
        }
        public              double          ReadNumber()
        {
            int c = _SkipSpaces();
            if (c < 0)
                throw new PdfException("Missing number value.");

            bool            Sign          = false;
            Int64           IntegerValue  = 0;
            double          FractionDiv   = 1;

            if (c == '-' || c == '+') {
                Sign = (c == '-');
                c = _ReadByte();
            }

            while ((c >= '0' && c <= '9') || c == '.') {
                IntegerValue = (IntegerValue * 10) + (c - '0');
                c = _ReadByte();
            }

            if (c == '.') {
                c = _ReadByte();

                while (c >= '0' && c <= '9') {
                    IntegerValue = (IntegerValue * 10) + (c - '0');
                    FractionDiv *= 10;
                    c = _ReadByte();
                }
            }

            if (Sign)
                IntegerValue = -IntegerValue;

            return (double)IntegerValue / FractionDiv;
        }
        public              bool            ReadBoolean()
        {
            switch(ReadString(false)) {
            case "true":    return true;
            case "false":   return false;
            default:        throw new PdfException("Invalid boolean value.");
            }
        }
        public              AfmRectangle    ReadRectangle()
        {
            return new AfmRectangle(ReadInteger(), ReadInteger(), ReadInteger(), ReadInteger());
        }

        private             int             _SkipSpaces()
        {
            int     c;

            while (!(_atNewLine || _atSubKey)) {
                c = _ReadByte();

                if (c != ' ' && c!= '\t' && c != '\r')
                    return c;
            }

            return -1;
        }
        private             int             _ReadByte()
        {
            int     c;

            c = _stream.ReadByte();

            switch(c) {
            case ';':
                _atSubKey  = true;
                _atNewLine = false;
                break;

            case '\n':
                _atSubKey  = true;
                _atNewLine = true;
                ++_lineNo;
                break;

            case -1:
                _atSubKey  = true;
                _atNewLine = true;
                break;

            default:
                _atSubKey  = false;
                _atNewLine = false;
                break;
            }

            return c;
        }
        private static      byte            _CharToNible(int c)
        {
            if (c >= '0' && c <= '9')
                return (byte)(c - '0');

            if (c >= 'A' && c <= 'F')
                return (byte)(10 + (c - 'A'));

            if (c >= 'a' && c <= 'f')
                return (byte)(10 + (c - 'a'));

            throw new PdfExceptionReader("hexadecimal character expected.");
        }

        private static      Hashtable       _LoadGlyphNameList()
        {
            try {
                Hashtable glyphList = new Hashtable(10000);

                using (Stream stream = new FileStream("GlyphNameList.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream == null)
                        throw new PdfException("Can't open resource 'Jannesen.FileFormat.Pdf.Data.GlyphList.txt'.");

                    AfmReader   reader = new AfmReader(stream);

                    string  key;

                    while ((key = reader.ReadKey(false)) != null) {
                        try {
                            glyphList.Add(key, (int)reader.ReadHex());
                        }
                        catch(Exception Err) {
                            throw new PdfException("Can't add '" + key + "'.", Err);
                        }
                    }
                }

                return glyphList;
            }
            catch(Exception err) {
                throw new Exception("Can't load GlyphList.", err);
            }
        }
    }
#endif
}
