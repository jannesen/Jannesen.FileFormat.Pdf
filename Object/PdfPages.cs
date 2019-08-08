using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public abstract class PdfPage_s: PdfObject
    {
        private                 PdfPage_s               _parent;

        public                  PdfPage_s               Parent
        {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }
    }

    public class PdfPages: PdfPage_s
    {
        public  const           int                     MaxKids = 16;

        private                 int                     _count;
        private readonly        List<PdfPage_s>         _pages;

        public                  int                     Count
        {
            get {
                return _count;
            }
            set {
                _count = value;
            }
        }
        public                  List<PdfPage_s>         Pages
        {
            get {
                return _pages;
            }
        }

        public                                          PdfPages()
        {
            _pages  = new List<PdfPage_s>(MaxKids);
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter Document, PdfStreamWriter Writer)
        {
            Writer.WriteDictionaryBegin();

            Writer.WriteName("Type");
            Writer.WriteName("Pages");

            if (Parent != null) {
                Writer.WriteName("Parent");
                Writer.WriteReference(Document, Parent);
            }

            Writer.WriteName("Count");
            Writer.WriteInteger(_count);

            Writer.WriteName("Kids");
            Writer.WriteArrayBegin();

            for (int i= 0 ; i < _pages.Count ; ++i)
                Writer.WriteReference(Document, _pages[i]);

            Writer.WriteArrayEnd();

            Writer.WriteDictionaryEnd();
        }
    }
}
