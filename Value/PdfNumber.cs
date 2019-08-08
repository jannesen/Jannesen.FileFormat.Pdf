using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfNumber: PdfValue
    {
        private readonly        double                  _value;

        public override         PdfValueType            Type        { get { return PdfValueType.Number;     } }
        public                  double                  Value       { get { return _value;                  } }

        public                                          PdfNumber(double value)
        {
            this._value = value;
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteNumber(_value);
        }
    }
}
