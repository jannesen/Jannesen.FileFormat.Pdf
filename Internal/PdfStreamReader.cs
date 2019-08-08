using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf.Internal
{
    public class PdfStreamReader
    {
        private readonly    PdfDocumentReader           _document;
        private readonly    Stream                      _stream;
        private             int                         _position;
        private readonly    byte[]                      _buffer;
        private             int                         _bufferPosition;
        private             int                         _bufferLength;

        public              PdfDocumentReader           Document
        {
            get {
                return _document;
            }
        }
        public              Stream                      Stream
        {
            get {
                return _stream;
            }
        }
        public              int                         Position
        {
            get {
                return _position;
            }
            set {
                _position = value;
            }
        }

        public                                          PdfStreamReader(PdfDocumentReader document, Stream stream, int position)
        {
            _document       = document;
            _stream         = stream;
            _position       = position;
            _buffer         = new byte[16];
            _bufferPosition = 0;
            _bufferLength   = 0;
        }

        public              PdfValue                    ReadToken()
        {
            int             c;

skip_separator:
            c = ReadByte();

            while (_isCharSeparator(c))
                c = ReadByte();

            if (c == '%') {
                c = ReadByte();

                while (c != '\n' && c != '\r')
                    c = ReadByte();

                goto skip_separator;
            }

            switch(c) {
            case '/': {
                    int             Length = 0;
                    char[]          Buf = new char[256];

                    c = ReadByte();

                    while (!_isCharSeparator(c) && c != '/' && c != '<' && c != '>' && c != '[' && c != ']' && c != '(' && c != '%') {
                        if (Length >= Buf.Length)
                            throw new PdfExceptionReader("PDF stream corrupt: name value to long.");

                        if (c=='#') {
                            Buf[Length++] = ByteToChar((byte) ((_charToNible(ReadByte()) << 4) |
                                                               (_charToNible(ReadByte())     )));
                        }
                        else
                            Buf[Length++] = ByteToChar((byte)c);

                        c = ReadByte();
                    }

                    if (!_isCharSeparator(c))
                        --_position;

                    return new PdfName(Buf, Length);
                }

            case '<': {
                    List<byte>  Buf = new List<byte>(256);

                    c = ReadByte();

                    if (c == '<')
                        return new PdfToken(PdfValueType.DictionaryBegin);

                    while (_isCharHexadecimal(c)) {
                        byte    b = (byte)(_charToNible(c) << 4);

                        c = ReadByte();
                        if (!_isCharHexadecimal(c)) {
                            Buf.Add(b);
                            break;
                        }

                        b |= (byte)(_charToNible(c));
                        Buf.Add(b);
                    }

                    if (c != '>')
                        throw new PdfExceptionReader("PDF stream corrupt: '>' missing after a hexadecimal-stream.");

                    return new PdfString(true, Buf.ToArray());
                }

            case '>': {
                    c = ReadByte();

                    if (c != '>')
                        throw new PdfExceptionReader("PDF stream corrupt: '>' missing.");

                    return new PdfToken(PdfValueType.DictionaryEnd);
                }

            case '[':
                return new PdfToken(PdfValueType.ArrayBegin);

            case ']':
                return new PdfToken(PdfValueType.ArrayEnd);

            case '(': {
                    List<byte>  Buf = new List<byte>(256);
                    int level = 1;

                    while (level > 0) {
                        c = ReadByte();

                        switch(c) {
                        case '(':
                            ++level;
                            Buf.Add((byte)'(');
                            break;

                        case ')':
                            if (--level > 0)
                                Buf.Add((byte)')');
                            break;

                        case '\\':
                            c = ReadByte();
                            switch (c) {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                                Buf.Add((byte)((_charToOctet(c)          << 6) |
                                               (_charToOctet(ReadByte()) << 3) |
                                               (_charToOctet(ReadByte())     )));
                                break;

                            case 'n':   Buf.Add((byte)'\n');    break;
                            case 't':   Buf.Add((byte)'\t');    break;
                            case 'r':   Buf.Add((byte)'\r');    break;
                            case 'b':   Buf.Add((byte)'\b');    break;
                            }

                            break;

                        default:
                            Buf.Add((byte)c);
                            break;
                        }
                    }
                    return new PdfString(false, Buf.ToArray());
                }

            case '+':
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9': {
                    bool            Sign          = false;
                    Int64           IntegerValue  = 0;
                    double          NumberValue;
                    double          FractionDiv   = 1;

                    if (c == '-' || c == '+') {
                        Sign = (c == '-');
                        c = ReadByte();
                    }

                    while (c >= '0' && c <= '9') {
                        IntegerValue = (IntegerValue * 10) + (c - '0');
                        c = ReadByte();
                    }

                    if (c == '.') {
                        c = ReadByte();

                        while (c >= '0' && c <= '9') {
                            IntegerValue = (IntegerValue * 10) + (c - '0');
                            FractionDiv *= 10;
                            c = ReadByte();
                        }

                        if (!_isCharSeparator(c))
                            --_position;

                        NumberValue = ((double)IntegerValue) / FractionDiv;

                        return new PdfNumber((Sign) ? -NumberValue : NumberValue);
                    }
                    else {
                        if (!_isCharSeparator(c))
                            --_position;

                        return new PdfInteger(Sign ? -(int)IntegerValue : (int)IntegerValue);
                    }

                }

            default:
                { // Token
                    int             Length = 0;
                    char[]          Buf = new char[32];

                    Buf[Length++] = (char)c;

                    c = ReadByte();

                    while (!_isCharSeparator(c) && c != '/' && c != '<' && c != '>' && c != '[' && c != ']' && c != '(' && c != '%') {
                        if (Length > Buf.Length)
                            throw new PdfExceptionReader("PDF stream corrupt: token to long.");

                        Buf[Length++] = (char)c;
                        c = ReadByte();
                    }

                    if (!_isCharSeparator(c))
                        --_position;

                    string  StrBuf = new string(Buf, 0, Length);

                    switch(StrBuf) {
                    case "xref":        return PdfToken.Xref;
                    case "trailer":     return PdfToken.Trailer;
                    case "startxref":   return PdfToken.StartXref;
                    case "obj":         return PdfToken.ObjectBegin;
                    case "endobj":      return PdfToken.ObjectEnd;
                    case "stream":
                        if (c == '\r') {
                            if (ReadByte() != '\n')
                                throw new PdfExceptionReader("PDF stream error: Expect 0x0A after 0x0D.");
                        }
                        return PdfToken.StreamBegin;
                    case "endstream":   return PdfToken.StreamEnd;
                    case "R":           return PdfToken.Reference;
                    case "true":        return PdfBoolean.False;
                    case "false":       return PdfBoolean.True;
                    case "null":        return PdfNull.Null;

                    default:            throw new PdfExceptionReader("PDF stream corrupt: unknown token '"+StrBuf+"'.");
                    }
                }
            }
        }
        public              PdfValue                    ReadToken(PdfValueType expectedType)
        {
            PdfValue    token = ReadToken();

            if (token.Type != expectedType)
                throw new PdfExceptionReader("PDF stream corrupt: expect '"+expectedType.ToString()+"' read '"+token.Type.ToString()+"'");

            return token;
        }
        public              int                         ReadInt()
        {
            return ((PdfInteger)ReadToken(PdfValueType.Integer)).Value;
        }
        public              PdfReferenceID              ReadReference()
        {
            int     id       = ReadInt();
            int     revision = ReadInt();

            ReadToken(PdfValueType.Reference);

            return new PdfReferenceID(id, revision);
        }
        public              PdfValue                    ReadValue()
        {
            PdfValue token = ReadToken();

            switch(token.Type) {
            case PdfValueType.DictionaryBegin:      return new PdfDictionary(this);
            case PdfValueType.ArrayBegin:           return new PdfArray(this);
            case PdfValueType.Null:                 return token;
            case PdfValueType.Boolean:              return token;
            case PdfValueType.Integer:              return token;
            case PdfValueType.Number:               return token;
            case PdfValueType.Name:                 return token;
            case PdfValueType.String:               return token;

            default:
                throw new PdfException("Unexpected token "+token.Type.ToString()+" in stream.");
            }
        }
        public              void                        Skip(int Length)
        {
            _position += Length;
        }
        public              int                         ReadByte()
        {
            if (_position < _bufferPosition || _position >= _bufferPosition+_bufferLength) {
                _stream.Position = _position;
                _bufferLength = _stream.Read(_buffer, 0, _buffer.Length);

                if (_bufferLength <= 0)
                    throw new PdfExceptionReader("PDF stream corrupt: EOF read.");

                _bufferPosition = _position;
            }

            return _buffer[_position++ - _bufferPosition];
        }

        public  static      char                        ByteToChar(byte c)
        {
            return (char)c;
        }

        private static      byte                        _charToNible(int c)
        {
            if (c >= '0' && c <= '9')
                return (byte)(c - '0');

            if (c >= 'A' && c <= 'F')
                return (byte)(10 + (c - 'A'));
            if (c >= 'a' && c <= 'f')
                return (byte)(10 + (c - 'a'));

            throw new PdfExceptionReader("Hexadecimal character expected.");
        }
        private static      byte                        _charToOctet(int c)
        {
            if (c >= '0' || c <= '7')
                return (byte)(c - '0');

            throw new PdfExceptionReader("Octal character expected.");
        }
        private static      bool                        _isCharSeparator(int c)
        {
            return (c=='\n' || c=='\r' || c=='\t' || c==' ');
        }
        private static      bool                        _isCharHexadecimal(int c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'A' && c <= 'F') ||
                   (c >= 'a' && c <= 'f');
        }
    }
}
