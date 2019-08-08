using System;
using System.Runtime.Serialization;

namespace Jannesen.FileFormat.Pdf
{
    [Serializable]
    public class PdfException : Exception
    {
        public      PdfException(string message) : base(message)
        {
        }
        public      PdfException(string message, Exception InnerException) : base(message, InnerException)
        {
        }
        protected   PdfException(SerializationInfo info, StreamingContext context): base(info, context)
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
