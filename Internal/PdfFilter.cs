/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.IO;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfFilter
    {
        public  static      Stream      GetCompressor(string filter, Stream output)
        {
            switch (filter) {
            case null:          return output;
            case "FlateDecode": return new Jannesen.FileFormat.Pdf.ZLib.ZDeflaterOutputStream(output);
            default:            throw new PdfException("Can't compress '"+filter+"'.");
            }
        }
        public  static      Stream      GetDecompressor(string filter, Stream input)
        {
            switch (filter) {
            case "FlateDecode": return new Jannesen.FileFormat.Pdf.ZLib.ZInflaterInputStream(input);
            default:            throw new PdfException("Can't decompress '"+filter+"'.");
            }
        }
    }
}
