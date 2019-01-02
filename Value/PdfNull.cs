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
    public class PdfNull: PdfValue
    {
        public static readonly  PdfNull                 Null = new PdfNull();

        public override         PdfValueType            Type        { get { return PdfValueType.Null;       } }

        public                                          PdfNull()
        {
        }

        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteNull();
        }
    }
}
