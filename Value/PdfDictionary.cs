using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfDictionary: PdfValue
    {
        private readonly        PdfValueList            _children;

        public override         PdfValueType            Type        { get { return PdfValueType.Dictionary; } }
        public                  PdfValueList            Children    { get { return _children;           } }
        public                  string                  NamedType
        {
            get {
                return ValueByName<PdfName>("Type", mandatory:false)?.Value;
            }
        }

        internal                                        PdfDictionary(PdfStreamReader reader)
        {
            _children = new PdfValueList();
            _children.pdfReadChildren(reader, PdfValueType.DictionaryEnd);
        }
        public                  PdfValue                ValueByName(string name)
        {
            for (int i = 0 ; i < Children.Count ; ++i) {
                if (Children[i].Type == PdfValueType.Name) {
                    if (((PdfName)Children[i]).Value == name) {
                        PdfValue    Token = Children[i+1];

                        return Token;
                    }
                }
            }

            return null;
        }
        public                  T                       ValueByName<T>(string name, bool mandatory=true, bool resolve=true) where T: PdfValue
        {
            var value = ValueByName(name);

            if (value == null) {
                if (mandatory)
                    throw new PdfExceptionReader("Missing value '" + name + "'.");

                return null;
            }

            if (value is PdfReferenceReader && resolve)
                value = ((PdfReferenceReader)value).Value;

            if (!(value is T))
                throw new PdfExceptionReader("Invalid value '" + name + "' expect " + typeof(T).Name + " got " + value.GetType().Name + ".");

            return (T)value;
        }

        internal override       void                    pdfReadAll()
        {
            _children.pdfReadAll();
        }
        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteDictionaryBegin();
            _children.pdfWriteToDocument(document, writer);
            writer.WriteDictionaryEnd();
        }
    }
}
