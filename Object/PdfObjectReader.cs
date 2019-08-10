using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Stream is not owned by this class.")]
    public sealed class PdfObjectReader: PdfObject
    {
        private readonly        PdfDictionary               _dictionary;
        private                 Stream                      _stream;
        private readonly        string                      _namedType;

        public  override        PdfValueType                Type        { get => PdfValueType.Object; }
        public  override        string                      NamedType   { get => _namedType;  }
        public                  PdfDictionary               Dictionary  { get => _dictionary; }
        public  override        bool                        hasStream   { get => true;        }

        internal                                            PdfObjectReader(PdfDictionary dictionary, PdfDataStreamReader stream)
        {
            _dictionary = dictionary;
            _stream     = stream;
            _namedType  = _dictionary.NamedType;
        }

        public                  void                        ReadAll()
        {
            if (_stream is PdfDataStreamReader) {
                _dictionary.pdfReadAll();
                _stream = new MemoryStream(GetStreamData(), false);
            }
        }
        public                  Stream                      GetStream()
        {
            return _stream;
        }
        public                  byte[]                      GetStreamData()
        {
            byte[]  data = new byte[(int)_stream.Length];

            if (_stream.Read(data, 0, data.Length) != data.Length)
                throw new PdfExceptionReader("EOF read while reading stream data.");

            return data;
        }
        public                  Stream                      GetUncompressStream()
        {
            var stream  = GetStream();
            var filter  = Dictionary.ValueByName<PdfName>("Filter", mandatory:false);

            if (filter != null) {
                stream = PdfFilter.GetDecompressor(filter.Value, stream);

                var decodeParms = Dictionary.ValueByName<PdfDictionary>("DecodeParms", mandatory:false);
                if (decodeParms != null)
                    stream = new PdfDecodeParmsDecoder(decodeParms, stream);
            }

            return stream;
        }
        public                  byte[]                      GetUncompressData()
        {
            using (MemoryStream OutputStream = new MemoryStream()) {
                using (Stream InputStream = GetUncompressStream()) {
                    int     rtn;
                    byte[]  buf = new byte[4096];

                    while ((rtn = InputStream.Read(buf, 0, 4096)) > 0)
                        OutputStream.Write(buf, 0, rtn);
                }

                OutputStream.Capacity = (int)OutputStream.Position;

                return OutputStream.GetBuffer();
            }
        }

        internal override       void                        pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            _dictionary.pdfWriteToDocument(document, writer);
            writer.WriteStream(GetStream(), (int)_stream.Length);
        }
    }
}
