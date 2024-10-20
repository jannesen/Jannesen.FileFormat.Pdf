using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfColorCMYK : PdfColor
    {
        public  const               string              ColorSpaceName  = "DeviceCMYK";
        public  static  readonly    PdfColorCMYK        ncBlack         = new PdfColorCMYK(0, 0, 0, 1);
        public  static  readonly    PdfColorCMYK        ncWhite         = new PdfColorCMYK(0, 0, 0, 0);

        private readonly            bool                _locked;
        private                     double              _cyan;
        private                     double              _magenta;
        private                     double              _yellow;
        private                     double              _black;

        public  override            string              ColorSpace
        {
            get {
                return ColorSpaceName;
            }
        }
        public                      double              Cyan
        {
            get {
                return _cyan;
            }
            set {
                _CheckLocked();
                _cyan = value;
            }
        }
        public                      double              Magenta
        {
            get {
                return _magenta;
            }
            set {
                _CheckLocked();
                _magenta = value;
            }
        }
        public                      double              Yellow
        {
            get {
                return _yellow;
            }
            set {
                _CheckLocked();
                _yellow  = value;
            }
        }
        public                      double              Black
        {
            get {
                return _black;
            }
            set {
                _CheckLocked();
                _black   = value;
            }
        }

        public                                          PdfColorCMYK()
        {
            _locked  = false;
            _cyan    = 0;
            _magenta = 0;
            _yellow  = 0;
            _black   = 0;
        }
        public                                          PdfColorCMYK(PdfColorCMYK color)
        {
            ArgumentNullException.ThrowIfNull(color);

            _locked  = false;
            _cyan    = color._cyan;
            _magenta = color._magenta;
            _yellow  = color._yellow;
            _black   = color._black;
        }
        public                                          PdfColorCMYK(double cyan, double magenta, double yellow, double black)
        {
            _locked  = true;
            _cyan    = cyan;
            _magenta = magenta;
            _yellow  = yellow;
            _black   = black;
        }

        public  override            bool                Equals(object other)
        {
            if (other is PdfColorCMYK) {
                if (((PdfColorCMYK)other)._cyan    == Cyan
                 && ((PdfColorCMYK)other)._magenta == Magenta
                 && ((PdfColorCMYK)other)._yellow  == Yellow
                 && ((PdfColorCMYK)other)._black   == Black)
                    return true;
            }

            return false;
        }
        public  override            int                 GetHashCode()
        {
            return (((int)_cyan)             )
                 + (((int)_magenta) *     100)
                 + (((int)_yellow)  *   10000)
                 + (((int)_black)   * 1000000);
        }

        private                     void                _CheckLocked()
        {
            if (_locked)
                throw new PdfException("PdfColorCMYK locked.");
        }
    }
}
