/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStandardFont : PdfFont
    {
        internal                                    PdfStandardFont(System.IO.BinaryReader reader): base(reader)
        {
        }
#if DEBUG
        internal                                    PdfStandardFont(string dataName)
        {
            try {
                using (Stream Stream = new FileStream(dataName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (Stream == null)
                        throw new PdfException("Can't open resource '"+dataName+"'.");

                    ReadAfmFile(Stream);
                }
            }
            catch(Exception Err) {
                throw new Exception("Can't load standard font '"+FontName+"'.", Err);
            }
        }
#endif

        public  override        void                pdfWriteToDocument(PdfDocumentWriter Document, PdfStreamWriter Writer)
        {
            Writer.WriteDictionaryBegin();
            {
                Writer.WriteName("Type");
                Writer.WriteName("Font");

                Writer.WriteName("Subtype");
                Writer.WriteName("Type1");

                Writer.WriteName("BaseFont");
                Writer.WriteName(FontName);

                Writer.WriteName("Encoding");
                Writer.WriteName("WinAnsiEncoding");
            }
            Writer.WriteDictionaryEnd();
        }
    }
}
