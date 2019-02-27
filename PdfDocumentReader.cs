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
    public sealed class PdfDocumentReader: PdfDocument, IDisposable
    {
        private             Stream                      _stream;
        private             bool                        _leaveOpen;             // Close stream on finise
        private             int                         _fileVersion;
        private             PdfReferenceReader[]        _xrefTable;
        private             PdfReferenceReader          _rootReference;
        private             PdfReferenceReader          _encryptReference;
        private             PdfReferenceReader          _infoReference;
        private             PdfValue[]                  _id;
        private             PdfDictionary[]             _pages;

        public              PdfDictionary               EncryptDictionary
        {
            get {
                return (PdfDictionary)_encryptReference?.Value;
            }
        }
        public              PdfDictionary               InfoDictionary
        {
            get {
                return (PdfDictionary)_infoReference?.Value;
            }
        }
        public              PdfDictionary               RootDictionary
        {
            get {
                if (_rootReference == null)
                    throw new InvalidOperationException("Root object not defined.");

                return (PdfDictionary)_rootReference.Value;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public              PdfDictionary               PagesRoot
        {
            get {
                var pages = RootDictionary.ValueByName<PdfDictionary>("Pages");

                if (pages.NamedType != "Pages")
                    throw new PdfExceptionReader("No pages in document catalog.");

                return pages;
            }
        }
        public              PdfDictionary[]             Pages
        {
            get {
                if (_pages == null)
                    _constructPages();

                return _pages;
            }
        }
        public              PdfValue[]                  ID
        {
            get {
                return _id;
            }
        }
        public              PdfReferenceReader[]        Objects
        {
            get {
                int     objectCount = 0;
                int     objectPos   = 0;

                for (int i = 0 ; i < _xrefTable.Length ; ++i)
                    if (_xrefTable[i] != null)
                        ++objectCount;

                var objects = new PdfReferenceReader[objectCount];

                for (int i = 0 ; i < _xrefTable.Length ; ++i) {
                    if (_xrefTable[i] != null)
                        objects[objectPos++] = _xrefTable[i];
                }

                return objects;
            }
        }

        public                                          PdfDocumentReader(string fileName) : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, false), false)
        {
        }
        public                                          PdfDocumentReader(Stream stream, bool leaveOpen)
        {
            try {
                if (!stream.CanSeek) {
                    stream    = _copyStreamToMemory(stream, leaveOpen);
                    leaveOpen = false;
                }

                this._stream           = stream;
                this._leaveOpen        = leaveOpen;
                this._rootReference    = null;
                this._encryptReference = null;
                this._infoReference    = null;

                _fileVersion = _readHeader();

                int pos = _readStartXref();
                var reader = new PdfStreamReader(this, stream, pos);

                try {
                    var token = reader.ReadToken();
                    switch(token.Type) {
                    case PdfValueType.Xref:
                        _readXrefTable(reader);
                        break;

                    case PdfValueType.Integer:
                        _readXrefObject(pos, reader, token);
                        break;

                    default:
                        throw new PdfExceptionReader("Can't locate table.");
                    }
                }
                catch(Exception err) {
                    throw new PdfExceptionReader("Error reading xref table.", err);
                }
            }
            catch(Exception err) {
                if (!leaveOpen)
                    stream.Close();

                throw new PdfExceptionReader("Can't open PDF stream.", err);
            }
        }

        public              void                        Dispose()
        {
            if (_stream != null) {
                if (!_leaveOpen)
                    _stream.Close();

                _stream = null;
            }

            _xrefTable = null;
            _pages     = null;
        }
        public              void                        Close()
        {
            Dispose();
        }

        internal            PdfReferenceReader          pdfGetReference(PdfReferenceID r)
        {
            return pdfGetReference(r.Id, r.Revision);
        }
        internal            PdfReferenceReader          pdfGetReference(int id, int revision)
        {
            if (id < 0 || id > 65000)
                throw new PdfExceptionReader("Invalid reference #" + id + " to a unknown object.");

            if (id >= _xrefTable.Length)
                _allocateXrefTable(id + 1);

            if (_xrefTable[id] == null)
                return _xrefTable[id] = new PdfReferenceReader(this, id, revision);

            if (_xrefTable[id].Revision != revision)
                throw new PdfExceptionReader("Invalid revesion for object #" + id + ".");

            return _xrefTable[id];
        }
        internal            PdfValue                    pdfReadObj(PdfReferenceReader reference)
        {
            try {
                if (reference.CompressedObj == null) {
                    var reader = new PdfStreamReader(this, _stream, reference.Position);

                    int id       = reader.ReadInt();
                    int revision = reader.ReadInt();
                    reader.ReadToken(PdfValueType.ObjectBegin);

                    if (id != reference.Id || revision != reference.Revision)
                        throw new PdfException("Object id d'not match with xreftable.");

                    var obj = _readObj(reader, false);

                    if (obj is PdfObjectReader && ((PdfObjectReader)obj).NamedType == "ObjStm")
                        obj = new PdfObjStmReader(this, (PdfObjectReader)obj);

                    return obj;
                }
                else {
                    var objStm = _xrefTable[reference.CompressedObj.Id].Value as PdfObjStmReader;
                    if (objStm == null)
                        throw new PdfException("Obj not ObjStm.");

                    return _readObj(new PdfStreamReader(this,  objStm.GetStream(reference.Position, reference.Id), 0), true);
                }
            }
            catch(Exception err) {
                throw new PdfExceptionReader("Can't read object "+reference.Id.ToString()+"/"+reference.Revision.ToString()+".", err);
            }
        }

        private             void                        _constructPages()
        {
            List<PdfDictionary> pages = new List<PdfDictionary>();

            _constructPages(PagesRoot, pages);

            _pages = pages.ToArray();
        }
        private             void                        _constructPages(PdfDictionary pageNode, List<PdfDictionary> pages)
        {
            switch(pageNode.NamedType) {
            case "Page":
                pages.Add(pageNode);
                return;

            case "Pages":
                foreach(PdfValue kid in pageNode.ValueByName<PdfArray>("Kids").Children)
                    _constructPages(kid.Resolve<PdfDictionary>(), pages);
                break;
            }
        }
        private             PdfReferenceReader          _getObjectReference(PdfReferenceID reference)
        {
            return (reference != null) ? pdfGetReference(reference.Id, reference.Revision) : null;
        }
        private             int                         _readHeader()
        {
            byte[]      buf          = new byte[9];

            _stream.Position = 0;
            if (_stream.Read(buf, 0, 9) != 9)
                throw new PdfExceptionReader("IO error reading file header.");

            if ((char)buf[0]  == '%'
             && (char)buf[1]  == 'P'
             && (char)buf[2]  == 'D'
             && (char)buf[3]  == 'F'
             && (char)buf[4]  == '-'
             && ((char)buf[5] >= '0'  && (char)buf[5] <= '9')
             && (char)buf[6]  == '.'
             && ((char)buf[7] >= '0'  && (char)buf[7] <= '9')
             && ((char)buf[8] == '\n' || (char)buf[8] == '\r'))
            {
                return (((int)buf[5] - '0') * 10) + (((int)buf[7] - '0'));
            }

            throw new PdfExceptionReader("Not a PDF stream.");
        }
        private             int                         _readStartXref()
        {
            try {
                int         p;
                int         posEof       = -1;
                int         posStartxref = -1;
                byte[]      buf          = new byte[128];

                _stream.Position = _stream.Length - 128;
                if (_stream.Read(buf, 0, 128) != 128)
                    throw new PdfExceptionReader("IO error reading file trailer.");

                for (p = 128 - 5 ; p > 10 ; --p) {
                    if (buf[p + 0] == (byte)'%'
                     && buf[p + 1] == (byte)'%'
                     && buf[p + 2] == (byte)'E'
                     && buf[p + 3] == (byte)'O'
                     && buf[p + 4] == (byte)'F')
                    {
                        posEof       = p;
                    }

                    if (p < 128 - 10) {
                        if (buf[p + 0] == (byte)'s'
                         && buf[p + 1] == (byte)'t'
                         && buf[p + 2] == (byte)'a'
                         && buf[p + 3] == (byte)'r'
                         && buf[p + 4] == (byte)'t'
                         && buf[p + 5] == (byte)'x'
                         && buf[p + 6] == (byte)'r'
                         && buf[p + 7] == (byte)'e'
                         && buf[p + 8] == (byte)'f'
                         && (buf[p + 9] == (byte)'\n' || buf[p + 9] == (byte)'\r' || buf[p + 9] == (byte)' ' || buf[p + 9] == (byte)'\t'))
                        {
                            posStartxref = p;
                            break;
                        }
                    }
                }

                if (posEof == -1 || posStartxref == -1)
                    throw new PdfExceptionReader("Not a valid PDF file. Can't find startxref.");

                p = posStartxref + 10;
                while (buf[p] == (byte)'\n' || buf[p] == (byte)'\r' || buf[p] == (byte)' ' || buf[p] == (byte)'\t')
                    ++ p;

                int posXref = 0;

                while ('0' <= buf[p] && buf[p] <= '9') {
                    posXref = posXref * 10 + (buf[p] - '0');
                    ++p;
                }

                return posXref;
            }
            catch(Exception Err) {
                throw new PdfExceptionReader("Error reading startxref.", Err);
            }
        }
        private             void                        _readXrefTable(PdfStreamReader reader)
        {
            PdfValue    Token;

        // Read xref table

            while ((Token = reader.ReadToken()).Type == PdfValueType.Integer) {
                int Id    = ((PdfInteger)Token).Value;
                int Count = reader.ReadInt();

                _allocateXrefTable(Id + Count);

                while (Count-->0) {
                    int     EntryPosition = reader.ReadInt();
                    int     EntryRevision = reader.ReadInt();
                    bool    EntryUsed     = (reader.ReadByte() == 'n');

                    if (EntryPosition >= 10 && EntryUsed)
                        _xrefTable[Id] = new PdfReferenceReader(this, Id, EntryRevision, EntryPosition);

                    ++Id;
                }
            }

        // Read trailer
            if (Token.Type != PdfValueType.Trailer)
                throw new PdfExceptionReader("PDF stream corrupt: 'trailer' expected.");

            reader.ReadToken(PdfValueType.DictionaryBegin);

            while ((Token = reader.ReadToken()).Type != PdfValueType.DictionaryEnd) {
                if (Token.Type == PdfValueType.Name) {
                    switch(((PdfName)Token).Value) {
                    case "Size":
                            if (reader.ReadInt() != _xrefTable.Length)
                                throw new PdfExceptionReader("PDF stream corrupt: Xref table size d'not match trailer 'Size'.");
                        break;

                    case "Prev": {
                            var p = reader.ReadInt();
                            reader.ReadToken(PdfValueType.Xref);
                            _readXrefTable(new PdfStreamReader(this, _stream, p));
                        }
                        break;

                    case "Root":
                        _rootReference = pdfGetReference(reader.ReadReference());
                        break;

                    case "Encrypt":
                        _encryptReference = pdfGetReference(reader.ReadReference());
                        break;

                    case "Info":
                        _infoReference = pdfGetReference(reader.ReadReference());
                        break;

                    case "ID": {
                            List<PdfValue> ID = new List<PdfValue>();

                            reader.ReadToken(PdfValueType.ArrayBegin);

                            while ((Token = reader.ReadToken()).Type != PdfValueType.ArrayEnd)
                                ID.Add(Token);

                            _id = ID.ToArray();
                        }
                        break;
                    }
                }
            }
        }
        private             void                        _readXrefObject(int pos, PdfStreamReader reader, PdfValue firstToken)
        {
            int id       = ((PdfInteger)firstToken).Value;
            int revision = reader.ReadInt();
            reader.ReadToken(PdfValueType.ObjectBegin);
            _allocateXrefTable(id + 1);
            var rr  = new PdfReferenceReader(this, id, revision, pos);
            var obj = _readObj(reader, false) as PdfObjectReader;
            if (obj == null)
                throw new PdfExceptionReader("Invalid Xref obj.");
            rr.setValue(obj);
            _xrefTable[id] = rr;

            var dictionary = obj.Dictionary.Children;
            PdfValueList        w             = null;
            PdfValueList        index         = null;
            int                 size          = 0;
            PdfReferenceReader  prev          = null;

            for (int i = 0 ; i < dictionary.Count ; ++i) {
                var entry = dictionary[i];
                if (entry is PdfName) {
                    switch(((PdfName)entry).Value) {
                    case "W":       w                 = ((PdfArray)dictionary[++i]).Children;               break;
                    case "Index":   index             = ((PdfArray)dictionary[++i]).Children;               break;
                    case "Size":    size              = ((PdfInteger)dictionary[++i]).Value;                break;
                    case "Prev":    prev              = (PdfReferenceReader)dictionary[++i];                break;
                    case "Root":    _rootReference    = (PdfReferenceReader)dictionary[++i];                break;
                    case "Encrypt": _encryptReference = (PdfReferenceReader)dictionary[++i];                break;
                    case "Info":    _infoReference    = (PdfReferenceReader)dictionary[++i];                break;
                    case "ID":      _id               = ((PdfArray)dictionary[++i]).Children.ToArray();     break;
                    }
                }
            }

            if (obj.NamedType != "XRef" || w == null || index == null)
                throw new PdfExceptionReader("Invalid xref object.");

            int s1 = ((PdfInteger)w[0]).Value;
            int s2 = ((PdfInteger)w[1]).Value;
            int s3 = ((PdfInteger)w[2]).Value;
            id = ((PdfInteger)index[0]).Value;

            _allocateXrefTable(((PdfInteger)index[1]).Value);

            using (var r = new PdfXrefStreamReader(obj.GetUncompressStream()))
            {
                while (!r.EOF) {
                    int     t  = r.ReadValue(s1);
                    int     v2 = r.ReadValue(s2);
                    int     v3 = r.ReadValue(s3);

                    switch(t) {
                    case 0:                                                                             break;
                    case 1:     pdfGetReference(id, v3).setPosition(v2);                                break;
                    case 2:     pdfGetReference(id, 0).setCompressed(v3, pdfGetReference(v2, 0));       break;
                    default:    throw new PdfExceptionReader("cross-reference stream type #" + t + " unknown.");
                    }

                    ++id;
                }
            }
        }
        private             PdfValue                    _readObj(PdfStreamReader reader, bool compressed)
        {
            var value = reader.ReadValue();
            var token = (!compressed && reader.Position < reader.Stream.Length) ? reader.ReadToken() : null;
            if (token != null && value is PdfDictionary && token.Type == PdfValueType.StreamBegin) {
                var streamBegin  = reader.Position;
                var streamLength = ((PdfDictionary)value).ValueByName<PdfInteger>("Length").Value;
                reader.Skip(streamLength);
                reader.ReadToken(PdfValueType.StreamEnd);
                value = new PdfObjectReader((PdfDictionary)value, new PdfDataStreamReader(_stream, streamBegin, streamLength));
                token = (!compressed && reader.Position < reader.Stream.Length) ? reader.ReadToken() : null;
            }

            if (token != null && token.Type != PdfValueType.ObjectEnd)
                throw new PdfExceptionReader("Missing endobj");

            return value;
        }
        private             void                        _allocateXrefTable(int size)
        {
            if (_xrefTable == null || _xrefTable.Length < size) {
                PdfReferenceReader[]    NewTable = new PdfReferenceReader[size];

                if (_xrefTable != null)
                    Array.Copy(_xrefTable, NewTable, _xrefTable.Length);

                _xrefTable = NewTable;
            }
        }
        private static      Stream                      _copyStreamToMemory(Stream stream, bool leaveOpen)
        {
            MemoryStream    MemoryStream = new MemoryStream();
            byte[]          Buf          = new byte[4096];
            int             ReadSize;

            try {
                while ((ReadSize = stream.Read(Buf, 0, Buf.Length)) > 0)
                    MemoryStream.Write(Buf, 0, ReadSize);

                MemoryStream.Seek(0, SeekOrigin.Begin);
            }
            finally {
                if (!leaveOpen)
                    stream.Close();
            }

            return MemoryStream;
        }
    }
}
