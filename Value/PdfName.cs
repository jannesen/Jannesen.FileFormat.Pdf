using System;
using System.Collections.Generic;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfName: PdfValue
    {
        private readonly        string                  _value;

        public override         PdfValueType            Type        { get { return PdfValueType.Name;       } }
        public                  string                  Value       { get { return _value;                  } }

        public                                          PdfName(string value)
        {
            this._value = value;
        }
        public                                          PdfName(char[] value, int length)
        {
            this._value = new string(value, 0, length);
        }

        internal override       void                    pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteName(_value);
        }

        public  override        int                     GetHashCode()
        {
            return Value.GetHashCode();
        }
        public  override        bool                    Equals(object obj)
        {
            if (obj is PdfName) {
                return ((PdfName)obj).Value == Value;
            }

            return false;
        }
        public  override        string                  ToString()
        {
            return "/" + Value;
        }
    }
}
