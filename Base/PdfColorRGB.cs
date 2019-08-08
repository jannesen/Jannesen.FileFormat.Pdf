using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfColorRGB : PdfColor
    {
        public  const               string              ColorSpaceName  = "DeviceRGB";
        public  static  readonly    PdfColorRGB         ncBlack         = new PdfColorRGB(  0,   0,   0);
        public  static  readonly    PdfColorRGB         ncRed           = new PdfColorRGB(100,   0,   0);
        public  static  readonly    PdfColorRGB         ncGreen         = new PdfColorRGB(  0, 100,   0);
        public  static  readonly    PdfColorRGB         ncBlue          = new PdfColorRGB(  0,   0, 100);
        public  static  readonly    PdfColorRGB         ncWhite         = new PdfColorRGB(100, 100, 100);

        private readonly            bool                _locked;
        private                     double              _red;
        private                     double              _green;
        private                     double              _blue;

        public  override            string              ColorSpace
        {
            get {
                return ColorSpaceName;
            }
        }
        public                      double              Red
        {
            get {
                return _red;
            }
            set {
                _CheckLocked();
                _red   = value;
            }
        }
        public                      double              Green
        {
            get {
                return _green;
            }
            set {
                _CheckLocked();
                _green = value;
            }
        }
        public                      double              Blue
        {
            get {
                return _blue;
            }
            set {
                _CheckLocked();
                _blue  = value;
            }
        }

        public                                          PdfColorRGB()
        {
            _locked = false;
            _red    = 0;
            _green  = 0;
            _blue   = 0;
        }
        public                                          PdfColorRGB(PdfColorRGB color)
        {
            if (color is null) throw new ArgumentNullException(nameof(color));

            _locked = false;
            _red    = color._red;
            _green  = color._green;
            _blue   = color._blue;
        }
        public                                          PdfColorRGB(double red, double green, double blue)
        {
            _locked = true;
            _red   = red;
            _green = green;
            _blue  = blue;
        }

        public  override            bool                Equals(object other)
        {
            if (other is PdfColorRGB) {
                if (((PdfColorRGB)other)._red   == Red
                 && ((PdfColorRGB)other)._green == Green
                 && ((PdfColorRGB)other)._blue  == Blue)
                    return true;
            }

            return false;
        }
        public  override            int                 GetHashCode()
        {
            return (((int)_red)          )
                 + (((int)_green)   * 100)
                 + (((int)_blue)  * 10000);
        }

        private                     void                _CheckLocked()
        {
            if (_locked)
                throw new PdfException("PdfColorRGB locked.");
        }
    }
}
