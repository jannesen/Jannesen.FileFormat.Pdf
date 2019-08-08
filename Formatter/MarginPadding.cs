using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Formatter
{
    public class MarginPadding
    {
        private                 PdfDistance         _top;
        private                 PdfDistance         _left;
        private                 PdfDistance         _right;
        private                 PdfDistance         _bottom;

        public                  PdfDistance         Top
        {
            get {
                return _top;
            }
            set {
                _top = value;
            }
        }
        public                  PdfDistance         Left
        {
            get {
                return _left;
            }
            set {
                _left = value;
            }
        }
        public                  PdfDistance         Right
        {
            get {
                return _right;
            }
            set {
                _right = value;
            }
        }
        public                  PdfDistance         Bottom
        {
            get {
                return _bottom;
            }
            set {
                _bottom = value;
            }
        }

        public                                      MarginPadding()
        {
        }
    }
}
