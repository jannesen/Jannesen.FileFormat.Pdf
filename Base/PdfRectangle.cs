/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

namespace Jannesen.FileFormat.Pdf
{
    public struct PdfRectangle
    {
        public              PdfDistance     llx;
        public              PdfDistance     lly;
        public              PdfDistance     urx;
        public              PdfDistance     ury;

        public                              PdfRectangle(double llx, double lly, double urx, double ury)
        {
            this.llx.pnts = llx;
            this.lly.pnts = lly;
            this.urx.pnts = urx;
            this.ury.pnts = ury;
        }
        public                              PdfRectangle(PdfDistance llx, PdfDistance lly, PdfDistance urx, PdfDistance ury)
        {
            this.llx = llx;
            this.lly = lly;
            this.urx = urx;
            this.ury = ury;
        }

        public  static      bool            operator == (PdfRectangle r1, PdfRectangle r2)
        {
            return r1.llx.pnts == r2.llx.pnts
                && r1.lly.pnts == r2.lly.pnts
                && r1.urx.pnts == r2.urx.pnts
                && r1.ury.pnts == r2.ury.pnts;
        }
        public  static      bool            operator != (PdfRectangle r1, PdfRectangle r2)
        {
            return r1.llx.pnts != r2.llx.pnts
                || r1.lly.pnts != r2.lly.pnts
                || r1.urx.pnts != r2.urx.pnts
                || r1.ury.pnts != r2.ury.pnts;
        }

        public  override    bool            Equals(object other)
        {
            if (other is PdfRectangle) {
                if (this == ((PdfRectangle)other))
                    return true;
            }

            return false;
        }
        public  override    int             GetHashCode()
        {
            return (int)((llx.pnts + lly.pnts + urx.pnts + ury.pnts) * 1000) % 1000000000;
        }
        public  override    string          ToString()
        {
            return "(" + llx.ToString() + "," + lly.ToString() + urx.ToString() + "," + ury.ToString() + ")";
        }
    }
}
