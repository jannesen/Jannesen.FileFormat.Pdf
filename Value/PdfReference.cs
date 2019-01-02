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
    public class PdfWriterReference
    {
        public                  int                     Id                  { get; private set; }
        public                  int                     Position            { get ; set; }
        public                  int                     CompressedObjId     { get ; set; }

        public                                          PdfWriterReference(int id)
        {
            this.Id       = id;
        }
    }

    public class PdfReferenceReader: PdfValue
    {
        private                 PdfValue                _value;

        public                  PdfDocumentReader       Document            { get; private set; }
        public override         PdfValueType            Type                { get { return PdfValueType.Reference;  } }
        public                  int                     Id                  { get; private set; }
        public                  int                     Revision            { get; private set; }
        public                  int                     Position            { get; private set; }
        public                  PdfReferenceReader      CompressedObj       { get; private set; }

        public                  PdfValue                Value
        {
            get {
                if (_value == null) {
                    if (Position < 0)
                        throw new InvalidOperationException("Object #" + Id + " unavailable.");

                    _value = Document.pdfReadObj(this);
                }

                return _value;
            }
        }
        public                  PdfObjectReader         Object
        {
            get {
                var obj = Value as PdfObjectReader;

                if (obj == null)
                    throw new InvalidOperationException("Object expected.");

                return obj;
            }
        }

        public                                          PdfReferenceReader(PdfDocumentReader document, int id, int revision)
        {
            this.Document = document;
            this.Id       = id;
            this.Revision = revision;
            this.Position = -1;
        }
        public                                          PdfReferenceReader(PdfDocumentReader document, int id, int revision, int position)
        {
            this.Document = document;
            this.Id       = id;
            this.Revision = revision;
            this.Position = position;
        }
        public                                          PdfReferenceReader(PdfDocumentReader document, int id, int revision, int position, PdfReferenceReader compressObj)
        {
            this.Document      = document;
            this.Id            = id;
            this.Revision      = revision;
            this.Position      = position;
            this.CompressedObj = compressObj;
        }

        public  override        void                    pdfReadAll()
        {
            Value.pdfReadAll();
        }
        public  override        void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteReference(document, Value);
        }

        internal                void                    setPosition(int pos)
        {
            this.Position = pos;
        }
        internal                void                    setCompressed(int pos, PdfReferenceReader compressedObj)
        {
            this.Position      = pos;
            this.CompressedObj = compressedObj;
        }
        internal                void                    setValue(PdfValue value)
        {
            this._value = value;
        }
    }
}
