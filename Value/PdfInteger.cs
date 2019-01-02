/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfInteger: PdfValue
    {
        private                 int                     _value;

        public override         PdfValueType            Type        { get { return PdfValueType.Integer;    } }
        public                  int                     Value       { get { return _value;                  } }

        public                                          PdfInteger(int value)
        {
            this._value = value;
        }

        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteInteger(_value);
        }
    }
}
