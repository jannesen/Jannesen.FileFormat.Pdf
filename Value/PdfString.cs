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
    public class PdfString: PdfValue
    {
        private                 bool                    _hexadecimal;
        private                 byte[]                  _value;

        public override         PdfValueType            Type        { get { return PdfValueType.String;     } }
        public                  bool                    Hexadecimal { get { return _hexadecimal;            } }
        public                  byte[]                  Value       { get { return _value;                  } }

        public                                          PdfString(bool hexadecimal, byte[] value)
        {
            this._hexadecimal = hexadecimal;
            this._value       = value;
        }

        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            if (_hexadecimal)
                writer.WriteStringHex(_value);
            else
                writer.WriteStringRaw(_value);
        }
    }
}
