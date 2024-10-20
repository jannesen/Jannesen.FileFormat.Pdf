using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStyleText
    {
        private readonly            bool                _locked;
        private                     PdfFont             _font;
        private                     PdfDistance         _fontSize;
        private                     double              _lineSpacing;
        private                     PdfColor            _textColor;

        public                      PdfFont             Font
        {
            get {
                if (_font == null)
                    _font = PdfStandardFontFamily.Helvetica.Normal;

                return _font;
            }
            set {
                _checkLocked();
                _font = value;
            }
        }
        public                      PdfDistance         FontSize
        {
            get {
                return _fontSize;
            }
            set {
                if (value.pnts < 2.0)
                    throw new InvalidOperationException("FontSize is minimal 2.");

                _checkLocked();
                _fontSize  = value;
            }
        }
        public                      double              LineSpacing
        {
            get {
                return _lineSpacing;
            }
            set {
                if (value < 1.0)
                    throw new InvalidOperationException("LineSpacing is minimal 1.");

                _checkLocked();
                _lineSpacing = value;
            }
        }
        public                      PdfColor            TextColor
        {
            get {
                if (_textColor == null)
                    _textColor = new PdfColorCMYK();

                return _textColor;
            }
            set {
                _checkLocked();
                _textColor  = value;
            }
        }
        public                      PdfDistance         LineHeight
        {
            get {
                return _fontSize * _lineSpacing;
            }
        }
        public                      PdfDistance         YOffset
        {
            get {
                return _fontSize * ((_lineSpacing - 1.0) / 2.0 - ((double)_font.FontBBox.llY / 1000.0));
            }
        }
        public                      PdfDistance         YOffsetFromTop
        {
            get {
                return LineHeight - YOffset;
            }
        }

        public                                          PdfStyleText()
        {
            _locked      = false;
            _font        = null;
            _fontSize    = new PdfDistance(12.0);
            _lineSpacing = 1;
            _textColor   = null;
        }
        public                                          PdfStyleText(PdfStyleText style)
        {
            ArgumentNullException.ThrowIfNull(style);

            _locked      = false;
            _font        = style._font;
            _fontSize    = style._fontSize;
            _lineSpacing = style._lineSpacing;
            _textColor   = style._textColor;
        }
        public                                          PdfStyleText(PdfFont font, PdfDistance size)
        {
            ArgumentNullException.ThrowIfNull(font);

            _locked      = true;
            _font        = font;
            _fontSize    = size;
            _lineSpacing = 1.1;
            _textColor   = PdfColorCMYK.ncBlack;
        }
        public                                          PdfStyleText(PdfFont font, PdfDistance size, PdfColor textColor)
        {
            ArgumentNullException.ThrowIfNull(font);

            _locked      = true;
            _font        = font;
            _fontSize    = size;
            _lineSpacing = 1.1;
            _textColor   = textColor;
        }
        public                                          PdfStyleText(PdfFont font, PdfDistance size, double lineSpacing, PdfColor textColor)
        {
            ArgumentNullException.ThrowIfNull(font);

            _locked      = true;
            _font        = font;
            _fontSize    = size;
            _lineSpacing = lineSpacing;
            _textColor   = textColor;
        }

        public                      PdfDistance         CharWidth(char chr)
        {
            return _font.CharWidth(_fontSize, chr);
        }
        public                      PdfDistance         TextWidth(string text)
        {
            return _font.TextWidth(_fontSize, text);
        }

        private                     void                _checkLocked()
        {
            if (_locked)
                throw new PdfException("PdfStyleLine locked.");
        }
    }
}
