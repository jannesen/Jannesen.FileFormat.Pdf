using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfExceptionReader : Exception
    {
        public      PdfExceptionReader(string Message) : base(Message)
        {
        }
        public      PdfExceptionReader(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
