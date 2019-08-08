using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class Border
    {
        private                 PdfStyleLine        _top;
        private                 PdfStyleLine        _left;
        private                 PdfStyleLine        _right;
        private                 PdfStyleLine        _bottom;

        public                  PdfStyleLine        All
        {
            set {
                _top    = value;
                _left   = value;
                _right  = value;
                _bottom = value;
            }
        }
        public                  PdfStyleLine        Top
        {
            get {
                return _top;
            }
            set {
                _top = value;
            }
        }
        public                  PdfStyleLine        Left
        {
            get {
                return _left;
            }
            set {
                _left = value;
            }
        }
        public                  PdfStyleLine        Right
        {
            get {
                return _right;
            }
            set {
                _right = value;
            }
        }
        public                  PdfStyleLine        Bottom
        {
            get {
                return _bottom;
            }
            set {
                _bottom = value;
            }
        }

        public                  PdfDistance         TopHeight
        {
            get {
                return _top != null ? _top.LineWidth : PdfDistance.Zero;
            }
        }
        public                  PdfDistance         LeftWidth
        {
            get {
                return _left != null ? _left.LineWidth : PdfDistance.Zero;
            }
        }
        public                  PdfDistance         RightWidth
        {
            get {
                return _right != null ? _right.LineWidth : PdfDistance.Zero;
            }
        }
        public                  PdfDistance         BottomHeight
        {
            get {
                return _bottom != null ? _bottom.LineWidth : PdfDistance.Zero;
            }
        }
        public                  PdfDistance         TotalWidth
        {
            get {
                PdfDistance width = new PdfDistance(0);

                if (_left != null)
                    width += _left.LineWidth;

                if (_right != null)
                    width += _right.LineWidth;

                return width;
            }
        }
        public                  PdfDistance         TotalHeight
        {
            get {
                PdfDistance height = new PdfDistance(0);

                if (_top != null)
                    height += _top.LineWidth;

                if (_bottom != null)
                    height += _bottom.LineWidth;

                return height;
            }
        }

        public                                      Border()
        {
        }

        internal                void                boxPrint(PdfPoint upperLeftCorner, PdfSize size, PrintBackground background)
        {
            if (_top != null)
                background.DrawLineHorizontal(_top,    upperLeftCorner + new PdfSize(0, -_top.LineWidth.pnts / 2), size.width);

            if (_left != null)
                background.DrawLineVertical(_left,     upperLeftCorner + new PdfSize(_left.LineWidth.pnts / 2, 0), size.height);

            if (_right != null)
                background.DrawLineVertical(_right,    upperLeftCorner + new PdfSize(size.width.pnts - _right.LineWidth.pnts / 2, 0), size.height);

            if (_bottom != null)
                background.DrawLineHorizontal(_bottom, upperLeftCorner + new PdfSize(0, size.height.pnts + _bottom.LineWidth.pnts / 2), size.width);
        }
    }
}
