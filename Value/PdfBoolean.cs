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
    public class PdfBoolean: PdfValue
    {
        public static readonly  PdfBoolean              True  = new PdfBoolean(true);
        public static readonly  PdfBoolean              False = new PdfBoolean(false);

        private                 bool                    _value;

        public override         PdfValueType            Type        { get { return PdfValueType.Boolean;    } }
        public                  bool                    Value       { get { return _value;                  } }

        public                                          PdfBoolean(bool value)
        {
            this._value = value;
        }

        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteBoolean(_value);
        }
    }
}
