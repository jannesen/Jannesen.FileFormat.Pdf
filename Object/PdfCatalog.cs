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

        internal override       void                pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteDictionaryBegin();
            writer.WriteName("Type");
            writer.WriteName("Catalog");

            if (_pages != null) {
                writer.WriteName("Pages");
                writer.WriteReference(document, _pages);
            }

            writer.WriteName("ViewerPreferences");
            writer.WriteDictionaryBegin();
                writer.WriteName("PrintScaling");
                writer.WriteName("None");
            writer.WriteDictionaryEnd();

            writer.WriteName("PageLayout");
            writer.WriteName("SinglePage");

            writer.WriteDictionaryEnd();
        }
    }
}
