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
    public enum PdfValueType
    {
        Xref,
        Trailer,
        StartXref,
        Object,
        ObjectBegin,
        ObjectEnd,
        StreamBegin,
        StreamEnd,
        Reference,
        Dictionary,
        DictionaryBegin,
        DictionaryEnd,
        Array,
        ArrayBegin,
        ArrayEnd,
        Null,
        Boolean,
        Integer,
        Number,
        Name,
        String,

    // Sepcial document writer.
        ByteStr,
        HexadecimalString,
        None
    }

    public abstract class PdfValue
    {
        public                  T                       Cast<T>() where T: PdfValue
        {
            var value = this;

            if (!(value is T))
                throw new PdfExceptionReader("Invalid type " + value.GetType().Name + " expect " + typeof(T).Name + ".");

            return (T)value;
        }
        public                  T                       Resolve<T>() where T: PdfValue
        {
            var value = this;

            if (value is PdfReferenceReader)
                value = ((PdfReferenceReader)value).Value;

            if (!(value is T))
                throw new PdfExceptionReader("Invalid type " + value.GetType().Name + " expect " + typeof(T).Name + ".");

            return (T)value;
        }

        public  abstract        PdfValueType            Type        { get ; }
        public  virtual         bool                    hasStream   { get => false;               }

        public  abstract        void                    pdfWriteToDocument(PdfDocumentWriter Document, PdfStreamWriter Writer);
        public  virtual         void                    pdfReadAll()
        {
        }
        public  virtual         void                    pdfAddToDocument(PdfDocumentWriter documentWriter)
        {
        }
    }

    public class PdfValueList : List<PdfValue>
    {
        public                  PdfValueType            pdfReadChildren(PdfStreamReader reader, PdfValueType endValueType)
        {
            PdfValue token;

            while ((token = reader.ReadToken()).Type != endValueType) {
                switch (token.Type) {
                case PdfValueType.Reference:            pdfReadReference(reader);           break;
                case PdfValueType.ArrayBegin:           pdfReadArray(reader);               break;
                case PdfValueType.DictionaryBegin:      pdfReadDictionary(reader);          break;

                case PdfValueType.ObjectBegin:  // This is a syntax error in the stream but eDocPrint somtimes fails to write endobj.....
                case PdfValueType.StreamBegin:
                    if (endValueType == PdfValueType.ObjectEnd)
                        return token.Type;
                    goto error;

                case PdfValueType.ObjectEnd:
                case PdfValueType.DictionaryEnd:
                case PdfValueType.ArrayEnd:
                case PdfValueType.StreamEnd:
error:              throw new PdfException("Unexpected token "+token.Type.ToString()+" in stream.");

                default:
                    base.Add(token);
                    break;
                }
            }

            return token.Type;
        }
        public                  void                    pdfReadReference(PdfStreamReader reader)
        {
            if (!(base.Count >= 2 &&
                  base[base.Count-2] is PdfInteger &&
                  base[base.Count-1] is PdfInteger))
                throw new PdfException("Invalid reference in stream.");

            int id       = ((PdfInteger)base[base.Count-2]).Value;
            int revision = ((PdfInteger)base[base.Count-1]).Value;

            base.RemoveRange(base.Count - 2, 2);

            base.Add(reader.Document.pdfGetReference(id, revision));
        }
        public                  void                    pdfReadArray(PdfStreamReader reader)
        {
            base.Add(new PdfArray(reader));
        }
        public                  void                    pdfReadDictionary(PdfStreamReader reader)
        {
            base.Add(new PdfDictionary(reader));
        }
        public                  void                    pdfReadAll()
        {
            for (int i = 0 ; i < Count ; ++i)
                base[i].pdfReadAll();
        }
        public                  void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            for (int i = 0 ; i < Count ; ++i)
                base[i].pdfWriteToDocument(document, writer);
        }
    }
}
