using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfExceptionWriter : Exception
    {
        public      PdfExceptionWriter(string message) : base(message)
        {
        }
        public      PdfExceptionWriter(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
