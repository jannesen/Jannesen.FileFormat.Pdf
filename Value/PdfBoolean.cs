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

        private readonly        bool                    _value;

        public override         PdfValueType            Type        { get { return PdfValueType.Boolean;    } }
        public                  bool                    Value       { get { return _value;                  } }

        public                                          PdfBoolean(bool value)
        {
            this._value = value;
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteBoolean(_value);
        }
    }
}
