using System;

#pragma warning disable CA1805 // Do not initialize unnecessarily

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
            ArgumentNullException.ThrowIfNull(style);

            _fillColor  = style._fillColor;
            _locked     = false;
        }
        public                                          PdfStyleFill(PdfColor fillColor)
        {
            ArgumentNullException.ThrowIfNull(fillColor);

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
