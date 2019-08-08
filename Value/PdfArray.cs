using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfArray: PdfValue
    {
        private readonly        PdfValueList            _children;

        public override         PdfValueType            Type        { get { return PdfValueType.Array;  } }
        public                  PdfValueList            Children    { get { return _children;           } }

        internal                                        PdfArray(PdfStreamReader reader)
        {
            _children = new PdfValueList();
            _children.pdfReadChildren(reader, PdfValueType.ArrayEnd);
        }

        internal override       void                    pdfReadAll()
        {
            _children.pdfReadAll();
        }
        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteArrayBegin();
            _children.pdfWriteToDocument(document, writer);
            writer.WriteArrayEnd();
        }
    }
}
