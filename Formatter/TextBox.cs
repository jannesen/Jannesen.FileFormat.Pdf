/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TextBox: Box
    {
        private struct TextLine
        {
            public  int             StrBegin;
            public  int             StrLength;
            public  PdfDistance     Width;
            public  PdfDistance     Left;
        }

        private                 PdfDistance     _setWidth;
        private                 PdfDistance     _calcWidth;
        private                 PdfDistance     _calcHeight;
        private                 PdfStyleText    _style;
        private                 PdfTextAlign    _align;
        private                 int             _maxLines;
        private                 string          _text;
        private                 TextLine[]      _lines;

        public      override    PdfDistance     Width
        {
            get {
                if (_lines == null)
                    _format();

                return _calcWidth;
            }
            set {
                if (_setWidth != value) {
                    _lines    = null;
                    _setWidth = value;
                }
            }
        }
        public      override    PdfDistance     Height
        {
            get {
                if (_lines == null)
                    _format();

                return _calcHeight;
            }
        }
        public                  PdfStyleText    Style
        {
            get {
                return _style;
            }
            set {
                if (_style != value) {
                    _lines = null;
                    _style = value;
                }
            }
        }
        public                  PdfTextAlign    Align
        {
            get {
                return _align;
            }
            set {
                if (_align != value) {
                    _lines = null;
                    _align = value;
                }
            }
        }
        public                  int             MaxLines
        {
            get {
                return _maxLines;
            }
            set {
                _lines    = null;
                _maxLines = value;
            }
        }
        public                  string          Text
        {
            get {
                return _text;
            }
            set {
                if (_text != value) {
                    _lines = null;
                    _text  = value;
                }
            }
        }

        public                                  TextBox(PdfStyleText style, PdfTextAlign align, PdfDistance width, int maxLines, string text)
        {
            _setWidth = width;
            _style    = style;
            _align    = align;
            _maxLines = maxLines;
            _text     = text;
        }
        public                                  TextBox(PdfStyleText style, PdfTextAlign align, int maxLines, string text)
        {
            _setWidth = PdfDistance.NaN;
            _style    = style;
            _align    = align;
            _maxLines = maxLines;
            _text     = text;
        }

        public      override    void            Format()
        {
            if (_lines == null)
                _format();
        }

        public      override    void            boxPrintForground(PdfPoint upperLeftCorner, PdfContent content)
        {
            if (_lines == null)
                _format();

            PdfDistance LineHeight = _style.LineHeight;
            PdfDistance YOffset    = _style.YOffset;

            upperLeftCorner += new PdfPoint(Left, Top);

            content.SetTextStyle(_style);

            for(int l = 0 ; l < _lines.Length ; ++l) {
                upperLeftCorner.y.pnts -= LineHeight.pnts;
                content.SetTextMatrixH(upperLeftCorner + new PdfPoint(_lines[l].Left, YOffset));
                content.opShowText(_style.Font, Text, _lines[l].StrBegin, _lines[l].StrLength);
            }
        }

        private                 void            _format()
        {
            PdfDistance     spaceWidth = _style.Font.CharWidth(_style.FontSize, ' ');
            TextLine[]      lines      = new TextLine[1];
            int             curPos     = 0;
            int             sepPos     = -1;
            PdfDistance     curWidth   = new PdfDistance(0);
            PdfDistance     sepWidth   = new PdfDistance(0);

            lines[0].StrBegin  = 0;

            while (curPos < Text.Length) {
                char        Chr      = Text[curPos];

                if (Chr == '\n' || Chr == '\r') {
                    lines[lines.Length - 1].StrLength = curPos - lines[lines.Length - 1].StrBegin;
                    lines[lines.Length - 1].Width     = curWidth;
                    curWidth = new PdfDistance(0);

                    if (_maxLines > 0 && lines.Length >= _maxLines)
                        break;

                    Array.Resize<TextLine>(ref lines, lines.Length+1);

                    ++curPos;

                    if (Chr == '\r' && curPos < Text.Length && Text[curPos] == '\n')
                        ++curPos;

                    lines[lines.Length - 1].StrBegin = curPos;
                    sepPos   = -1;
                    sepWidth = new PdfDistance(0);
                }
                else {
                    PdfDistance ChrWidth = (Chr == ' ') ? spaceWidth : _style.Font.CharWidth(_style.FontSize, Chr);

                    if (!_setWidth.IsNaN &&  _setWidth.pnts > 0 && curWidth + ChrWidth > _setWidth) {
                        if (_maxLines == 0 || lines.Length < _maxLines) {
                            if (sepWidth == new PdfDistance(0)) {
                                if (curWidth > new PdfDistance(0)) {
                                    sepPos   = curPos;
                                    sepWidth = curWidth;
                                }
                                else {
                                    sepPos   = curPos + 1;
                                    sepWidth = Width;
                                }
                            }

                            lines[lines.Length - 1].StrLength = sepPos - lines[lines.Length - 1].StrBegin;
                            lines[lines.Length - 1].Width     = sepWidth;

                            Array.Resize<TextLine>(ref lines, lines.Length+1);

                            lines[lines.Length - 1].StrBegin = sepPos;
                            curPos   = sepPos;
                            sepPos   = -1;
                            curWidth = new PdfDistance(0);
                            sepWidth = new PdfDistance(0);
                        }
                        else {
                            lines[lines.Length - 1].StrLength = curPos - lines[lines.Length - 1].StrBegin;
                            lines[lines.Length - 1].Width     = curWidth;
                            curWidth = new PdfDistance(0);
                            break;
                        }
                    }
                    else {
                        ++curPos;
                        curWidth += ChrWidth;

                        if (Chr == ' ') {
                            sepPos   = curPos;
                            sepWidth = curWidth;
                        }
                    }
                }
            }

            if (curWidth.pnts > 0) {
                lines[lines.Length - 1].StrLength = curPos - lines[lines.Length - 1].StrBegin;
                lines[lines.Length - 1].Width     = curWidth;
            }

            _calcWidth = !_setWidth.IsNaN ? _setWidth : PdfDistance.Zero;

            for (int l = 0 ; l < lines.Length ; ++l) {
                lines[l].Left = new PdfDistance(0);

                if (lines[l].Width > _calcWidth)
                    _calcWidth = lines[l].Width;
            }

            switch(_align) {
            case PdfTextAlign.Center:
                for (int l = 0 ; l < lines.Length ; ++l)
                    lines[l].Left = (_calcWidth - lines[l].Width) / 2;
                break;

            case PdfTextAlign.Right:
                for (int l = 0 ; l < lines.Length ; ++l)
                    lines[l].Left = _calcWidth - lines[l].Width;
                break;
            }

            for (int l = 0 ; l < lines.Length ; ++l) {
                while (lines[l].StrLength > 0 && _text[lines[l].StrBegin + lines[l].StrLength - 1] == ' ') {
                    lines[l].Width -= spaceWidth;
                    --lines[l].StrLength;
                }

                while (lines[l].StrLength > 0 && _text[lines[l].StrBegin] == ' ') {
                    lines[l].Left += spaceWidth;
                    ++lines[l].StrBegin;
                    --lines[l].StrLength;
                }
            }

            _calcHeight = -Style.LineHeight * lines.Length;
            _lines      = lines;
        }
    }
}
