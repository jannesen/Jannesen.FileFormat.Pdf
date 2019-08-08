using System;

namespace Jannesen.FileFormat.Pdf
{
    public static class PdfPageSize
    {
        public static readonly PdfSize      A0      = new PdfSize(2384, 3370);
        public static readonly PdfSize      A1      = new PdfSize(1684, 2384);
        public static readonly PdfSize      A2      = new PdfSize(1191, 1684);
        public static readonly PdfSize      A3      = new PdfSize( 842, 1191);
        public static readonly PdfSize      A4      = new PdfSize( 595,  842);
        public static readonly PdfSize      A5      = new PdfSize( 420,  595);
        public static readonly PdfSize      A6      = new PdfSize( 297,  420);
        public static readonly PdfSize      A7      = new PdfSize( 210,  297);
        public static readonly PdfSize      A8      = new PdfSize( 148,  210);
        public static readonly PdfSize      A9      = new PdfSize( 105,  148);
        public static readonly PdfSize      A10     = new PdfSize(  73,  105);
    }
}
