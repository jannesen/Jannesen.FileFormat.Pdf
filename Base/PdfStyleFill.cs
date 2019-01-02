/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStyleFill
    {
        public  readonly static     PdfStyleFill        NoFill      = null;
        public  readonly static     PdfStyleFill        SolidBlack  = new PdfStyleFill(PdfColorCMYK.ncBlack);
        public  readonly static     PdfStyleFill        SolidWhite  = new PdfStyleFill(PdfColorCMYK.ncWhite);

        private                     bool                _locked;
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
            _fillColor  = style._fillColor;
            _locked     = false;
        }
        public                                          PdfStyleFill(PdfColor fillColor)
        {
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
