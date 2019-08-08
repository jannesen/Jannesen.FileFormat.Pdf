using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jannesen.FileFormat.Pdf.Internal
{
    public class PdfReferenceID
    {
        public          int                         Id;
        public          int                         Revision;

        public                                      PdfReferenceID(int Id, int Reference)
        {
            this.Id        = Id;
            this.Revision = Reference;
        }
    }
}
