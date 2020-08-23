using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public sealed class PdfContent: PdfObject, IDisposable
    {
        private static  readonly    byte[]                              bs_b        = Encoding.ASCII.GetBytes("b\n");
        private static  readonly    byte[]                              bs_B        = Encoding.ASCII.GetBytes("B\n");
        private static  readonly    byte[]                              bs_CS       = Encoding.ASCII.GetBytes("CS\n");
        private static  readonly    byte[]                              bs_cs       = Encoding.ASCII.GetBytes("cs\n");
        private static  readonly    byte[]                              bs_d        = Encoding.ASCII.GetBytes("d\n");
        private static  readonly    byte[]                              bs_l        = Encoding.ASCII.GetBytes("l\n");
        private static  readonly    byte[]                              bs_BT       = Encoding.ASCII.GetBytes("BT\n");
        private static  readonly    byte[]                              bs_ET       = Encoding.ASCII.GetBytes("ET\n");
        private static  readonly    byte[]                              bs_f        = Encoding.ASCII.GetBytes("f\n");
        private static  readonly    byte[]                              bs_h        = Encoding.ASCII.GetBytes("h\n");
        private static  readonly    byte[]                              bs_J        = Encoding.ASCII.GetBytes("J\n");
        private static  readonly    byte[]                              bs_j        = Encoding.ASCII.GetBytes("j\n");
        private static  readonly    byte[]                              bs_m        = Encoding.ASCII.GetBytes("m\n");
        private static  readonly    byte[]                              bs_M        = Encoding.ASCII.GetBytes("M\n");
        private static  readonly    byte[]                              bs_Tf       = Encoding.ASCII.GetBytes("Tf\n");
        private static  readonly    byte[]                              bs_Tj       = Encoding.ASCII.GetBytes("Tj\n");
        private static  readonly    byte[]                              bs_Tm_POS_H = Encoding.ASCII.GetBytes("1 0 0 1 ");
        private static  readonly    byte[]                              bs_Tm_POS_V = Encoding.ASCII.GetBytes("0 1 -1 0 ");
        private static  readonly    byte[]                              bs_Tm       = Encoding.ASCII.GetBytes("Tm\n");
        private static  readonly    byte[]                              bs_re       = Encoding.ASCII.GetBytes("re\n");
        private static  readonly    byte[]                              bs_s        = Encoding.ASCII.GetBytes("s\n");
        private static  readonly    byte[]                              bs_S        = Encoding.ASCII.GetBytes("S\n");
        private static  readonly    byte[]                              bs_SC       = Encoding.ASCII.GetBytes("SC\n");
        private static  readonly    byte[]                              bs_sc       = Encoding.ASCII.GetBytes("sc\n");
        private static  readonly    byte[]                              bs_w        = Encoding.ASCII.GetBytes("w\n");
        private static  readonly    byte[]                              bs_q        = Encoding.ASCII.GetBytes("q\n");
        private static  readonly    byte[]                              bs_Q        = Encoding.ASCII.GetBytes("Q\n");
        private static  readonly    byte[]                              bs_cm       = Encoding.ASCII.GetBytes("cm\n");
        private static  readonly    byte[]                              bs_Do       = Encoding.ASCII.GetBytes("Do\n");
        private static  readonly    byte[]                              bs_0        = Encoding.ASCII.GetBytes("0");
        private static  readonly    byte[]                              bs_0S       = Encoding.ASCII.GetBytes("0 ");

        private                 PdfContent                          _parent;
        private                 bool                                _locked;
        private                 StreamBuffer                        _dataStream;
        private                 PdfResourceEntryList                _resources;
        private                 string                              _curStrokeColorSpace;
        private                 string                              _curNonStrokeColorSpace;
        private                 PdfColor                            _curStrokeColor;
        private                 PdfColor                            _curNonStrokeColor;
        private                 PdfDistance                         _curLineWidth;
        private                 PdfLineCapStyle                     _curLineCap;
        private                 PdfLineJoinStyle                    _curLineJoin;
        private                 double                              _curMiterLimit;
        private                 PdfDistance[]                       _curDashArray;
        private                 PdfDistance                         _curDashPhase;
        private                 bool                                _textMode;
        private                 PdfFont                             _curFont;
        private                 PdfDistance                         _curFontSize;

        public  override        bool                                hasStream           { get => true;          }

        public                  PdfContent                          Parent
        {
            get {
                return _parent;
            }
        }
        public                  PdfResourceEntryList                Resources
        {
            get {
                return _resources;
            }
        }

        public                  string                              PostScript
        {
            get {
                return ASCIIEncoding.Default.GetString(_dataStream.ToArray());
            }
            set {
                _dataStream             = new StreamBuffer();
                _curStrokeColorSpace    = null;
                _curNonStrokeColorSpace = null;
                _curStrokeColor         = null;
                _curNonStrokeColor      = null;
                _curLineWidth           = new PdfDistance(-1);
                _curLineCap             = PdfLineCapStyle.Unknown;
                _curLineJoin            = PdfLineJoinStyle.Unknown;
                _curMiterLimit          = -1;
                _curDashArray           = null;
                _curDashPhase           = new PdfDistance(-1);
                var bytes = ASCIIEncoding.Default.GetBytes(value);
                _dataStream.Write(bytes, 0, bytes.Length);
            }
        }

        public                                                      PdfContent()
        {
            _init();
        }
        public                                                      PdfContent(PdfContent parent)
        {
            _parent    = parent;
            _init();

            if (parent != null) {
                for (int i = 0 ; i < parent._resources.Count ; ++i)
                    _resources.Add(parent._resources[i]);
            }
        }
        public                                                      PdfContent(PdfDictionary dictonary)
        {
            if (dictonary is null) throw new ArgumentNullException(nameof(dictonary));

            if (dictonary.NamedType != PdfObject.ntPage)
                throw new PdfException("Object is not a page or content.");

            try {
                _init();
                _readPageResources(dictonary);
                _readPageContent(dictonary);
            }
            catch(Exception err) {
                throw new PdfException("Construct content from ReaderPage failed.", err);
            }
        }
        private                                                     PdfContent(PdfContent parent, PdfObjectReader obj)
        {
            _parent = parent;
            _init();
            _readContent(obj);
        }

        public                  void                                Dispose()
        {
            if (_dataStream != null) {
                _dataStream.Dispose();
                _dataStream = null;
            }
        }

        public                  void                                TextBox(PdfPoint upperLeftCorner, PdfStyleText style, PdfTextAlign align, PdfDistance width, int maxLines, string text)
        {
            if (!string.IsNullOrEmpty(text)) {
                Formatter.TextBox       TextBox = new Formatter.TextBox(style, align, width, maxLines, text);
                TextBox.Print(upperLeftCorner,  this);
            }
        }
        public                  void                                DrawLine(PdfStyleLine lineStyle, PdfPoint begin, PdfSize size)
        {
            if (_textMode)
                opEndText();

            SetLineStyle(lineStyle);
            opMoveTo(begin);
            opLineTo(begin + size);
            opStroke();
        }
        public                  void                                DrawRectangle(PdfStyleLine lineStyle, PdfStyleFill fillStyle, PdfPoint begin, PdfSize size)
        {
            if (_textMode)
                opEndText();

            if (lineStyle != null)
                SetLineStyle(lineStyle);

            if (fillStyle != null)
                SetFillStyle(fillStyle);

            opRectangle(begin, size);

            if (lineStyle != null & fillStyle != null)
                opFillStroke();
            else
            if (lineStyle != null)
                opStroke();
            else
            if (fillStyle != null)
                opFill();
        }
        public                  void                                Draw(PdfStyleLine lineStyle, PdfStyleFill fillStyle, PdfPoint begin, PdfSize[] sizes, bool closePath)
        {
            if (_textMode)
                opEndText();

            if (lineStyle != null)
                SetLineStyle(lineStyle);

            if (fillStyle != null)
                SetFillStyle(fillStyle);

            opMoveTo(begin);

            if (sizes != null) {
                PdfPoint    Pos = begin;

                for (int i = 0 ; i < sizes.Length ; ++i) {
                    Pos += sizes[i];
                    opLineTo(Pos);
                }
            }

            if (closePath) {
                if (lineStyle != null & fillStyle != null)
                    opCloseFillStroke();
                else
                if (lineStyle != null)
                    opCloseStroke();
                else
                if (fillStyle != null) {
                    opClosePath();
                    opFill();
                }
            }
            else {
                if (fillStyle != null)
                    throw new PdfException("Can't fill a non closed path.");

                opStroke();
            }
        }
        public                  void                                Text(PdfStyleText textStyle, PdfPoint point, PdfTextAlign align, string txt)
        {
            if (textStyle is null) throw new ArgumentNullException(nameof(textStyle));

            if (!string.IsNullOrEmpty(txt)) {
                SetTextStyle(textStyle);

                if (align != PdfTextAlign.Left) {
                    PdfDistance Width = textStyle.Font.TextWidth(textStyle.FontSize, txt);

                    switch(align) {
                    case PdfTextAlign.Right:    point.x -= Width;       break;
                    case PdfTextAlign.Center:   point.x -= Width / 2;   break;
                    }
                }

                SetTextMatrixH(point);
                opShowText(txt, 0, txt.Length);
            }
        }
        public                  void                                Text(PdfStyleText textStyle, PdfPoint point, PdfTextAlign align, double rotation, string txt)
        {
            if (textStyle is null) throw new ArgumentNullException(nameof(textStyle));

            if (!string.IsNullOrEmpty(txt)) {
                SetTextStyle(textStyle);

                rotation *= Math.PI/180.0;

                double  Rotation_Sin = Math.Round(Math.Sin(rotation), 5);
                double  Rotation_Cos = Math.Round(Math.Cos(rotation), 5);

                if (align != PdfTextAlign.Left) {
                    PdfDistance Width = textStyle.Font.TextWidth(textStyle.FontSize, txt);

                    if (align == PdfTextAlign.Center)
                        Width = Width / 2.0;

                    point.x -= Width * Rotation_Cos;
                    point.y -= Width * Rotation_Sin;
                }

                SetTextMatrix(point, Rotation_Sin, Rotation_Cos);
                opShowText(txt, 0, txt.Length);
            }
        }
        public                  void                                Image(PdfPoint point, PdfSize size, PdfImage image)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));

            if (_textMode)
                opEndText();

            opSaveGraphicsState();
            opCurrentTransformationMatrix(1,0,0,1,point.y.pnts,point.x.pnts);
            opCurrentTransformationMatrix(size.height.pnts,0,0,size.width.pnts,0,0);
            WriteResourceName(image);
            WriteStr(bs_Do);
            opRestoreGraphicsState();
        }
        public                  void                                SetLineStyle(PdfStyleLine style)
        {
            if (style is null) throw new ArgumentNullException(nameof(style));

            if (style.LineColor != _curStrokeColor) {
                if (style.LineColor.ColorSpace != _curStrokeColorSpace)
                    opSetStrokeColorSpace(style.LineColor.ColorSpace);

                opSetStrokeColor(style.LineColor);
            }

            if (style.LineWidth  != _curLineWidth)
                opSetLineWidth(style.LineWidth);

            if (style.CapStyle  != _curLineCap)
                opSetLineCap(style.CapStyle);

            if (style.JoinStyle != _curLineJoin)
                opSetLineJoin(style.JoinStyle);

            if (style.MiterLimit != _curMiterLimit)
                opSetMiterLimit(style.MiterLimit);

            if (style.DashArray != _curDashArray || style.DashPhase != _curDashPhase)
                opSetDash(style.DashArray, style.DashPhase);
        }
        public                  void                                SetFillStyle(PdfStyleFill style)
        {
            if (style is null) throw new ArgumentNullException(nameof(style));

            if (style.FillColor != _curNonStrokeColor) {
                if (style.FillColor.ColorSpace != _curNonStrokeColorSpace)
                    opSetNonStrokeColorSpace(style.FillColor.ColorSpace);

                opSetNonStrokeColor(style.FillColor);
            }
        }
        public                  void                                SetTextStyle(PdfStyleText style)
        {
            if (style is null) throw new ArgumentNullException(nameof(style));

            if (style.TextColor != _curNonStrokeColor) {
                if (style.TextColor.ColorSpace != _curNonStrokeColorSpace)
                    opSetNonStrokeColorSpace(style.TextColor.ColorSpace);

                opSetNonStrokeColor(style.TextColor);
            }

            if (style.Font != _curFont || style.FontSize != _curFontSize)
                opSelectFont(style.Font, style.FontSize);
        }
        public                  void                                SetTextMatrixH(PdfPoint point)
        {
            if (!_textMode)
                opBeginText();

            WriteStr(bs_Tm_POS_H);
            WriteNumber(point.x.pnts, 2, true);
            WriteNumber(point.y.pnts, 2, true);
            WriteStr(bs_Tm);
        }
        public                  void                                SetTextMatrix(PdfPoint point, double rotation_Sin, double rotation_Cos)
        {
            if (!_textMode)
                opBeginText();

            if (rotation_Sin == 0.0 && rotation_Cos == 1.0) {
                WriteStr(bs_Tm_POS_H);
            }
            else
            if (rotation_Sin == 1.0 && rotation_Cos == 0.0) {
                WriteStr(bs_Tm_POS_V);
            }
            else {
                WriteNumber( rotation_Cos, 6, true);
                WriteNumber( rotation_Sin, 6, true);
                WriteNumber(-rotation_Sin, 6, true);
                WriteNumber( rotation_Cos, 6, true);
            }

            WriteNumber(point.x.pnts, 2, true);
            WriteNumber(point.y.pnts, 2, true);
            WriteStr(bs_Tm);
        }
        public                  void                                opSetStrokeColorSpace(string name)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            WriteByte((byte)'/');
            WriteStr(name);
            WriteByte((byte)' ');
            WriteStr(bs_CS);
            _curStrokeColorSpace = name;
        }
        public                  void                                opSetNonStrokeColorSpace(string name)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));

            WriteByte((byte)'/');
            WriteStr(name);
            WriteByte((byte)' ');
            WriteStr(bs_cs);
            _curNonStrokeColorSpace = name;
        }
        public                  void                                opSetStrokeColor(PdfColor color)
        {
            if (color is PdfColorRGB rgb) {
                WriteNumber(rgb.Red,   4, true);
                WriteNumber(rgb.Green, 4, true);
                WriteNumber(rgb.Blue,  4, true);
            }
            else
            if (color is PdfColorCMYK cmyk) {
                WriteNumber(cmyk.Cyan,    4, true);
                WriteNumber(cmyk.Magenta, 4, true);
                WriteNumber(cmyk.Yellow,  4, true);
                WriteNumber(cmyk.Black,   4, true);
            }

            WriteStr(bs_SC);
            _curStrokeColor = color;
        }
        public                  void                                opSetNonStrokeColor(PdfColor color)
        {
            if (color is PdfColorRGB rgb) {
                if (_curNonStrokeColorSpace != "DeviceRGB")
                    throw new PdfException("Can't set RGB Color, Invalid device colorspace "+_curStrokeColorSpace+" selected.");

                WriteNumber(rgb.Red,   4, true);
                WriteNumber(rgb.Green, 4, true);
                WriteNumber(rgb.Blue,  4, true);
            }
            else
            if (color is PdfColorCMYK cmyk) {
                if (_curNonStrokeColorSpace != "DeviceCMYK")
                    throw new PdfException("Can't set CMYK Color, Invalid device colorspace "+_curStrokeColorSpace+" selected.");

                WriteNumber(cmyk.Cyan,    4, true);
                WriteNumber(cmyk.Magenta, 4, true);
                WriteNumber(cmyk.Yellow,  4, true);
                WriteNumber(cmyk.Black,   4, true);
            }

            WriteStr(bs_sc);
            _curNonStrokeColor = color;
        }
        public                  void                                opBeginText()
        {
            WriteStr(bs_BT);
            _textMode = true;
        }
        public                  void                                opSelectFont(PdfFont font, PdfDistance size)
        {
            if (font is null) throw new ArgumentNullException(nameof(font));

            WriteResourceName(font);
            WriteNumber(size.pnts, 2, true);
            WriteStr(bs_Tf);
            _curFont     = font;
            _curFontSize = size;
        }
        public                  void                                opShowText(string text, int start, int length)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));

            WriteString(text, start, length);
            WriteStr(bs_Tj);
        }
        public                  void                                opEndText()
        {
            WriteStr(bs_ET);
            _textMode = false;
        }
        public                  void                                opSetLineWidth(PdfDistance width)
        {
            WriteNumber(width.pnts, 2, true);
            WriteStr(bs_w);
            _curLineWidth = width;
        }
        public                  void                                opSetLineCap(PdfLineCapStyle style)
        {
            WriteInteger((int)style, true);
            WriteStr(bs_J);
            _curLineCap = style;
        }
        public                  void                                opSetLineJoin(PdfLineJoinStyle style)
        {
            WriteInteger((int)style, true);
            WriteStr(bs_j);
            _curLineJoin = style;
        }
        public                  void                                opSetMiterLimit(double miterLimit)
        {
            WriteNumber(miterLimit, 2, true);
            WriteStr(bs_M);
            _curMiterLimit = miterLimit;
        }
        public                  void                                opSetDash(PdfDistance[] dashArray, PdfDistance dashPhase)
        {
            WriteByte((byte)'[');

            if (dashArray != null) {
                for(int i = 0 ; i < dashArray.Length ; ++i) {
                    WriteNumber(dashArray[i].pnts, 2, i < dashArray.Length - 1);
                }
            }

            WriteByte((byte)']');
            WriteByte((byte)' ');
            WriteNumber(dashPhase.pnts, 2, true);
            WriteStr(bs_d);

            _curDashArray = dashArray;
            _curDashPhase = dashPhase;
        }
        public                  void                                opMoveTo(PdfPoint point)
        {
            WriteNumber(point.x.pnts, 2, true);
            WriteNumber(point.y.pnts, 2, true);
            WriteStr(bs_m);
        }
        public                  void                                opLineTo(PdfPoint point)
        {
            WriteNumber(point.x.pnts, 2, true);
            WriteNumber(point.y.pnts, 2, true);
            WriteStr(bs_l);
        }
        public                  void                                opRectangle(PdfPoint point, PdfSize size)
        {
            WriteNumber(point.x.pnts, 2,     true);
            WriteNumber(point.y.pnts, 2,     true);
            WriteNumber(size.width.pnts, 2,  true);
            WriteNumber(size.height.pnts, 2, true);
            WriteStr(bs_re);
        }
        public                  void                                opClosePath()
        {
            WriteStr(bs_h);
        }
        public                  void                                opStroke()
        {
            WriteStr(bs_S);
        }
        public                  void                                opFill()
        {
            WriteStr(bs_f);
        }
        public                  void                                opFillStroke()
        {
            WriteStr(bs_B);
        }
        public                  void                                opCloseStroke()
        {
            WriteStr(bs_s);
        }
        public                  void                                opCloseFillStroke()
        {
            WriteStr(bs_b);
        }
        public                  void                                opSaveGraphicsState()
        {
            WriteStr(bs_q);
        }
        public                  void                                opRestoreGraphicsState()
        {
            WriteStr(bs_Q);
        }
        public                  void                                opCurrentTransformationMatrix(double a, double b, double c, double d, double e, double f)
        {
            WriteNumber(a, 4, true);
            WriteNumber(b, 4, true);
            WriteNumber(c, 4, true);
            WriteNumber(d, 4, true);
            WriteNumber(e, 4, true);
            WriteNumber(f, 4, true);
            WriteStr(bs_cm);
        }
        public                  void                                WriteResourceName(PdfObject resource)
        {
            if (resource is null) throw new ArgumentNullException(nameof(resource));

            WriteByte((byte)'/');
            WriteStr(_resources.GetName(resource));
            WriteByte((byte)' ');
        }
        public                  void                                WriteInteger(int value, bool trailingSpace)
        {
            bool    sign = false;
            byte[]  buf  = new byte[16];
            int     pos  = buf.Length;

            if (value < 0) {
                sign = true;
                value = -value;
            }

            if (trailingSpace)
                buf[--pos] = (byte)' ';

            if (value == 0) {
                buf[--pos] = (byte)'0';
            }
            else {
                while (value > 0) {
                    buf[--pos] = (byte)('0' + (value % 10));
                    value /= 10;
                }

                if (sign)
                    buf[--pos] = (byte)'-';
            }

            if (_locked)
                throw new PdfExceptionWriter("Not allowed to change the content after it is added to a document.");

            _dataStream.Write(buf, pos, buf.Length - pos);
        }
        public                  void                                WriteNumber(double number, int precision, bool trailingSpace)
        {
            if (number != 0.0) {
                bool    sign = false;
                byte[]  buf  = new byte[16];
                int     pos  = buf.Length;
                bool    f    = false;

                if (number < 0) {
                    sign = true;
                    number = -number;
                }

                switch(precision) {
                case 0:
                case 1:     number *=      10.0;        break;
                case 2:     number *=     100.0;        break;
                case 3:     number *=    1000.0;        break;
                case 4:     number *=   10000.0;        break;
                case 5:     number *=  100000.0;        break;
                case 6:     number *= 1000000.0;        break;
                default:    throw new ArgumentException("Precision out of range");
                }

                Int64   Value = (Int64)(number + 0.5);

                if (trailingSpace)
                    buf[--pos] = (byte)' ';

                if (Value == 0) {
                    buf[--pos] = (byte)'0';
                }
                else {
                    ++precision;

                    while (Value > 0 || precision > 0) {
                        if (precision > 0) {
                            if (--precision == 0) {
                                if (f)
                                    buf[--pos] = (byte)'.';

                                f = true;
                            }
                        }

                        if (f || (Value % 10) != 0) {
                            buf[--pos] = (byte)('0' + (Value % 10));
                            f = true;
                        }

                        Value /= 10;
                    }

                    if (sign)
                        buf[--pos] = (byte)'-';
                }

                if (_locked)
                    throw new PdfExceptionWriter("Not allowed to change the content after it is added to a document.");

                _dataStream.Write(buf, pos, buf.Length - pos);
            }
            else
                WriteStr(trailingSpace ? bs_0S : bs_0);
        }
        public                  void                                WriteString(string value, int start, int length)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            if (_locked)
                throw new PdfExceptionWriter("Not allowed to change the content after it is added to a document.");

            _dataStream.WriteByte((byte)'(');

            for (int i = 0 ; i < length ; ++i) {
                byte chr = PdfFont.Encode(value[start++]);

                if (chr == '(' || chr==')' || chr=='\\')
                    _dataStream.WriteByte((byte)'\\');

                _dataStream.WriteByte(chr);
            }

            _dataStream.WriteByte((byte)')');
            _dataStream.WriteByte((byte)' ');
        }
        public                  void                                WriteStr(string data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            WriteStr(Encoding.ASCII.GetBytes(data));
        }
        public                  void                                WriteStr(byte[] byteStr)
        {
            if (byteStr is null) throw new ArgumentNullException(nameof(byteStr));

            if (_locked)
                throw new PdfExceptionWriter("Not allowed to change the content after it is added to a document.");

            _dataStream.Write(byteStr, 0, byteStr.Length);
        }
        public                  void                                WriteByte(byte b)
        {
            if (_locked)
                throw new PdfExceptionWriter("Not allowed to change the content after it is added to a document.");

            _dataStream.WriteByte(b);
        }

        public                  string                              GetResourceName(PdfObject resource)
        {
            if (resource is null) throw new ArgumentNullException(nameof(resource));

            return _resources.GetName(resource);
        }

        internal override       void                                pdfAddToDocument(PdfDocumentWriter writer)
        {
            if (_textMode)
                opEndText();

            _locked = true;

            if (Parent != null)
                writer.AddObj(Parent);

            for(int i = 0 ; i<_resources.Count ; ++i) {
                if (_resources[i].Resource is PdfObject obj)
                    writer.AddObj(obj);
            }
        }
        internal override       void                                pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            if (document.CompressContent) {
                using (var compressStream = new StreamBuffer()) {
                    string  Filter = "FlateDecode";

                    using (Stream CompressWriter = PdfFilter.GetCompressor(Filter, compressStream))
                        CompressWriter.Write(_dataStream.GetBuffer(), 0, (int)_dataStream.Length);

                    _writeContent(writer, Filter, compressStream);
                }
            }
            else
                _writeContent(writer, null, _dataStream);
        }

        private                 void                                _writeContent(PdfStreamWriter writer, string filter, StreamBuffer buffer)
        {
            writer.WriteDictionaryBegin();

            if (filter != null) {
                writer.WriteName("Filter");
                writer.WriteName(filter);
            }

            writer.WriteName("Length");
            writer.WriteInteger((int)buffer.Length);
            writer.WriteDictionaryEnd();
            writer.WriteStream(buffer.GetBuffer(), (int)buffer.Length);
            _dataStream.Dispose();
            _dataStream = null; // Release datastream for less memory usage
        }
        private                 void                                _init()
        {
            _resources              = new PdfResourceEntryList();
            _dataStream             = new StreamBuffer();
            _curStrokeColorSpace    = null;
            _curNonStrokeColorSpace = null;
            _curStrokeColor         = null;
            _curNonStrokeColor      = null;
            _curLineWidth           = new PdfDistance(-1);
            _curLineCap             = PdfLineCapStyle.Unknown;
            _curLineJoin            = PdfLineJoinStyle.Unknown;
            _curMiterLimit          = -1;
            _curDashArray           = null;
            _curDashPhase           = new PdfDistance(-1);
        }
        private                 void                                _readContent(PdfObjectReader obj)
        {
            byte[]  buf = new byte[4096];

            using (Stream Source = obj.GetUncompressStream()) {
                int rsize;

                while ((rsize = Source.Read(buf, 0, buf.Length)) > 0)
                    _dataStream.Write(buf, 0, rsize);
            }
        }
        private                 void                                _readPageResources(PdfDictionary page)
        {
            PdfValueList resources = page.ValueByName<PdfDictionary>("Resources", mandatory:false)?.Children;
            if (resources != null) {
                for (int resourceIdx = 0 ; resourceIdx < resources.Count ; ++resourceIdx) {
                    if (resources[resourceIdx] is PdfName) {
                        string  cls = resources[resourceIdx].Cast<PdfName>().Value;

                        try {
                            if (++resourceIdx >= resources.Count)
                                throw new PdfException("Resource dictionary corrupt.");


                            switch(cls) {
#if DEBUG_TEST
                            case "ProcSet":
                                System.Diagnostics.Debug.WriteLine("Resourse " + cls);
                                break;

                            case "Properties": {
                                    var resourceItems = resources[resourceIdx].Resolve<PdfDictionary>();

                                    if ((resourceItems.Children.Count % 2) != 0)
                                        throw new PdfException("Resource dictionary corrupt.");

                                    for (int ItemIdx = 0 ; ItemIdx < resourceItems.Children.Count - 1 ; ItemIdx += 2)
                                        System.Diagnostics.Debug.WriteLine("Resourse " + cls + " " + resourceItems.Children[ItemIdx    ].Cast<PdfName>().Value + " removed.");

                                    ++resourceIdx;
                                }
                                break;
#else
                            case "ProcSet":
                            case "Properties":
                                ++resourceIdx;
                                break;
#endif
                            case "Font":
                            case "XObject":
                            case "ExtGState":
                            case "ColorSpace": {
                                    var resourceItems = resources[resourceIdx].Resolve<PdfDictionary>();

                                    if ((resourceItems.Children.Count % 2) != 0)
                                        throw new PdfException("Resource dictionary corrupt.");

                                    for (int ItemIdx = 0 ; ItemIdx < resourceItems.Children.Count - 1 ; ItemIdx += 2) {
                                        var value = resourceItems.Children[ItemIdx + 1];

                                        if (cls == "Font") {
                                            var fontDic = value.Resolve<PdfDictionary>();

                                            if (new PdfName("Type")           .Equals(fontDic.Children[0]) &&
                                                new PdfName("Font")           .Equals(fontDic.Children[1]) &&
                                                new PdfName("Subtype")        .Equals(fontDic.Children[2]) &&
                                                new PdfName("Type1")          .Equals(fontDic.Children[3]) &&
                                                new PdfName("BaseFont")       .Equals(fontDic.Children[4]) &&
                                                new PdfName("Encoding")       .Equals(fontDic.Children[6]) &&
                                                new PdfName("WinAnsiEncoding").Equals(fontDic.Children[7]))
                                            {
                                                var stdFont = PdfStandard.Standard.FindStandardFontByName(fontDic.Children[5].Cast<PdfName>().Value);
                                                if (stdFont != null)
                                                    value = stdFont;
                                            }
                                        }

                                        _resources.Add(cls,
                                                        resourceItems.Children[ItemIdx    ].Cast<PdfName>().Value,
                                                        value);
                                    }
                                }
                                break;
                            }
                        }
                        catch(Exception err) {
                            throw new PdfException("Processing resource '" + cls + "' failed.", err);
                        }
                    }
                }
            }
        }
        private                 void                                _readPageContent(PdfDictionary page)
        {
            var contents = page.ValueByName("Contents");

            if (contents is PdfArray pdfArray) {
                var         contentsArray = pdfArray.Children;
                PdfContent  Parent        = null;

                for (int i = 0 ; i < contentsArray.Count - 1 ; ++i) {
                    if (!(contentsArray[i] is PdfReferenceReader))
                        throw new PdfException("Invalid content referecen.");

                    Parent = new PdfContent(Parent, ((PdfReferenceReader)contentsArray[i]).Object);
                }

                if (!(contentsArray[contentsArray.Count - 1] is PdfReferenceReader))
                    throw new PdfException("Invalid content referecen.");

                _parent = Parent;
                _readContent(((PdfReferenceReader)contentsArray[contentsArray.Count - 1]).Object);
            }
            else if (contents is PdfReferenceReader pdfRR)
            {
                _readContent(pdfRR.Object);
            }
            else
                throw new PdfExceptionReader("Unknown Content " + contents.GetType().Name);
        }
    }
}
