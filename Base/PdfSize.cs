/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

namespace Jannesen.FileFormat.Pdf
{
    public struct PdfSize
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

        public  override    string          ToString()
        {
            return "(" + width.ToString() + "," + height.ToString() + ")";
        }
    }
}
