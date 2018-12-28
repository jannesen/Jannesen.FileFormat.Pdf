/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.IO;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfData
    {
        public                      PdfStandardFontFamily           FontFamilyCourier       { get ; private set; }
        public                      PdfStandardFontFamily           FontFamilyHelvetica     { get ; private set; }
        public                      PdfStandardFontFamily           FontFamilyTimesRoman    { get ; private set; }
        public                      PdfStandardFontFamily           FontFamilySymbol        { get ; private set; }
        public                      PdfStandardFontFamily           FontFamilyZapfDingbats  { get ; private set; }

        private                     Dictionary<string, PdfFont>     _standardFonts;

        public      static          PdfData                         ReadEmbedded()
        {
            PdfData     rtn = new PdfData();

            rtn._standardFonts = new Dictionary<string, PdfFont>();

            using (BinaryReader reader = new BinaryReader(typeof(PdfData).Assembly.GetManifestResourceStream("Jannesen.FileFormat.Pdf.Data.bin")))
            {
                rtn.FontFamilyCourier      = new PdfStandardFontFamily(reader);
                rtn.FontFamilyHelvetica    = new PdfStandardFontFamily(reader);
                rtn.FontFamilyTimesRoman   = new PdfStandardFontFamily(reader);
                rtn.FontFamilySymbol       = new PdfStandardFontFamily(reader);
                rtn.FontFamilyZapfDingbats = new PdfStandardFontFamily(reader);
            }

            rtn._indexFonts();

            return rtn;
        }
        public                      PdfFont                         GetFont(string familyName, bool bold, bool italic)
        {
            PdfFontFamily   FontFamily = null;

            if (string.Compare(familyName, "courier", true) == 0)       FontFamily = FontFamilyCourier;
            else
            if (string.Compare(familyName, "helvetica", true) == 0)     FontFamily = FontFamilyHelvetica;
            else
            if (string.Compare(familyName, "times-roman", true) == 0)   FontFamily = FontFamilyTimesRoman;
            else
            if (string.Compare(familyName, "symbol", true) == 0)        FontFamily = FontFamilySymbol;
            else
            if (string.Compare(familyName, "zapf-dngbats", true) == 0)  FontFamily = FontFamilyZapfDingbats;
            else
                throw new PdfException("Unknown font-family '"+familyName+"'.");

            return FontFamily.SubFont(bold, italic);
        }
        public                      PdfFont                         FindStandardFontByName(string fontName)
        {
            _standardFonts.TryGetValue(fontName, out PdfFont value);
            return value;
        }
        public                      void                            _indexFonts()
        {
            _standardFonts = new Dictionary<string, PdfFont>();
            _indexFontFamily(FontFamilyCourier);
            _indexFontFamily(FontFamilyHelvetica);
            _indexFontFamily(FontFamilyTimesRoman);
            _indexFontFamily(FontFamilySymbol);
            _indexFontFamily(FontFamilyZapfDingbats);
        }
        public                      void                            _indexFontFamily(PdfStandardFontFamily fontFamily)
        {
            foreach(var f in fontFamily.Fonts) {
                if (f != null) {
                    _standardFonts.Add(f.FontName, f);
                }
            }
        }
#if DEBUG
        public      static          PdfData                         ReadFromFiles()
        {
            PdfData     rtn = new PdfData();
            rtn.FontFamilyCourier       = new PdfStandardFontFamily("Courier",      "Courier.afm",
                                                                                    "Courier-Bold.afm",
                                                                                    "Courier-Oblique.afm",
                                                                                    "Courier-BoldOblique.afm");
            rtn.FontFamilyHelvetica     = new PdfStandardFontFamily("Helvetica",    "Helvetica.afm",
                                                                                    "Helvetica-Bold.afm",
                                                                                    "Helvetica-Oblique.afm",
                                                                                    "Helvetica-BoldOblique.afm");
            rtn.FontFamilyTimesRoman    = new PdfStandardFontFamily("Courier",      "Times-Roman.afm",
                                                                                    "Times-Bold.afm",
                                                                                    "Times-Italic.afm",
                                                                                    "Times-BoldItalic.afm");
            rtn.FontFamilySymbol        = new PdfStandardFontFamily("Symbol",       "Symbol.afm",
                                                                                    null,
                                                                                    null,
                                                                                    null);
            rtn.FontFamilyZapfDingbats  = new PdfStandardFontFamily("ZapfDingbats", "ZapfDingbats.afm",
                                                                                    null,
                                                                                    null,
                                                                                    null);

            rtn._indexFonts();

            return rtn;
        }
        public                      void                            WriteTo(BinaryWriter writer)
        {
            FontFamilyCourier.WriteTo(writer);
            FontFamilyHelvetica.WriteTo(writer);
            FontFamilyTimesRoman.WriteTo(writer);
            FontFamilySymbol.WriteTo(writer);
            FontFamilyZapfDingbats.WriteTo(writer);
        }
#endif
    }
}
