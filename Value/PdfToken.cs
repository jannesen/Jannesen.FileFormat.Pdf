using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfToken: PdfValue
    {
        public static readonly  PdfToken                Xref        = new PdfToken(PdfValueType.Xref);
        public static readonly  PdfToken                Trailer     = new PdfToken(PdfValueType.Trailer);
        public static readonly  PdfToken                StartXref   = new PdfToken(PdfValueType.StartXref);
        public static readonly  PdfToken                ObjectBegin = new PdfToken(PdfValueType.ObjectBegin);
        public static readonly  PdfToken                ObjectEnd   = new PdfToken(PdfValueType.ObjectEnd);
        public static readonly  PdfToken                StreamBegin = new PdfToken(PdfValueType.StreamBegin);
        public static readonly  PdfToken                StreamEnd   = new PdfToken(PdfValueType.StreamEnd);
        public static readonly  PdfToken                Reference   = new PdfToken(PdfValueType.Reference);

        private readonly        PdfValueType            _type;

        public override         PdfValueType            Type        { get { return _type;                   } }

        public                                          PdfToken(PdfValueType type)
        {
            this._type = type;
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            throw new PdfExceptionWriter("Can't write PdfToken.");
        }
    }
}
