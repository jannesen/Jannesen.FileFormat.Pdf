/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Internal
{
    public class PdfStreamWriter
    {
        private readonly static         byte[]              bsFileHeader        = new byte[] { (byte)'%', (byte)'P', (byte)'D', (byte)'F', (byte)'-', (byte)'1', (byte)'.', (byte)'6', (byte)'\n', (byte)'%', 0xE2, 0xE3, 0xCF, 0xD3, (byte)'\n' };
        private readonly static         byte[]              bsFileEOF           = Encoding.ASCII.GetBytes("%%EOF\n");
        private readonly static         byte[]              bsNull              = Encoding.ASCII.GetBytes("null");
        private readonly static         byte[]              bsFalse             = Encoding.ASCII.GetBytes("false");
        private readonly static         byte[]              bsTrue              = Encoding.ASCII.GetBytes("true");
        private readonly static         byte[]              bsObjBegin          = Encoding.ASCII.GetBytes("obj\n");
        private readonly static         byte[]              bsObjEnd            = Encoding.ASCII.GetBytes("\nendobj\n");
        private readonly static         byte[]              bsXref              = Encoding.ASCII.GetBytes("xref\n");
        private readonly static         byte[]              bsTrailer           = Encoding.ASCII.GetBytes("trailer\n");
        private readonly static         byte[]              bsStartXref         = Encoding.ASCII.GetBytes("startxref\n");
        private readonly static         byte[]              bsDictionaryBegin   = Encoding.ASCII.GetBytes("<<");
        private readonly static         byte[]              bsDictionaryEnd     = Encoding.ASCII.GetBytes(">>");
        private readonly static         byte[]              bsStreamBegin       = Encoding.ASCII.GetBytes("stream\n");
        private readonly static         byte[]              bsStreamEnd         = Encoding.ASCII.GetBytes("\nendstream");
        private readonly static         byte[]              bsRelative          = Encoding.ASCII.GetBytes(" R");
        private readonly static         byte[]              bsUnicodePrefix     = new byte[] { 254, 254, 255 };

        private                         Stream              _pdfStream;         // The PDF data stream
        private                         int                 _pdfPosition;       // Position in PDF stream.
        private                         PdfValueType        _previousValue;     // of the last writen token

        public                          Stream              PdfStream
        {
            get {
                return _pdfStream;
            }
        }
        public                          int                 PdfPosition
        {
            get {
                return _pdfPosition;
            }
        }

        public                                              PdfStreamWriter(Stream pdfStream)
        {
            _pdfStream     = (pdfStream is MemoryStream) ? pdfStream : new BufferedStream(pdfStream);
            _pdfPosition   = 0;
            _previousValue = PdfValueType.None;
        }

        public                          PdfStreamWriter     ResetState()
        {
            _previousValue = PdfValueType.None;
            return this;
        }
        public                          void                WriteRectangle(PdfRectangle rectangle)
        {
            WriteArrayBegin();
            WriteNumber(rectangle.llx.pnts);
            WriteNumber(rectangle.lly.pnts);
            WriteNumber(rectangle.urx.pnts);
            WriteNumber(rectangle.ury.pnts);
            WriteArrayEnd();
        }
        public                          void                WriteNull()
        {
            switch (_previousValue)
            {
            case PdfValueType.None:
            case PdfValueType.DictionaryBegin:
            case PdfValueType.ArrayBegin:
                break;

            default:
                WriteSeparator();
                break;
            }

            WriteByteArray(bsNull);
            _previousValue = PdfValueType.Null;
        }
        public                          void                WriteBoolean(bool value)
        {
            switch (_previousValue)
            {
            case PdfValueType.None:
            case PdfValueType.DictionaryBegin:
            case PdfValueType.ArrayBegin:
                break;

            default:
                WriteSeparator();
                break;
            }

            WriteByteArray((value) ? bsTrue : bsFalse);
            _previousValue = PdfValueType.Boolean;
        }
        public                          void                WriteInteger(int value)
        {
            switch (_previousValue)
            {
            case PdfValueType.None:
            case PdfValueType.DictionaryBegin:
            case PdfValueType.ArrayBegin:
            case PdfValueType.ObjectEnd:
                break;

            default:
                WriteSeparator();
                break;
            }

            if (value != 0) {
                bool    sign = false;
                byte[]  buf  = new byte[16];
                int     pos  = 16;

                if (value < 0) {
                    sign = true;
                    value = -value;
                }

                while (value > 0) {
                    buf[--pos] = (byte)('0' + (value % 10));
                    value /= 10;
                }

                if (sign)
                    buf[--pos] = (byte)'-';

                WriteByteArray(buf, pos, 16-pos);
            }
            else
                WriteByte((byte)'0');

            _previousValue = PdfValueType.Integer;
        }
        public                          void                WriteNumber(double value)
        {
            switch (_previousValue)
            {
            case PdfValueType.None:
            case PdfValueType.DictionaryBegin:
            case PdfValueType.ArrayBegin:
                break;

            default:
                WriteSeparator();
                break;
            }

            bool    sign = false;
            int     prec = 2;
            byte[]  buf  = new byte[16];
            int     pos  = buf.Length;
            bool    f    = false;

            if (value < 0) {
                sign = true;
                value = -value;
            }

            switch(prec)
            {
            case 0:
            case 1:     value *=     10.0;      break;
            case 2:     value *=    100.0;      break;
            case 3:     value *=   1000.0;      break;
            case 4:     value *=  10000.0;      break;
            case 5:     value *= 100000.0;      break;
            }

            Int64   IntValue = (Int64)(value + 0.5);

            if (IntValue == 0) {
                buf[--pos] = (byte)'0';
            }
            else {
                ++prec;

                while (IntValue > 0 || prec > 0) {
                    if (prec > 0) {
                        if (--prec == 0) {
                            if (f)
                                buf[--pos] = (byte)'.';

                            f = true;
                        }
                    }

                    if (f || (IntValue % 10) != 0) {
                        buf[--pos] = (byte)('0' + (IntValue % 10));
                        f = true;
                    }

                    IntValue /= 10;
                }

                if (sign)
                    buf[--pos] = (byte)'-';
            }

            WriteByteArray(buf, pos, buf.Length - pos);

            _previousValue = PdfValueType.Number;
        }
        public                          void                WriteName(string name)
        {
            WriteByte((byte)'/');

            for (int i=0 ; i < name.Length ; ++i) {
                char    chr = name[i];

                if (chr > (char)255)
                    throw new PdfExceptionWriter("Invalid character in name.");

                if (chr < (char)33 || chr > (char)126) {
                    WriteByte((byte)'#');
                    WriteHex((byte)chr);
                }
                else
                    WriteByte((byte)chr);
            }

            _previousValue = PdfValueType.Name;
        }
        public                          void                WriteString(string value)
        {
            int     Length = value.Length;
            bool    unicode = false;

            {
                for (int i = 0 ; i < Length ; ++i) {
                    if (value[i] > (char)126) {
                        unicode = true;
                        break;
                    }
                }
            }

            if (unicode) {
                WriteByte((byte)'<');
                WriteByteArray(bsUnicodePrefix, 0, bsUnicodePrefix.Length);

                for (int i = 0 ; i < Length ; ++i)
                    WriteHex(value[i]);

                WriteByte((byte)'>');
                _previousValue = PdfValueType.HexadecimalString;
            }
            else {
                WriteByte((byte)'(');

                for (int i = 0 ; i < Length ; ++i) {
                    char chr = value[i];

                    if (chr < (char)32) {
                        WriteByte((byte)'\\');

                        switch(chr)
                        {
                        case '\n':  WriteByte((byte)'n');       break;
                        case '\r':  WriteByte((byte)'r');       break;
                        case '\t':  WriteByte((byte)'t');       break;
                        case '\b':  WriteByte((byte)'b');       break;
                        case '\f':  WriteByte((byte)'f');       break;

                        default:
                            WriteByte((byte)('0' + ((chr >> 6) & 0x7)));
                            WriteByte((byte)('0' + ((chr >> 3) & 0x7)));
                            WriteByte((byte)('0' + ((chr     ) & 0x7)));
                            break;
                        }
                    }
                    else {
                        if (chr == '(' || chr==')' || chr=='\\')
                            WriteByte((byte)'\\');

                        WriteByte((byte)chr);
                    }
                }

                WriteByte((byte)')');
                _previousValue = PdfValueType.String;
            }
        }
        public                          void                WriteStringHex(byte[] value)
        {
            WriteByte((byte)'<');

            for (int i = 0 ; i < value.Length ; ++i)
                WriteHex(value[i]);

            WriteByte((byte)'>');

            _previousValue = PdfValueType.HexadecimalString;
        }
        public                          void                WriteStringRaw(byte[] value)
        {
            WriteByte((byte)'(');
            WriteByteArray(value, 0, value.Length);
            WriteByte((byte)')');

            _previousValue = PdfValueType.String;
        }
        public                          void                WriteDate(DateTime dt)
        {
            WriteString(dt.ToString("D:yyyyMMddhhmmss+00"));
        }
        public                          void                WriteReference(PdfDocumentWriter document, PdfValue value)
        {
            WriteReference(document.AddObj(value));
        }
        public                          void                WriteReference(PdfWriterReference reference)
        {
            WriteInteger(reference.Id);
            WriteInteger(0);
            WriteByteArray(bsRelative);
            _previousValue = PdfValueType.Reference;
        }

        public                          void                WriteNewLine()
        {
            WriteByte((byte)'\n');
            _previousValue = PdfValueType.None;
        }
        public                          void                WriteSeparator()
        {
            WriteByte((byte)' ');
            _previousValue = PdfValueType.None;
        }
        public                          void                WriteDictionaryBegin()
        {
            WriteByteArray(bsDictionaryBegin);
            _previousValue = PdfValueType.DictionaryBegin;
        }
        public                          void                WriteDictionaryEnd()
        {
            WriteByteArray(bsDictionaryEnd);
            _previousValue = PdfValueType.DictionaryEnd;
        }
        public                          void                WriteArrayBegin()
        {
            WriteByte((byte)'[');

            _previousValue = PdfValueType.ArrayBegin;
        }
        public                          void                WriteArrayEnd()
        {
            WriteByte((byte)']');
            _previousValue = PdfValueType.ArrayEnd;
        }
        public                          void                WriteFileHeader()
        {
            WriteByteArray(PdfStreamWriter.bsFileHeader);
            _previousValue = PdfValueType.None;
        }
        public                          void                WriteXrefHeader(int cnt)
        {
            WriteByteArray(PdfStreamWriter.bsXref);
            _previousValue = PdfValueType.None;
            WriteInteger(0);
            WriteInteger(cnt);
            WriteNewLine();
        }
        public                          void                WriteTrailer()
        {
            WriteByteArray(PdfStreamWriter.bsTrailer);
            _previousValue = PdfValueType.None;
        }
        public                          void                WriteEOF(int pos)
        {
            WriteByteArray(PdfStreamWriter.bsStartXref);
            _previousValue = PdfValueType.None;
            WriteInteger(pos);
            WriteNewLine();
            WriteByteArray(PdfStreamWriter.bsFileEOF);
        }
        public                          void                WriteObj(PdfDocumentWriter document, PdfWriterReference reference, PdfValue obj)
        {
            WriteInteger(reference.Id);
            WriteInteger(0);
            WriteObjBegin();
            obj.pdfWriteToDocument(document, this);
            WriteObjEnd();
        }
        public                          void                WriteStream(byte[] streamData, int dataLength)
        {
            WriteByteStr(bsStreamBegin);
            WriteByteArray(streamData, 0, dataLength);
            WriteByteArray(bsStreamEnd);
            _previousValue = PdfValueType.StreamEnd;
        }
        public                          void                WriteStream(Stream stream, int dataLength)
        {
            byte[]  buf  = new byte[4096];

            WriteByteStr(bsStreamBegin);

            while (dataLength > 0) {
                int size = (dataLength > buf.Length) ? buf.Length : dataLength;

                if (stream.Read(buf, 0, size) != size)
                    throw new PdfExceptionWriter("EOF read on object stream.");

                WriteByteArray(buf, 0, size);

                dataLength -= size;
            }

            WriteByteArray(bsStreamEnd);
            _previousValue = PdfValueType.StreamEnd;
        }
        public                          void                WriteObjBegin()
        {
            WriteByteStr(bsObjBegin);
            _previousValue = PdfValueType.ObjectBegin;
        }
        public                          void                WriteObjEnd()
        {
            WriteByteArray(bsObjEnd);
            _previousValue = PdfValueType.ObjectEnd;
        }
        public                          void                WriteByteStr(byte[] str)
        {
            switch (_previousValue)
            {
            case PdfValueType.None:
            case PdfValueType.DictionaryBegin:
            case PdfValueType.ArrayBegin:
            case PdfValueType.DictionaryEnd:
            case PdfValueType.ArrayEnd:
            case PdfValueType.ObjectBegin:
            case PdfValueType.ObjectEnd:
                break;

            default:
                WriteSeparator();
                break;
            }

            WriteByteArray(str);
            _previousValue = PdfValueType.ByteStr;
        }
        public                          void                WriteHex(char chr)
        {
            WriteByte(_nibleToHex(chr >> 12));
            WriteByte(_nibleToHex(chr >> 8));
            WriteByte(_nibleToHex(chr >> 4));
            WriteByte(_nibleToHex(chr     ));
        }
        public                          void                WriteHex(byte b)
        {
            WriteByte(_nibleToHex(b >> 4));
            WriteByte(_nibleToHex(b     ));
        }
        public                          void                WriteByteArray(byte[] ba)
        {
            WriteByteArray(ba, 0, ba.Length);
        }
        public                          void                WriteByteArray(byte[] ba, int offset, int length)
        {
            _pdfStream.Write(ba, offset, length);
            _pdfPosition += length;
        }
        public                          void                WriteByte(byte b)
        {
            _pdfStream.WriteByte(b);
            _pdfPosition++;
        }

        private static                  byte                _nibleToHex(int nible)
        {
            nible &= 0xF;

            return (nible < 10) ? (byte)('0'+nible) : (byte)('A'+ (nible - 10));
        }
    }
}
