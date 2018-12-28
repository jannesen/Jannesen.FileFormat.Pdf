/*@
    Copyright � Jannesen Holding B.V. 2007-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class Barcode39Box: Box
    {
        private                     PdfDistance         _width;
        private                     PdfDistance         _height;
        private                     string              _text;
        private static  readonly    UInt16[]            _charEncoding = _loadCharEncoding();
        private static  readonly    UInt16              _startStop    = _barEncoding("NWNNWNWNN");

        public  override            PdfDistance         Width
        {
            get {
                return _width;
            }
            set {
                _width = value;
            }
        }
        public  override            PdfDistance         Height
        {
            get {
                return _height;
            }
        }
        public                      string              Text
        {
            get {
                return _text;
            }
            set {
                _validateText(value);
                _text = value;
            }
        }

        public                                          Barcode39Box(PdfDistance width, PdfDistance height, string text)
        {
            _validateText(text);
            _width  = width;
            _height = height;
            _text   = text;
        }

        public  override            void                boxPrintForground(PdfPoint upperLeftCorner, PdfContent content)
        {
            content.DrawRectangle(null, PdfStyleFill.SolidWhite, upperLeftCorner, new PdfSize(_width, _height));

            PdfDistance     widthNarrow;
            PdfDistance     widthWidth;
            PdfDistance     widthGap;

            widthNarrow = _width / ((_text.Length + 2) * (3 * 3 + 6 ) + (_text.Length + 1) * 1);

            if (widthNarrow > PdfDistance.d_pnt(1.5)) {
                widthWidth  = widthNarrow * 3;
                widthGap    = widthNarrow;
            }
            else {
                widthNarrow = _width / ((_text.Length + 2) * (2.2 * 3 + 6 ) + (_text.Length + 1) * 1);
                widthWidth  = widthNarrow * 2.2;
                widthGap    = widthNarrow;
            }

            PdfDistance     X = upperLeftCorner.x;

            _writeBarcode(content, widthNarrow, widthWidth, ref X, upperLeftCorner.y, _startStop);
            X += widthGap;

            for (int i = 0 ; i < _text.Length ; ++i) {
                _writeBarcode(content, widthNarrow, widthWidth, ref X, upperLeftCorner.y, _charEncoding[_text[i]]);
                X += widthGap;
            }

            _writeBarcode(content, widthNarrow, widthWidth, ref X, upperLeftCorner.y, _startStop);
        }

        private                     void                _writeBarcode(PdfContent content, PdfDistance widthNarrow, PdfDistance widthWide, ref PdfDistance x, PdfDistance y, UInt16 barcode)
        {
            for (int i = 1 << 8 ; i > 0 ; i = i >> 1) {
                PdfDistance     w = (barcode & i) != 0 ? widthWide : widthNarrow;

                if ((i & 0x155) != 0)
                    content.DrawRectangle(null, PdfStyleFill.SolidBlack, new PdfPoint(x, y), new PdfSize(w, _height));

                x += w;
            }
        }
        private static              void                _validateText(string text)
        {
            for (int i = 0 ; i < text.Length ; ++i) {
                char c = text[i];

                if (c >= _charEncoding.Length || _charEncoding[c] == 0)
                    throw new ArgumentException("Invalid Text for barcode39.");
            }
        }
        private static              UInt16[]            _loadCharEncoding()
        {
            UInt16[] Encoding   = new UInt16[128];

            Encoding[(byte)'0'] = _barEncoding("NNNWWNWNN");
            Encoding[(byte)'1'] = _barEncoding("WNNWNNNNW");
            Encoding[(byte)'2'] = _barEncoding("NNWWNNNNW");
            Encoding[(byte)'3'] = _barEncoding("WNWWNNNNN");
            Encoding[(byte)'4'] = _barEncoding("NNNWWNNNW");
            Encoding[(byte)'5'] = _barEncoding("WNNWWNNNN");
            Encoding[(byte)'6'] = _barEncoding("NNWWWNNNN");
            Encoding[(byte)'7'] = _barEncoding("NNNWNNWNW");
            Encoding[(byte)'8'] = _barEncoding("WNNWNNWNN");
            Encoding[(byte)'9'] = _barEncoding("NNWWNNWNN");
            Encoding[(byte)'A'] = _barEncoding("NNWWNNWNN");
            Encoding[(byte)'B'] = _barEncoding("NNWNNWNNW");
            Encoding[(byte)'C'] = _barEncoding("WNWNNWNNN");
            Encoding[(byte)'D'] = _barEncoding("NNNNWWNNW");
            Encoding[(byte)'E'] = _barEncoding("WNNNWWNNN");
            Encoding[(byte)'F'] = _barEncoding("NNWNWWNNN");
            Encoding[(byte)'G'] = _barEncoding("NNNNNWWNW");
            Encoding[(byte)'H'] = _barEncoding("WNNNNWWNN");
            Encoding[(byte)'I'] = _barEncoding("NNWNNWWNN");
            Encoding[(byte)'J'] = _barEncoding("NNNNWWWNN");
            Encoding[(byte)'K'] = _barEncoding("WNNNNNNWW");
            Encoding[(byte)'L'] = _barEncoding("NNWNNNNWW");
            Encoding[(byte)'M'] = _barEncoding("WNWNNNNWN");
            Encoding[(byte)'N'] = _barEncoding("NNNNWNNWW");
            Encoding[(byte)'O'] = _barEncoding("WNNNWNNWN");
            Encoding[(byte)'P'] = _barEncoding("NNWNWNNWN");
            Encoding[(byte)'Q'] = _barEncoding("NNNNNNWWW");
            Encoding[(byte)'R'] = _barEncoding("WNNNNNWWN");
            Encoding[(byte)'S'] = _barEncoding("NNWNNNWWN");
            Encoding[(byte)'T'] = _barEncoding("NNNNWNWWN");
            Encoding[(byte)'U'] = _barEncoding("WWNNNNNNW");
            Encoding[(byte)'V'] = _barEncoding("NWWNNNNNW");
            Encoding[(byte)'W'] = _barEncoding("WWWNNNNNN");
            Encoding[(byte)'X'] = _barEncoding("NWNNWNNNW");
            Encoding[(byte)'Y'] = _barEncoding("WWNNWNNNN");
            Encoding[(byte)'Z'] = _barEncoding("NWWNWNNNN");
            Encoding[(byte)'-'] = _barEncoding("NWNNNNWNW");
            Encoding[(byte)'.'] = _barEncoding("WWNNNNWNN");
            Encoding[(byte)' '] = _barEncoding("NWWNNNWNN");
            Encoding[(byte)'$'] = _barEncoding("NWNWNWNNN");
            Encoding[(byte)'/'] = _barEncoding("NWNWNNNWN");
            Encoding[(byte)'+'] = _barEncoding("NWNNNWNWN");
            Encoding[(byte)'%'] = _barEncoding("NNNWNWNWN");

            return Encoding;
        }
        private static              UInt16              _barEncoding(string Encoding)
        {
            int     bits = 0;
            int     n    = 0;

            if (Encoding.Length != 9)
                throw new ArgumentException("Invalid Encoding.");

            for(int i = 0 ; i < 9 ; ++i) {
                bits = bits << 1;

                switch(Encoding[i])
                {
                case 'N':
                    break;

                case 'W':
                    ++n;
                    bits = bits | 1;
                    break;

                default:
                    throw new ArgumentException("Invalid Encoding.");
                }
            }

            if (n != 3)
                throw new ArgumentException("Invalid Encoding.");

            return (UInt16)bits;
        }
    }
}
