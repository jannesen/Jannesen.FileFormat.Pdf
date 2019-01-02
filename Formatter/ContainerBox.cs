/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class ContainerBox: Box
    {
        private                 MarginPadding       _margin;
        private                 Border              _border;
        private                 MarginPadding       _padding;
        private                 PdfStyleFill        _background;
        private                 BoxList             _children;
        private                 PdfDistance         _setWidth;
        private                 PdfDistance         _calcWidth;
        private                 PdfDistance         _calcHeight;

        public                  MarginPadding       Margin
        {
            get {
                if (_margin == null)
                    _margin = new MarginPadding();

                return _margin;
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
        public                  MarginPadding       Padding
        {
            get {
                if (_padding == null)
                    _padding = new MarginPadding();

                return _padding;
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
        public                  BoxList             Children
        {
            get {
                return _children;
            }
        }
        public      override    PdfDistance         Width
        {
            get {
                if (_calcWidth.IsNaN)
                    Format();

                return _calcWidth;
            }
            set {
                _setWidth    = value;
            }
        }
        public      override    PdfDistance         Height
        {
            get {
                if (_calcHeight.IsNaN)
                    Format();

                return _calcHeight;
            }
        }

        public                                      ContainerBox()
        {
            _border      = null;
            _children    = new BoxList(this);
            _calcWidth   = PdfDistance.NaN;
            _calcHeight  = PdfDistance.NaN;
            _setWidth    = PdfDistance.NaN;
        }

        public                  void                Append(Box box)
        {
            _children.Add(box);
        }
        public                  void                AddText(PdfStyleText style, PdfTextAlign align, int maxLines, string text)
        {
            TextBox TextBox = new TextBox(style, align, maxLines, text);
            Append(TextBox);
        }
        public                  void                AddText(PdfDistance left, PdfDistance top, PdfDistance width, PdfStyleText style, PdfTextAlign align, int maxLines, string text)
        {
            Append(new TextBox(style, align, maxLines, text)    { Fixed  = true, Left = left, Top = top, Width = width });
        }

        public      override    void                Format()
        {
            PdfDistance innerWidth = _setWidth;
            PdfDistance top        = PdfDistance.Zero;
            PdfDistance left       = PdfDistance.Zero;

            _calcWidth  = !_setWidth.IsNaN ? _setWidth : PdfDistance.Zero;
            _calcHeight = PdfDistance.Zero;

            if (_margin != null) {
                top  -= _margin.Top;
                left += _margin.Left;
            }

            if (_border != null) {
                top  -= _border.TopHeight;
                left += _border.LeftWidth;

                if (!innerWidth.IsNaN)
                    innerWidth -= _border.TotalWidth;
            }

            if (_padding != null) {
                top  -= _padding.Top;
                left += _padding.Left;
            }

            for (int i = 0 ; i < _children.Count ; ++i) {
                if (!_children[i].Fixed) {
                    _children[i].boxMoveTo(top, left);

                    if (!innerWidth.IsNaN)
                        _children[i].Width = innerWidth;

                    _children[i].Format();

                    top += _children[i].Height;
                }

                if (_calcWidth  < _children[i].Left + _children[i].Width)
                    _calcWidth  = _children[i].Left + _children[i].Width;

                if (_calcHeight > _children[i].Top  + _children[i].Height)
                    _calcHeight = _children[i].Top  + _children[i].Height;
            }

            if (_margin != null) {
                _calcWidth  += _margin.Right;
                _calcHeight -= _margin.Bottom;
            }

            if (_border != null) {
                _calcWidth  += _border.RightWidth;
                _calcHeight -= _border.BottomHeight;
            }

            if (_padding != null) {
                _calcWidth  += _padding.Right;
                _calcHeight -= _padding.Bottom;
            }
        }
        public      override    void                boxPrintBackground(PdfPoint upperLeftCorner, PrintBackground background)
        {
            upperLeftCorner += new PdfPoint(Left, Top);

            if (_border != null) {
                PdfPoint    point = upperLeftCorner;
                PdfSize     size  = new PdfSize(Width, Height);

                if (_margin != null) {
                    point.y     -= _margin.Top;
                    point.x     += _margin.Left;
                    size.width  -= _margin.Left + _margin.Right;
                    size.height += _margin.Top + _margin.Bottom;
                }

                _border.boxPrint(point, size, background);
            }

            for (int i = 0 ; i < _children.Count ; ++i)
                _children[i].boxPrintBackground(upperLeftCorner, background);
        }
        public      override    void                boxPrintForground(PdfPoint upperLeftCorner, PdfContent content)
        {
            upperLeftCorner += new PdfPoint(Left, Top);

            for (int i = 0 ; i < _children.Count ; ++i)
                _children[i].boxPrintForground(upperLeftCorner, content);
        }
    }
}
