using System;
using System.Runtime.Serialization;

namespace Jannesen.FileFormat.Pdf
{
    [Serializable]
    public class PdfExceptionReader : Exception
    {
        public      PdfExceptionReader(string Message) : base(Message)
        {
        }
        public      PdfExceptionReader(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected   PdfExceptionReader(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }
    }
}
