using System;

namespace Jannesen.FileFormat.Pdf
{
    public struct PdfSize: IEquatable<PdfSize>
    {
        public              PdfDistance     width;
        public              PdfDistance     height;

        public                              PdfSize(double width, double height)
        {
            this.width.pnts  = width;
            this.height.pnts = height;
        }
        public                              PdfSize(PdfDistance width, PdfDistance height)
        {
            this.width  = width;
            this.height = height;
        }

        public  static      bool    operator == (PdfSize p1, PdfSize p2)
        {
            return p1.width  == p2.width &&
                   p1.height == p2.height;
        }
        public  static      bool    operator != (PdfSize p1, PdfSize p2)
        {
            return !(p1 == p2);
        }
        public  override    bool    Equals(object obj)
        {
            if (obj is PdfSize)
                return this == (PdfSize)obj;

            return false;
        }
        public              bool    Equals(PdfSize o)
        {
            return this == o;
        }
        public  override    int     GetHashCode()
        {
            return width.GetHashCode() ^ width.GetHashCode();
        }

        public  override    string          ToString()
        {
            return "(" + width.ToString() + "," + height.ToString() + ")";
        }
    }
}
