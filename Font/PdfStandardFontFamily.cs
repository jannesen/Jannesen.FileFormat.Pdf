using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfStandardFontFamily : PdfFontFamily
    {
        public  static      PdfFontFamily       Courier                 { get { return PdfStandard.Standard.FontFamilyCourier;      } }
        public  static      PdfFontFamily       Helvetica               { get { return PdfStandard.Standard.FontFamilyHelvetica;    } }
        public  static      PdfFontFamily       TimesRoman              { get { return PdfStandard.Standard.FontFamilyTimesRoman;   } }
        public  static      PdfFontFamily       Symbol                  { get { return PdfStandard.Standard.FontFamilySymbol;       } }
        public  static      PdfFontFamily       ZapfDingbats            { get { return PdfStandard.Standard.FontFamilyZapfDingbats; } }

        private readonly    string              _familyName;
        private readonly    PdfStandardFont     _normal;
        private readonly    PdfStandardFont     _bold;
        private readonly    PdfStandardFont     _italic;
        private readonly    PdfStandardFont     _boldItalic;

        public  override    string              FamilyName
        {
            get {
                return _familyName;
            }
        }
        public  override    PdfFont             Normal
        {
            get {
                return _normal;
            }
        }
        public  override    PdfFont             Bold
        {
            get {
                return _bold;
            }
        }
        public  override    PdfFont             Italic
        {
            get {
                return _italic;
            }
        }
        public  override    PdfFont             BoldItalic
        {
            get {
                return _boldItalic;
            }
        }

        public              PdfFont[]           Fonts       { get => [ _normal, _bold, _italic, _boldItalic ]; }

        internal                                PdfStandardFontFamily(System.IO.BinaryReader reader)
        {
            _familyName = reader.ReadString();
            _normal     = (reader.ReadBoolean()) ? new PdfStandardFont(reader) : null;
            _bold       = (reader.ReadBoolean()) ? new PdfStandardFont(reader) : null;
            _italic     = (reader.ReadBoolean()) ? new PdfStandardFont(reader) : null;
            _boldItalic = (reader.ReadBoolean()) ? new PdfStandardFont(reader) : null;
        }

#if DEBUG
        internal                                PdfStandardFontFamily(string familyName, string fontNameNormal, string fontNameBold, string fontNameItalic, string fontNameBoldItalic)
        {
            try {
                _familyName = familyName;

                if (fontNameNormal != null)
                    _normal     = new PdfStandardFont(fontNameNormal);

                if (fontNameBold != null)
                    _bold       = new PdfStandardFont(fontNameBold);

                if (fontNameItalic != null)
                    _italic     = new PdfStandardFont(fontNameItalic);

                if (fontNameBoldItalic != null)
                    _boldItalic = new PdfStandardFont(fontNameBoldItalic);
            }
            catch(Exception Err) {
                throw new PdfException("Can't load font family '" + familyName + "'.", Err);
            }
        }
        public                      void        WriteTo(System.IO.BinaryWriter stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            stream.Write(_familyName);

            stream.Write(_normal != null);
            if (_normal != null)
                _normal.WriteTo(stream);

            stream.Write(_bold != null);
            if (_bold != null)
                _bold.WriteTo(stream);

            stream.Write(_italic != null);
            if (_italic != null)
                _italic.WriteTo(stream);

            stream.Write(_boldItalic != null);
            if (_boldItalic != null)
                _boldItalic.WriteTo(stream);
        }
#endif
    }
}
