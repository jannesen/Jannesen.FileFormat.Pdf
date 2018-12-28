/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public sealed class PdfObjStmWriter: PdfObject
    {
        private                 int                         _n;
        private                 StreamBuffer                _intBuffer;
        private                 PdfStreamWriter             _intWriter;
        private                 StreamBuffer                _dataBuffer;
        private                 PdfStreamWriter             _dataWriter;

        public  override        PdfValueType                Type        { get => PdfValueType.Object; }
        public  override        string                      NamedType   { get => "ObjStm"; }
        public  override        bool                        hasStream   { get => true; }

        public                                              PdfObjStmWriter()
        {
            _n          = 0;
            _intWriter  = new PdfStreamWriter(_intBuffer  = new StreamBuffer());
            _dataWriter = new PdfStreamWriter(_dataBuffer = new StreamBuffer());
        }

        public                  int                         AddObj(PdfDocumentWriter document, PdfWriterReference reference, PdfValue obj)
        {
            _intWriter.WriteInteger(reference.Id);
            _intWriter.WriteInteger((int)_dataBuffer.Length);
            obj.pdfWriteToDocument(document, _dataWriter.ResetState());
            return _n++;
        }

        public  override        void                        pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            using (var compressStream = new StreamBuffer())
            {
                string  filter = "FlateDecode";

                using (var compressWriter = PdfFilter.GetCompressor(filter, compressStream))
                {
                    compressWriter.Write(_intBuffer.GetBuffer(),  0, (int)_intBuffer.Length);
                    compressWriter.Write(_dataBuffer.GetBuffer(), 0, (int)_dataBuffer.Length);
                }

                writer.WriteDictionaryBegin();
                    writer.WriteName("Type");
                        writer.WriteName(NamedType);

                    writer.WriteName("N");
                        writer.WriteInteger(_n);

                    writer.WriteName("First");
                        writer.WriteInteger((int)_intBuffer.Length);

                    writer.WriteName("Filter");
                        writer.WriteName(filter);

                    writer.WriteName("Length");
                        writer.WriteInteger((int)compressStream.Length);
                writer.WriteDictionaryEnd();

                writer.WriteStream(compressStream.GetBuffer(), (int)compressStream.Length);
            }

            // Release datastream for less memory usage
            _intBuffer.Dispose();
            _dataBuffer.Dispose();
            _intBuffer  = null;
            _dataBuffer = null;
            _intWriter  = null;
            _dataWriter = null;
        }
    }
}
