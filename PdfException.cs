using System;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfException : Exception
    {
        public      PdfException(string message) : base(message)
        {
        }
        public      PdfException(string message, Exception InnerException) : base(message, InnerException)
        {
        }

        public  override    string          Source
        {
            get {
                return "Jannesen.FileFormat.Pdf";
            }
        }
    }
}
