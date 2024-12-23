﻿using System;
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
            PdfData     rtn = new PdfData() { _standardFonts = new Dictionary<string, PdfFont>() };

            using (BinaryReader reader = new BinaryReader(typeof(PdfData).Assembly.GetManifestResourceStream("Jannesen.FileFormat.Pdf.Data.bin"))) {
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
            PdfFontFamily   FontFamily;

            if (string.Equals(familyName, "courier", StringComparison.OrdinalIgnoreCase))       FontFamily = FontFamilyCourier;
            else
            if (string.Equals(familyName, "helvetica", StringComparison.OrdinalIgnoreCase))     FontFamily = FontFamilyHelvetica;
            else
            if (string.Equals(familyName, "times-roman", StringComparison.OrdinalIgnoreCase))   FontFamily = FontFamilyTimesRoman;
            else
            if (string.Equals(familyName, "symbol", StringComparison.OrdinalIgnoreCase))        FontFamily = FontFamilySymbol;
            else
            if (string.Equals(familyName, "zapf-dngbats", StringComparison.OrdinalIgnoreCase))  FontFamily = FontFamilyZapfDingbats;
            else
                throw new PdfException("Unknown font-family '"+familyName+"'.");

            return FontFamily.SubFont(bold, italic);
        }
        public                      PdfFont                         FindStandardFontByName(string fontName)
        {
            _standardFonts.TryGetValue(fontName, out PdfFont value);
            return value;
        }
        private                     void                            _indexFonts()
        {
            _standardFonts = new Dictionary<string, PdfFont>();
            _indexFontFamily(FontFamilyCourier);
            _indexFontFamily(FontFamilyHelvetica);
            _indexFontFamily(FontFamilyTimesRoman);
            _indexFontFamily(FontFamilySymbol);
            _indexFontFamily(FontFamilyZapfDingbats);
        }
        private                     void                            _indexFontFamily(PdfStandardFontFamily fontFamily)
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
            var rtn = new PdfData() {
                          FontFamilyCourier       = new PdfStandardFontFamily("Courier",      "Courier.afm",
                                                                                              "Courier-Bold.afm",
                                                                                              "Courier-Oblique.afm",
                                                                                              "Courier-BoldOblique.afm"),
                          FontFamilyHelvetica     = new PdfStandardFontFamily("Helvetica",    "Helvetica.afm",
                                                                                              "Helvetica-Bold.afm",
                                                                                              "Helvetica-Oblique.afm",
                                                                                              "Helvetica-BoldOblique.afm"),
                          FontFamilyTimesRoman    = new PdfStandardFontFamily("Courier",      "Times-Roman.afm",
                                                                                              "Times-Bold.afm",
                                                                                              "Times-Italic.afm",
                                                                                              "Times-BoldItalic.afm"),
                          FontFamilySymbol        = new PdfStandardFontFamily("Symbol",       "Symbol.afm",
                                                                                              null,
                                                                                              null,
                                                                                              null),
                          FontFamilyZapfDingbats  = new PdfStandardFontFamily("ZapfDingbats", "ZapfDingbats.afm",
                                                                                              null,
                                                                                              null,
                                                                                              null)
                    };

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
