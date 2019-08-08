using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class TableColumnRow
    {
        private                 MarginPadding       _margin;
        private                 MarginPadding       _padding;
        private                 Border              _border;
        private                 PdfStyleFill        _background;
        private                 PdfStyleText        _textStyle;
        private                 PdfTextAlign        _textAlign;
        private                 int                 _textMaxLines;

        public                  MarginPadding       Margin
        {
            get {
                if (_margin == null)
                    _margin = new MarginPadding();

                return _margin;
            }
        }
        public                  MarginPadding       Padding
        {
            get {
                if (_padding == null)
                    _padding = new MarginPadding();

                return _padding;
            }
        }
        public                  Border              Border
        {
            get {
                if (_border == null)
                    _border = new Border();

                return _border;
            }
        }
        public                  PdfStyleFill        Background
        {
            get {
                return _background;
            }
            set {
                _background = value;
            }
        }
        public                  PdfStyleText        TextStyle
        {
            get {
                return _textStyle;
            }
            set {
                _textStyle = value;
            }
        }
        public                  PdfTextAlign        TextAlign
        {
            get {
                return _textAlign;
            }
            set {
                _textAlign = value;
            }
        }
        public                  int                 TextMaxLines
        {
            get {
                return _textMaxLines;
            }
            set {
                _textMaxLines = value;
            }
        }

        public                  bool                hasMargin
        {
            get {
                return _margin != null;
            }
        }
        public                  bool                hasBorder
        {
            get {
                return _border != null;
            }
        }
        public                  bool                hasPadding
        {
            get {
                return _padding != null;
            }
        }

        protected                                   TableColumnRow()
        {
            _textAlign    = PdfTextAlign.Unknown;
            _textMaxLines = 0;
        }
    }
}
