/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;

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

        public  override    string          Source
        {
            get {
                return "Jannesen.FileFormat.Pdf";
            }
        }
    }
}
