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
    public class PdfArray: PdfValue
    {
        private                 PdfValueList            _children;

        public override         PdfValueType            Type        { get { return PdfValueType.Array;  } }
        public                  PdfValueList            Children    { get { return _children;           } }

        public                                          PdfArray(PdfStreamReader reader)
        {
            _children = new PdfValueList();
            _children.pdfReadChildren(reader, PdfValueType.ArrayEnd);
        }

        public  override        void                    pdfReadAll()
        {
            _children.pdfReadAll();
        }
        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteArrayBegin();
            _children.pdfWriteToDocument(document, writer);
            writer.WriteArrayEnd();
        }
    }
}
