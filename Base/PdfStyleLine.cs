/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStyleLine
    {
        public  readonly static     PdfStyleLine        SolidBlack1pt = new PdfStyleLine(PdfColorCMYK.ncBlack, new PdfDistance(1.0), PdfLineCapStyle.Butt, PdfLineJoinStyle.Bevel, 1.0, null, new PdfDistance(0));

        private                     bool                _locked;
        private                     PdfColor            _lineColor;
        private                     PdfDistance         _lineWidth;
        private                     PdfLineCapStyle     _capStyle;
        private                     PdfLineJoinStyle    _joinStyle;
        private                     double              _miterLimit;
        private                     PdfDistance[]       _dashArray;
        private                     PdfDistance         _dashPhase;

        public                      PdfColor            LineColor
        {
            get {
                if (_lineColor == null)
                    _lineColor = new PdfColorCMYK();

                return _lineColor;
            }
            set {
                _checkLocked();
                _lineColor  = value;
            }
        }
        public                      PdfDistance         LineWidth
        {
            get {
                return _lineWidth;
            }
            set {
                _checkLocked();
                _lineWidth   = value;
            }
        }
        public                      PdfLineCapStyle     CapStyle
        {
            get {
                return _capStyle;
            }
            set {
                _checkLocked();
                _capStyle   = value;
            }
        }
        public                      PdfLineJoinStyle    JoinStyle
        {
            get {
                return _joinStyle;
            }
            set {
                _checkLocked();
                _joinStyle  = value;
            }
        }
        public                      double              MiterLimit
        {
            get {
                return _miterLimit;
            }
            set {
                _checkLocked();
                _miterLimit = value;
            }
        }
        public                      PdfDistance[]       DashArray
        {
            get {
                return _dashArray;
            }
            set {
                _checkLocked();
                _dashArray  = value;
            }
        }
        public                      PdfDistance         DashPhase
        {
            get {
                return _dashPhase;
            }
            set {
                _checkLocked();
                _dashPhase  = value;
            }
        }

        public                                          PdfStyleLine()
        {
            _locked     = false;
            _lineColor  = null;
            _lineWidth   = new PdfDistance(1.0);
            _capStyle   = PdfLineCapStyle.Butt;
            _joinStyle  = PdfLineJoinStyle.Bevel;
            _miterLimit = 1.0;
            _dashArray  = null;
            _dashPhase  = new PdfDistance(0);
        }
        public                                          PdfStyleLine(PdfStyleLine style)
        {
            _locked     = false;
            _lineColor  = style._lineColor;
            _lineWidth  = style._lineWidth;
            _capStyle   = style._capStyle;
            _joinStyle  = style._joinStyle;
            _miterLimit = style._miterLimit;
            _dashArray  = style._dashArray;
            _dashPhase  = style._dashPhase;
        }
        public                                          PdfStyleLine(PdfColor lineColor, PdfDistance lineWidth)
        {
            _locked     = true;
            _lineColor  = lineColor;
            _lineWidth  = lineWidth;
            _capStyle   = PdfLineCapStyle.Butt;
            _joinStyle  = PdfLineJoinStyle.Bevel;
            _miterLimit = 1.0;
            _dashArray  = null;
            _dashPhase  = new PdfDistance(0);
        }
        public                                          PdfStyleLine(PdfColor lineColor, PdfDistance lineWith, PdfLineCapStyle CapStyle, PdfLineJoinStyle joinStyle, double miterLimit, PdfDistance[] dashArray, PdfDistance dashPhase)
        {
            _locked     = true;
            _lineColor  = lineColor;
            _lineWidth  = LineWidth;
            _capStyle   = CapStyle;
            _joinStyle  = joinStyle;
            _miterLimit = miterLimit;
            _dashArray  = dashArray;
            _dashPhase  = dashPhase;
        }

        protected                   void                _checkLocked()
        {
            if (_locked)
                throw new PdfException("PdfStyleLine locked.");
        }
    }
}
