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
    public class PdfPage : PdfPage_s
    {
        private             PdfSize                 _pageSize;
        private             PdfRectangle            _cropBox;
        private             PdfRectangle            _bleedBox;
        private             PdfRectangle            _trimBox;
        private             PdfRectangle            _artBox;
        private             PdfContent              _content;

        public              PdfRectangle            CropBox
        {
            get {
                return _cropBox;
            }
            set {
                _cropBox = value;
            }
        }
        public              PdfRectangle            BleedBox
        {
            get {
                return _bleedBox;
            }
            set {
                _bleedBox = value;
            }
        }
        public              PdfRectangle            TrimBox
        {
            get {
                return _trimBox;
            }
            set {
                _trimBox = value;
            }
        }
        public              PdfRectangle            ArtBox
        {
            get {
                return _artBox;
            }
            set {
                _artBox = value;
            }
        }

        public                                      PdfPage(PdfSize pageSize, PdfContent content)
        {
            _pageSize    = pageSize;
            _cropBox.llx = PdfDistance.Zero;
            _cropBox.lly = PdfDistance.Zero;
            _cropBox.urx = pageSize.width;
            _cropBox.ury = pageSize.height;
            _bleedBox    = _cropBox;
            _trimBox     = _cropBox;
            _artBox      = _cropBox;
            _content     = content;
        }

        public  override    void                    pdfAddToDocument(PdfDocumentWriter document)
        {
            for (var curContent = this._content ; curContent != null ; curContent = curContent.Parent)
                document.AddObj(curContent);
        }
        public  override    void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteDictionaryBegin();
            {
                writer.WriteName("Type");
                writer.WriteName("Page");

                writer.WriteName("Parent");
                writer.WriteReference(document, Parent);

                writer.WriteName("MediaBox");
                writer.WriteArrayBegin();
                {
                    writer.WriteInteger(0);
                    writer.WriteInteger(0);
                    writer.WriteNumber(_pageSize.width.pnts);
                    writer.WriteNumber(_pageSize.height.pnts);
                }
                writer.WriteArrayEnd();

                writer.WriteName("CropBox");
                writer.WriteRectangle(_cropBox);

                if (_bleedBox != _cropBox) {
                    writer.WriteName("BleedBox");
                    writer.WriteRectangle(_bleedBox);
                }

                if (_trimBox != _cropBox) {
                    writer.WriteName("TrimBox");
                    writer.WriteRectangle(_trimBox);
                }

                if (_artBox != _cropBox) {
                    writer.WriteName("ArtBox");
                    writer.WriteRectangle(_artBox);
                }

                writer.WriteName("Contents");
                writer.WriteArrayBegin();

                for (var curContent = this._content ; curContent != null ; curContent = curContent.Parent)
                    writer.WriteReference(document.GetReference(curContent));

                writer.WriteArrayEnd();


                writer.WriteName("Resources");
                writer.WriteDictionaryBegin();
                {
                    writer.WriteName("ProcSet");
                    writer.WriteArrayBegin();
                    {
                        writer.WriteName("PDF");
                        writer.WriteName("Text");
                        writer.WriteName("ImageB");
                        writer.WriteName("ImageC");
                    }
                    writer.WriteArrayEnd();

                    _content.Resources.pdfWriteResources(document, writer);
                }
                writer.WriteDictionaryEnd();
            }
            writer.WriteDictionaryEnd();
        }
    }
}
