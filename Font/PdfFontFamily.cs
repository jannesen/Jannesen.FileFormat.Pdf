/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public abstract class PdfFontFamily
    {
        public  abstract    string              FamilyName              { get ; }
        public  abstract    PdfFont             Normal                  { get ; }
        public  abstract    PdfFont             Bold                    { get ; }
        public  abstract    PdfFont             Italic                  { get ; }
        public  abstract    PdfFont             BoldItalic              { get ; }

        public              PdfFont             SubFont(bool bold, bool italic)
        {
            if (bold) {
                if (italic) {
                    if (this.BoldItalic == null)
                        throw new PdfException("Font-family '"+FamilyName+"' has no bold-italic font.");

                    return this.BoldItalic;
                }
                else {
                    if (this.Bold == null)
                        throw new PdfException("Font-family '"+FamilyName+"' has no bold font.");

                    return this.Bold;
                }
            }
            else {
                if (italic) {
                    if (this.Italic == null)
                        throw new PdfException("Font-family '"+FamilyName+"' has no italic font.");

                    return this.Italic;
                }
                else {
                    if (this.Normal == null)
                        throw new PdfException("Font-family '"+FamilyName+"' has no normal font.");

                    return this.Normal;
                }
            }
        }
    }
}
