/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

namespace Jannesen.FileFormat.Pdf
{
    public struct PdfDistance
    {
        public const        double          inc_mm = 25.4;
        public const        double          pnt_mm = 72 / inc_mm;

        public              double          pnts;

        public              double          mm
        {
            get {
                return (pnts / pnt_mm);
            }
            set {
                pnts = value * pnt_mm;
            }
        }
        public              double          inch
        {
            get {
                return (pnts / 72.0);
            }
            set {
                pnts = value * 72;
            }
        }
        public              bool            IsNaN
        {
            get {
                return double.IsNaN(pnts);
            }
        }

        public  static      PdfDistance     d_pnt(double pnt)
        {
            return new PdfDistance(pnt);
        }
        public  static      PdfDistance     d_mm(double mm)
        {
            return new PdfDistance(mm * pnt_mm);
        }
        public  static      PdfDistance     d_inch(double inch)
        {
            return new PdfDistance(inch * 72);
        }
        public  static      PdfDistance     Zero
        {
            get {
                return new PdfDistance(0.0);
            }
        }
        public  static      PdfDistance     NaN
        {
            get {
                return new PdfDistance(double.NaN);
            }
        }

        public                              PdfDistance(double pnts)
        {
            this.pnts = pnts;
        }

        public  static      bool            operator == (PdfDistance d1, PdfDistance d2)
        {
            return d1.pnts == d2.pnts;
        }
        public  static      bool            operator != (PdfDistance d1, PdfDistance d2)
        {
            return d1.pnts != d2.pnts;
        }
        public  static      bool            operator > (PdfDistance d1, PdfDistance d2)
        {
            return d1.pnts > d2.pnts;
        }
        public  static      bool            operator < (PdfDistance d1, PdfDistance d2)
        {
            return d1.pnts < d2.pnts;
        }
        public  static      PdfDistance     operator +  (PdfDistance d1, PdfDistance d2)
        {
            return new PdfDistance(d1.pnts + d2.pnts);
        }
        public  static      PdfDistance     operator -  (PdfDistance d1, PdfDistance d2)
        {
            return new PdfDistance(d1.pnts - d2.pnts);
        }
        public  static      PdfDistance     operator +  (PdfDistance d1, double d2)
        {
            return new PdfDistance(d1.pnts + d2);
        }
        public  static      PdfDistance     operator -  (PdfDistance d1, double d2)
        {
            return new PdfDistance(d1.pnts - d2);
        }
        public  static      PdfDistance     operator *  (PdfDistance d1, double d)
        {
            return new PdfDistance(d1.pnts * d);
        }
        public  static      PdfDistance     operator /  (PdfDistance d1, double d)
        {
            return new PdfDistance(d1.pnts / d);
        }
        public  static      PdfDistance     operator -  (PdfDistance d1)
        {
            return new PdfDistance(-d1.pnts);
        }

        public  override    bool            Equals(object other)
        {
            if (other is PdfDistance) {
                if (pnts == ((PdfDistance)other).pnts)
                    return true;
            }

            return false;
        }
        public  override    int             GetHashCode()
        {
            return (int)(pnts * 1000) % 1000000000;
        }
        public  override    string          ToString()
        {
            return pnts.ToString("f4", System.Globalization.NumberFormatInfo.InvariantInfo) + " pnt";
        }
    }
}
