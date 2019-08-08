using System;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public abstract class PdfObject : PdfValue
    {
        public  const           string                      ntPage    = "Page";
        public  const           string                      ntFont    = "Font";
        public  const           string                      ntXObject = "XObject";

        public override         PdfValueType                Type                    { get => PdfValueType.Object; }
        public virtual          string                      NamedType               { get => null;                }

        public                                              PdfObject()
        {
        }
    }
}
