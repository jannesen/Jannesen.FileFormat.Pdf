using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public sealed class PdfDocumentWriter: PdfDocument, IDisposable
    {
        private             PdfStreamWriter                             _writer;
        private             bool                                        _compressContent;       // Compress content
        private readonly    bool                                        _closeOnFinish;         // Close stream on finise
        private             List<PdfWriterReference>                    _xrefTable;             // xref Table.
        private readonly    Dictionary<PdfValue, PdfWriterReference>    _referenceTable;
        private             PdfDocumentInfo                             _documentInfo;          // Document info obj.
        private             PdfCatalog                                  _catalog;               // Catalog obj.
        private             List<PdfPage_s>                             _pages;                 // Pages
        private             List<PdfValue>                              _objToWrite;            // List of obj to write

        public              PdfStreamWriter         Writer
        {
            get {
                return _writer;
            }
        }
        public              bool                    CompressContent
        {
            get {
                return _compressContent;
            }
            set {
                _compressContent = value;
            }
        }
        public              PdfDocumentInfo         DocumentInfo
        {
            get {
                return _documentInfo;
            }
        }

        public                                      PdfDocumentWriter(Stream stream, bool closeOnFinish)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            try {
                _writer           = new PdfStreamWriter(stream);
                _compressContent  = true;
                _closeOnFinish    = closeOnFinish;
                _xrefTable        = new List<PdfWriterReference>();
                _referenceTable   = new Dictionary<PdfValue, PdfWriterReference>();
                _documentInfo     = new PdfDocumentInfo();
                _catalog          = new PdfCatalog();
                _pages            = new List<PdfPage_s>();
                _objToWrite   = new List<PdfValue>();
                _xrefTable.Add(null);

                _writer.WriteFileHeader();
            }
            catch(Exception) {
                if (closeOnFinish)
                    stream.Close();

                throw;
            }
        }

        public              void                    Dispose()
        {
            if (_writer != null) {
                _writer.PdfStream.Flush();

                if (_closeOnFinish)
                    _writer.PdfStream.Close();

                _writer = null;
            }

            if (_xrefTable != null) {
                _xrefTable.Clear();
                _xrefTable      = null;
                _documentInfo   = null;
                _catalog        = null;
                _pages          = null;
                _objToWrite = null;
            }

        }
        public              void                    Close()
        {
            if (_writer != null) {
                if (_pages.Count == 0)
                    throw new PdfExceptionWriter("Document has no page(s).");

                List<PdfPage_s>     IntermediateNodes = _createIntermediateNodes(_pages);

                while (IntermediateNodes.Count > 1)
                    IntermediateNodes = _createIntermediateNodes(IntermediateNodes);

                _catalog.Pages = (PdfPages)IntermediateNodes[0];
                AddObj(_catalog);
                AddObj(_documentInfo);
                _flushObjects();
                _writeTrailer();
            }

            Dispose();
        }

        public              void                    AddPage(PdfSize pageSize, PdfContent content)
        {
            AddObj(new PdfPage(pageSize, content));
        }
        internal            PdfWriterReference      AddObj(PdfValue obj)
        {
            if (!_referenceTable.TryGetValue(obj, out var reference)) {
                obj.pdfAddToDocument(this);

                reference = new PdfWriterReference(_xrefTable.Count);
                _xrefTable.Add(reference);
                _referenceTable.Add(obj, reference);

                if (obj is PdfPage)
                    _pages.Add((PdfPage)obj);

                if (obj.hasStream) {
                    reference.Position = _writer.PdfPosition;
                    _writer.WriteObj(this, reference, obj);
                }
                else
                    _objToWrite.Add(obj);
            }

            return reference;
        }
        internal            PdfWriterReference      GetReference(PdfValue obj)
        {
            return _referenceTable[obj];
        }

        private             void                    _flushObjects()
        {
            int     pos = 0;

            while (pos < _objToWrite.Count) {
                if (pos < _objToWrite.Count - 1 && _compressContent) {
                    var objStm    = new PdfObjStmWriter();
                    var objStmRef = new PdfWriterReference(_xrefTable.Count);
                    _xrefTable.Add(objStmRef);
                    _referenceTable.Add(objStm, objStmRef);

                    int n = 0;

                    while (pos < _objToWrite.Count && n < 250) {
                        var obj = _objToWrite[pos++];
                        var r   = _referenceTable[obj];
                        r.CompressedObjId = objStmRef.Id;
                        r.Position        = objStm.AddObj(this, r, obj);
                        n++;
                    }

                    objStmRef.Position  = _writer.PdfPosition;
                    _writer.WriteObj(this, objStmRef, objStm);
                }
                else
                    _writeObject(_objToWrite[pos++]);
            }

            _objToWrite.Clear();
        }
        private             void                    _writeObject(PdfValue obj)
        {
            var r = _referenceTable[obj];
            r.Position = _writer.PdfPosition;
            _writer.WriteObj(this, r, obj);
        }
        private             void                    _writeTrailer14()
        {
        // Write xref table
            int     posXrefTable = _writer.PdfPosition;

            _writer.WriteXrefHeader(_xrefTable.Count);

            {
                int             v;
                int             p;
                byte[]          buf   = new byte[20];

                for(int ObjectID = 0 ; ObjectID < _xrefTable.Count ; ++ObjectID) {
                    PdfWriterReference  entry = _xrefTable[ObjectID];

                    v = (entry != null) ? entry.Position : 0;
                    for (p = 0 ; p < 10 ; ++p) {
                        buf[9 - p] = (byte)('0' + (v % 10));
                        v /= 10;
                    }

                    buf[10] = (byte)' ';

                    v = (entry != null) ? 0 : 65535;
                    for (p = 0 ; p < 5 ; ++p) {
                        buf[15 - p] = (byte)('0' + (v % 10));
                        v /= 10;
                    }

                    buf[16] = (byte)' ';
                    buf[17] = (byte)((entry != null) ? 'n' : 'f');
                    buf[18] = (byte)' ';
                    buf[19] = (byte)'\n';

                    _writer.WriteByteArray(buf, 0, 20);
                }
            }

        // write trailer
            _writer.WriteTrailer();
            _writer.WriteDictionaryBegin();
            {
                // Size
                {
                    _writer.WriteName("Size");
                    _writer.WriteInteger(_xrefTable.Count);
                }

                // Root
                {
                    _writer.WriteName("Root");
                    _writer.WriteReference(GetReference(_catalog));
                }

                // Info
                {
                    _writer.WriteName("Info");
                    _writer.WriteReference(GetReference(_documentInfo));
                }

                // ID
                {
                    _writer.WriteName("ID");
                    _writer.WriteArrayBegin();
                        _writer.WriteStringHex(System.Guid.NewGuid().ToByteArray());
                        _writer.WriteStringHex(System.Guid.NewGuid().ToByteArray());
                    _writer.WriteArrayEnd();
                }
            }
            _writer.WriteDictionaryEnd();
            _writer.WriteNewLine();

        // write startxref and EOF
            _writer.WriteEOF(posXrefTable);
        }
        private             void                    _writeTrailer()
        {
            using (var compressStream = new StreamBuffer()) {
                var posXrefTable = _writer.PdfPosition;
                var objref       = new PdfWriterReference(_xrefTable.Count) { Position  = _writer.PdfPosition };
                _xrefTable.Add(objref);

                string  filter = "FlateDecode";

                using (var compressWriter = PdfFilter.GetCompressor(filter, compressStream)) {
                    byte[]  buf = new byte[6];

                    for (int i = 0 ; i < _xrefTable.Count ; ++i) {
                        var e = _xrefTable[i];

                        if (e != null) {
                            if (e.CompressedObjId > 0) {
                                buf[0] = 2;
                                buf[1] = (byte)(e.CompressedObjId >> 24);
                                buf[2] = (byte)(e.CompressedObjId >> 16);
                                buf[3] = (byte)(e.CompressedObjId >>  8);
                                buf[4] = (byte)(e.CompressedObjId      );
                                buf[5] = (byte)(e.Position             );
                            } else {
                                buf[0] = 1;
                                buf[1] = (byte)(e.Position >> 24);
                                buf[2] = (byte)(e.Position >> 16);
                                buf[3] = (byte)(e.Position >>  8);
                                buf[4] = (byte)(e.Position      );
                                buf[5] = 0;
                            }
                        } else {
                            buf[0] = 0;
                            buf[1] = 0;
                            buf[2] = 0;
                            buf[3] = 0;
                            buf[4] = 0;
                            buf[5] = 255;
                        }

                        compressWriter.Write(buf, 0, buf.Length);
                    }
                }

                _writer.WriteInteger(objref.Id);
                _writer.WriteInteger(0);
                _writer.WriteObjBegin();
                _writer.WriteDictionaryBegin();
                _writer.WriteName("Type");
                    _writer.WriteName("XRef");

                _writer.WriteName("W");
                    _writer.WriteArrayBegin();
                        _writer.WriteInteger(1);
                        _writer.WriteInteger(4);
                        _writer.WriteInteger(1);
                    _writer.WriteArrayEnd();

                _writer.WriteName("Index");
                    _writer.WriteArrayBegin();
                        _writer.WriteInteger(0);
                        _writer.WriteInteger(_xrefTable.Count);
                    _writer.WriteArrayEnd();

                _writer.WriteName("Size");
                    _writer.WriteInteger(_xrefTable.Count);

                _writer.WriteName("Root");
                    _writer.WriteReference(GetReference(_catalog));

                _writer.WriteName("Info");
                    _writer.WriteReference(GetReference(_documentInfo));

                _writer.WriteName("ID");
                    _writer.WriteArrayBegin();
                        _writer.WriteStringHex(System.Guid.NewGuid().ToByteArray());
                        _writer.WriteStringHex(System.Guid.NewGuid().ToByteArray());
                    _writer.WriteArrayEnd();

                _writer.WriteName("Filter");
                    _writer.WriteName(filter);

                _writer.WriteName("Length");
                    _writer.WriteInteger((int)(compressStream.Length));

                _writer.WriteDictionaryEnd();

                _writer.WriteStream(compressStream.GetBuffer(), (int)(compressStream.Length));

                _writer.WriteObjEnd();
                _writer.WriteNewLine();
                _writer.WriteEOF(posXrefTable);
            }
        }
        private             List<PdfPage_s>         _createIntermediateNodes(List<PdfPage_s> list)
        {
            int                     numberOfLeafs             = list.Count;
            int                     numberOfIntermediateNodes = 1 + ((numberOfLeafs - 1) / PdfPages.MaxKids);
            List<PdfPage_s>         intermediateNodes         = new List<PdfPage_s>(numberOfIntermediateNodes);
            PdfPages                currentIntermediateNode   = null;
            int                     numberOfLeafsInCurrentIntermediateNode = 16;
            int                     intermediateNodeNumber    = 0;

            for (int i = 0 ; i < numberOfLeafs ; ++i) {
                PdfPage_s       pageLink = list[i];

                if (currentIntermediateNode == null) {
                    currentIntermediateNode = new PdfPages();
                    ++intermediateNodeNumber;
                    intermediateNodes.Add(currentIntermediateNode);
                    AddObj(currentIntermediateNode);
                    numberOfLeafsInCurrentIntermediateNode = ((numberOfLeafs * intermediateNodeNumber) / numberOfIntermediateNodes)
                                                           - ((numberOfLeafs * (intermediateNodeNumber - 1)) / numberOfIntermediateNodes);
                }

                pageLink.Parent = currentIntermediateNode;
                currentIntermediateNode.Pages.Add(pageLink);
                currentIntermediateNode.Count += (pageLink is PdfPages) ? ((PdfPages)pageLink).Count : 1;

                if (currentIntermediateNode.Pages.Count >= numberOfLeafsInCurrentIntermediateNode)
                    currentIntermediateNode = null;
            }

            return intermediateNodes;
        }
    }
}
