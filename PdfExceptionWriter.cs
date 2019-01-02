/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

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
    }
}
