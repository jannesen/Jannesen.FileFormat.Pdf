/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfCatalog : PdfObject
    {
        private                 PdfPages            _pages;

        public                  PdfPages            Pages
        {
            get {
                return _pages;
            }
            set {
                _pages = value;
            }
        }

        public                                      PdfCatalog()
        {
        }

        public  override    void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter Writer)
        {
            Writer.WriteDictionaryBegin();
            Writer.WriteName("Type");
            Writer.WriteName("Catalog");

            if (_pages != null) {
                Writer.WriteName("Pages");
                Writer.WriteReference(document, _pages);
            }

            Writer.WriteName("ViewerPreferences");
            Writer.WriteDictionaryBegin();
                Writer.WriteName("PrintScaling");
                Writer.WriteName("None");
            Writer.WriteDictionaryEnd();

            Writer.WriteName("PageLayout");
            Writer.WriteName("SinglePage");

            Writer.WriteDictionaryEnd();
        }
    }
}
