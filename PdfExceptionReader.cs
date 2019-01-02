/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

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
    }
}
