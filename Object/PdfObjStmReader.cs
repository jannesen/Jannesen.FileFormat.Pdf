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
    public sealed class PdfObjStmReader: PdfObject
    {
        private                 int[]                       _ints;
        private                 byte[]                      _data;

        public  override        PdfValueType                Type        { get => PdfValueType.Object; }
        public  override        string                      NamedType   { get => "ObjStm"; }

        public                                              PdfObjStmReader(PdfDocumentReader documentReader, PdfObjectReader objReader)
        {
            int n2    = objReader.Dictionary.ValueByName<PdfInteger>("N", resolve:false).Value * 2;
            int first = objReader.Dictionary.ValueByName<PdfInteger>("First", resolve:false).Value;

            var stream = objReader.GetUncompressStream();

            {
                _ints = new int[n2];

                var buf = new byte[first];
                if (stream.Read(buf, 0, buf.Length) != buf.Length)
                    throw new PdfExceptionReader("EOF hile reading ObjStm integers");

                var reader = new PdfStreamReader(documentReader, new MemoryStream(buf, false), 0);

                for (int i = 0 ; i < n2 ; ++i)
                    _ints[i] = reader.ReadInt();
            }

            using (var sbuf = new MemoryStream())
            {
                stream.CopyTo(sbuf);
                _data = sbuf.ToArray();
            }
        }

        public                  Stream                      GetStream(int n, int id)
        {
            if (_ints[n * 2] != id)
                throw new PdfExceptionReader("XRef.id <> ObjStm.id");

            int pos = _ints[n * 2 + 1];
            int end = (n * 2 + 3) < _ints.Length ? _ints[n * 2 + 3] : _data.Length;

            return new PdfDataBufReader(_data, pos, end - pos);
        }
        public  override        void                        pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
