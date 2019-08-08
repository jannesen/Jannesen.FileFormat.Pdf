using System;
using System.Runtime.Serialization;

namespace Jannesen.FileFormat.Pdf
{
    [Serializable]
    public class PdfExceptionWriter : Exception
    {
        public      PdfExceptionWriter(string message) : base(message)
        {
        }
        public      PdfExceptionWriter(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected   PdfExceptionWriter(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }
    }
}
