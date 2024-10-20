using System;

namespace Jannesen.FileFormat.Pdf
{
    public struct PdfPoint: IEquatable<PdfPoint>
    {
        public                      PdfDistance     x;
        public                      PdfDistance     y;

        public                                      PdfPoint(double x, double y)
        {
            this.x.pnts = x;
            this.y.pnts = y;
        }
        public                                      PdfPoint(PdfDistance x, PdfDistance y)
        {
            this.x = x;
            this.y = y;
        }

        public  readonly            PdfPoint        AddX(PdfDistance d)
        {
            return new PdfPoint(x + d, y);
        }
        public  readonly            PdfPoint        AddY(PdfDistance d)
        {
            return new PdfPoint(x, y + d);
        }
        public  readonly            PdfPoint        SubX(PdfDistance d)
        {
            return new PdfPoint(x - d, y);
        }
        public  readonly            PdfPoint        SubY(PdfDistance d)
        {
            return new PdfPoint(x, y - d);
        }
        public  readonly            PdfPoint        AddX(double d)
        {
            return new PdfPoint(x + d, y);
        }
        public  readonly            PdfPoint        AddY(double d)
        {
            return new PdfPoint(x, y + d);
        }
        public  readonly            PdfPoint        SubX(double d)
        {
            return new PdfPoint(x - d, y);
        }
        public  readonly            PdfPoint        SubY(double d)
        {
            return new PdfPoint(x, y - d);
        }
        public  static              bool            operator == (PdfPoint d1, PdfPoint d2)
        {
            return d1.x == d2.x
                && d1.y == d2.y;
        }
        public  static              bool            operator != (PdfPoint d1, PdfPoint d2)
        {
            return d1.x != d2.x
                || d1.y != d2.y;
        }
        public  static              PdfPoint        operator +  (PdfPoint Point, PdfSize Size)
        {
            return new PdfPoint(Point.x + Size.width, Point.y + Size.height);
        }
        public  static              PdfPoint        operator -  (PdfPoint Point, PdfSize Size)
        {
            return new PdfPoint(Point.x - Size.width, Point.y - Size.height);
        }
        public  static              PdfPoint        operator +  (PdfPoint p1, PdfPoint p2)
        {
            return new PdfPoint(p1.x + p2.x, p1.y + p2.y);
        }
        public  static              PdfSize         operator -  (PdfPoint p1, PdfPoint p2)
        {
            return new PdfSize(p2.x - p1.x, p2.y - p1.y);
        }

        public  override readonly   bool            Equals(object other)
        {
            if (other is PdfPoint) {
                if (x == ((PdfPoint)other).x
                 && y == ((PdfPoint)other).y)
                    return true;
            }

            return false;
        }
        public           readonly   bool            Equals(PdfPoint other)
        {
            return this == other;
        }
        public  override readonly   int             GetHashCode()
        {
            return ((int)(x.pnts) % 1000)
                 + ((int)(y.pnts) % 1000) * 1000;
        }

        public  override readonly   string          ToString()
        {
            return "(" + x.ToString() + "," + y.ToString() + ")";
        }
    }
}
