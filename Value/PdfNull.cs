using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfNull: PdfValue
    {
        public static readonly  PdfNull                 Null = new PdfNull();

        public override         PdfValueType            Type        { get { return PdfValueType.Null;       } }

        public                                          PdfNull()
        {
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteNull();
        }
    }
}
