using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStyleFill
    {
        public  readonly static     PdfStyleFill        NoFill      = null;
        public  readonly static     PdfStyleFill        SolidBlack  = new PdfStyleFill(PdfColorCMYK.ncBlack);
        public  readonly static     PdfStyleFill        SolidWhite  = new PdfStyleFill(PdfColorCMYK.ncWhite);

        private readonly            bool                _locked;
        private                     PdfColor            _fillColor;

        public                      PdfColor            FillColor
        {
            get {
                if (_fillColor == null)
                    _fillColor = new PdfColorCMYK();

                return _fillColor;
            }
            set {
                _checkLocked();
                _fillColor  = value;
            }
        }

        public                                          PdfStyleFill()
        {
            _fillColor  = null;
            _locked     = false;
        }
        public                                          PdfStyleFill(PdfStyleFill style)
        {
            if (style is null) throw new ArgumentNullException(nameof(style));

            _fillColor  = style._fillColor;
            _locked     = false;
        }
        public                                          PdfStyleFill(PdfColor fillColor)
        {
            if (fillColor is null) throw new ArgumentNullException(nameof(fillColor));

            _fillColor  = fillColor;
            _locked     = true;
        }

        private                     void                _checkLocked()
        {
            if (_locked)
                throw new PdfException("PdfStyleFill locked.");
        }
    }
}
