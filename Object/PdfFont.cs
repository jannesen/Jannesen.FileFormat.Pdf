/*@
    Copyright © Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.IO;
using System.Text;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public abstract class PdfFont : PdfObject
    {
        private                 string              _fontName;
        private                 string              _fullName;
        private                 string              _familyName;
        private                 string              _weight;
        private                 bool                _isCIDFont;
        private                 double              _italicAngle;
        private                 bool                _isFixedPitch;
        private                 string              _characterSet;
        private                 AfmRectangle        _fontBBox;
        private                 int                 _underlinePosition;
        private                 int                 _underlineThickness;
        private                 int                 _capHeight;
        private                 int                 _xheight;
        private                 int                 _ascender;
        private                 int                 _descender;
        private                 int                 _stdHW;
        private                 int                 _stdVW;
        private                 AfmCharMetric[]     _charMetric;

        public  override        string              NamedType               { get { return "Font";                                      } }
        public                  string              FontName                { get { return _fontName;                                   } }
        public                  string              FullName                { get { return _fullName;                                   } }
        public                  string              FamilyName              { get { return _familyName;                                 } }
        public                  bool                Bold                    { get { return string.Compare(_weight, "bold", true) == 0;  } }
        public                  bool                Italic                  { get { return _italicAngle < -1.0;                         } }
        public                  string              Weight                  { get { return _weight;                                     } }
        public                  bool                IsCIDFont               { get { return _isCIDFont;                                  } }
        public                  double              ItalicAngle             { get { return _italicAngle;                                } }
        public                  bool                IsFixedPitch            { get { return _isFixedPitch;                               } }
        public                  string              CharacterSet            { get { return _characterSet;                               } }
        public                  AfmRectangle        FontBBox                { get { return _fontBBox;                                   } }
        public                  int                 UnderlinePosition       { get { return _underlinePosition;                          } }
        public                  int                 UnderlineThickness      { get { return _underlineThickness;                         } }
        public                  int                 CapHeight               { get { return _capHeight;                                  } }
        public                  int                 XHeight                 { get { return _xheight;                                    } }
        public                  int                 Ascender                { get { return _ascender;                                   } }
        public                  int                 Descender               { get { return _descender;                                  } }
        public                  int                 StdHW                   { get { return _stdHW;                                      } }
        public                  int                 StdVW                   { get { return _stdVW;                                      } }
        public                  AfmCharMetric[]     CharMetric              { get { return _charMetric;                                 } }

        public                  PdfDistance         CharWidth(PdfDistance FontSize, char c)
        {
            if (Encode(c) == '?')
                c = '?';

            AfmCharMetric   metric =_charMetric[c];

            return new PdfDistance(FontSize.pnts * (double)(metric != null ? metric.Width : _fontBBox.urX) / 1000.0);
        }
        public                  PdfDistance         TextWidth(PdfDistance fontSize, string text)
        {
            int     width = 0;

            if (text != null) {
                for (int i = 0 ; i < text.Length ; ++i) {
                    char    c = text[i];

                    if (Encode(c) == '?')
                        c = '?';

                    AfmCharMetric   Metric =_charMetric[c];

                    width += (Metric != null) ? Metric.Width : _fontBBox.urX;
                }
            }

            return new PdfDistance(fontSize.pnts * (double)width / 1000.0);
        }
        public                  byte                Encode(char c)
        {
            if (c >= 0x20 && c <= 0x7F)
                return (byte)c;

            switch(c) {
            case (char)0x00A1:  return  0x00A1; //      exclamdown
            case (char)0x00A2:  return  0x00A2; //      cent
            case (char)0x00A3:  return  0x00A3; //      sterling
            case (char)0x00A5:  return  0x00A5; //      yen
            case (char)0x00A6:  return  0x00A6; //      brokenbar
            case (char)0x00A7:  return  0x00A7; //      section
            case (char)0x00A8:  return  0x00A8; //      dieresis
            case (char)0x00A9:  return  0x00A9; //      copyright
            case (char)0x00AA:  return  0x00AA; //      ordfeminine
            case (char)0x00AC:  return  0x00AC; //      logicalnot
            case (char)0x00AE:  return  0x00AE; //      registered
            case (char)0x00AF:  return  0x00AF; //      macron
            case (char)0x00B0:  return  0x00B0; //      degree
            case (char)0x00B1:  return  0x00B1; //      plusminus
            case (char)0x00B2:  return  0x00B2; //      twosuperior
            case (char)0x00B3:  return  0x00B3; //      threesuperior
            case (char)0x00B4:  return  0x00B4; //      acute
            case (char)0x00B5:  return  0x00B5; //      mu
            case (char)0x00B6:  return  0x00B6; //      paragraph
            case (char)0x00B7:  return  0x00B7; //      periodcentered
            case (char)0x00B8:  return  0x00B8; //      cedilla
            case (char)0x00B9:  return  0x00B9; //      onesuperior
            case (char)0x00BA:  return  0x00BA; //      ordmasculine
            case (char)0x00BC:  return  0x00BC; //      onequarter
            case (char)0x00BD:  return  0x00BD; //      onehalf
            case (char)0x00BE:  return  0x00BE; //      threequarters
            case (char)0x00BF:  return  0x00BF; //      questiondown
            case (char)0x00C0:  return  0x00C0; //      Agrave
            case (char)0x00C1:  return  0x00C1; //      Aacute
            case (char)0x00C2:  return  0x00C2; //      Acircumflex
            case (char)0x00C3:  return  0x00C3; //      Atilde
            case (char)0x00C4:  return  0x00C4; //      Adieresis
            case (char)0x00C5:  return  0x00C5; //      Aring
            case (char)0x00C6:  return  0x00C6; //      AE
            case (char)0x00C7:  return  0x00C7; //      Ccedilla
            case (char)0x00C8:  return  0x00C8; //      Egrave
            case (char)0x00C9:  return  0x00C9; //      Eacute
            case (char)0x00CA:  return  0x00CA; //      Ecircumflex
            case (char)0x00CB:  return  0x00CB; //      Edieresis
            case (char)0x00CC:  return  0x00CC; //      Igrave
            case (char)0x00CD:  return  0x00CD; //      Iacute
            case (char)0x00CE:  return  0x00CE; //      Icircumflex
            case (char)0x00CF:  return  0x00CF; //      Idieresis
            case (char)0x00D0:  return  0x00D0; //      Eth
            case (char)0x00D1:  return  0x00D1; //      Ntilde
            case (char)0x00D2:  return  0x00D2; //      Ograve
            case (char)0x00D3:  return  0x00D3; //      Oacute
            case (char)0x00D4:  return  0x00D4; //      Ocircumflex
            case (char)0x00D5:  return  0x00D5; //      Otilde
            case (char)0x00D6:  return  0x00D6; //      Odieresis
            case (char)0x00D7:  return  0x00D7; //      multiply
            case (char)0x00D8:  return  0x00D8; //      Oslash
            case (char)0x00D9:  return  0x00D9; //      Ugrave
            case (char)0x00DA:  return  0x00DA; //      Uacute
            case (char)0x00DB:  return  0x00DB; //      Ucircumflex
            case (char)0x00DC:  return  0x00DC; //      Udieresis
            case (char)0x00DD:  return  0x00DD; //      Yacute
            case (char)0x00DE:  return  0x00DE; //      Thorn
            case (char)0x00DF:  return  0x00DF; //      germandbls
            case (char)0x00E0:  return  0x00E0; //      agrave
            case (char)0x00E1:  return  0x00E1; //      aacute
            case (char)0x00E2:  return  0x00E2; //      acircumflex
            case (char)0x00E3:  return  0x00E3; //      atilde
            case (char)0x00E4:  return  0x00E4; //      adieresis
            case (char)0x00E5:  return  0x00E5; //      aring
            case (char)0x00E6:  return  0x00E6; //      ae
            case (char)0x00E7:  return  0x00E7; //      ccedilla
            case (char)0x00E8:  return  0x00E8; //      egrave
            case (char)0x00E9:  return  0x00E9; //      eacute
            case (char)0x00EA:  return  0x00EA; //      ecircumflex
            case (char)0x00EB:  return  0x00EB; //      edieresis
            case (char)0x00EC:  return  0x00EC; //      igrave
            case (char)0x00ED:  return  0x00ED; //      iacute
            case (char)0x00EE:  return  0x00EE; //      icircumflex
            case (char)0x00EF:  return  0x00EF; //      idieresis
            case (char)0x00F0:  return  0x00F0; //      eth
            case (char)0x00F1:  return  0x00F1; //      ntilde
            case (char)0x00F2:  return  0x00F2; //      ograve
            case (char)0x00F3:  return  0x00F3; //      oacute
            case (char)0x00F4:  return  0x00F4; //      ocircumflex
            case (char)0x00F5:  return  0x00F5; //      otilde
            case (char)0x00F6:  return  0x00F6; //      odieresis
            case (char)0x00F7:  return  0x00F7; //      divide
            case (char)0x00F8:  return  0x00F8; //      oslash
            case (char)0x00F9:  return  0x00F9; //      ugrave
            case (char)0x00FA:  return  0x00FA; //      uacute
            case (char)0x00FB:  return  0x00FB; //      ucircumflex
            case (char)0x00FC:  return  0x00FC; //      udieresis
            case (char)0x00FD:  return  0x00FD; //      yacute
            case (char)0x00FE:  return  0x00FE; //      thorn
            case (char)0x00FF:  return  0x00FF; //      ydieresis
            case (char)0x0152:  return  0x008C; //      OE
            case (char)0x0153:  return  0x009C; //      oe
            case (char)0x0160:  return  0x008A; //      Scaron
            case (char)0x0161:  return  0x009A; //      scaron
            case (char)0x0178:  return  0x009F; //      Ydieresis
            case (char)0x0192:  return  0x0083; //      florin
            case (char)0x02C6:  return  0x0088; //      circumflex
            case (char)0x02DC:  return  0x0098; //      tilde
            case (char)0x039C:  return  0x00B5; //      mu
            case (char)0x2013:  return  0x0096; //      endash
            case (char)0x2014:  return  0x0097; //      emdash
            case (char)0x2018:  return  0x0091; //      quoteleft
            case (char)0x2019:  return  0x0092; //      quoteright
            case (char)0x201A:  return  0x0082; //      quotesinglbase
            case (char)0x201C:  return  0x0093; //      quotedblleft
            case (char)0x201D:  return  0x0094; //      quotedblright
            case (char)0x201E:  return  0x0084; //      quotedblbase
            case (char)0x2020:  return  0x0086; //      dagger
            case (char)0x2021:  return  0x0087; //      daggerdbl
            case (char)0x2026:  return  0x0085; //      ellipsis
            case (char)0x2030:  return  0x0089; //      perthousand
            case (char)0x2039:  return  0x008B; //      guilsinglleft
            case (char)0x203A:  return  0x009B; //      guilsinglright
            case (char)0x20AC:  return  0x0080; //      Euro
            case (char)0x2122:  return  0x0099; //      trademark
            case (char)0xF6C9:  return  0x00B4; //      acute
            case (char)0xF6CB:  return  0x00A8; //      dieresis
            case (char)0xF6D0:  return  0x00AF; //      macron

            default:            return (byte)'?';
            }
        }

        protected                                   PdfFont(System.IO.BinaryReader reader)
        {
            _fontName           = reader.ReadString();
            _fullName           = reader.ReadString();
            _familyName         = reader.ReadString();
            _weight             = reader.ReadString();
            _isCIDFont          = reader.ReadBoolean();
            _italicAngle        = reader.ReadDouble();
            _isFixedPitch       = reader.ReadBoolean();
            _characterSet       = reader.ReadString();
            _fontBBox           = new AfmRectangle(reader);
            _underlinePosition  = reader.ReadInt16();
            _underlineThickness = reader.ReadInt16();
            _capHeight          = reader.ReadInt16();
            _xheight            = reader.ReadInt16();
            _ascender           = reader.ReadInt16();
            _descender          = reader.ReadInt16();
            _stdHW              = reader.ReadInt16();
            _stdVW              = reader.ReadInt16();

            _charMetric         = new AfmCharMetric[65536];

            UInt16      pos;

            while ((pos = reader.ReadUInt16()) != 0)
                _charMetric[pos] = new AfmCharMetric(reader);
        }

#if DEBUG
        protected                                   PdfFont()
        {
            _fontName           = "";
            _fullName           = "";
            _familyName         = "";
            _weight             = "";
            _isCIDFont          = false;
            _italicAngle        = 0.0;
            _isFixedPitch       = false;
            _characterSet       = "";
            _fontBBox           = new AfmRectangle(0,0,0,0);
            _underlinePosition  = 0;
            _underlineThickness = 0;
            _capHeight          = 0;
            _xheight            = 0;
            _ascender           = 0;
            _descender          = 0;
            _stdHW              = 0;
            _stdVW              = 0;
            _charMetric         = new AfmCharMetric[65536];
        }
        protected               void                ReadAfmFile(Stream afmStream)
        {
            AfmReader       Reader = new AfmReader(afmStream);

            try {
                string          Key;
                bool            fCharMetrics = false;

                while ((Key = Reader.ReadKey(false)) != null) {
                    switch(Key) {
                    case "Comment":                                                                 break;
                    case "Version":                                                                 break;
                    case "FontName":            _fontName           = Reader.ReadString(false);     break;
                    case "FullName":            _fullName           = Reader.ReadString(false);     break;
                    case "FamilyName":          _familyName         = Reader.ReadString(false);     break;
                    case "Weight":              _weight             = Reader.ReadString(false);     break;
                    case "IsCIDFont":           _isCIDFont          = Reader.ReadBoolean();         break;
                    case "CharacterSet":        _characterSet       = Reader.ReadString(false);     break;
                    case "FontBBox":            _fontBBox           = Reader.ReadRectangle();       break;
                    case "ItalicAngle":         _italicAngle        = Reader.ReadNumber();          break;
                    case "IsFixedPitch":        _isFixedPitch       = Reader.ReadBoolean();         break;
                    case "CapHeight":           _capHeight          = Reader.ReadInteger();         break;
                    case "XHeight":             _xheight            = Reader.ReadInteger();         break;
                    case "UnderlinePosition":   _underlinePosition  = Reader.ReadInteger();         break;
                    case "UnderlineThickness":  _underlineThickness = Reader.ReadInteger();         break;
                    case "Ascender":            _ascender           = Reader.ReadInteger();         break;
                    case "Descender":           _descender          = Reader.ReadInteger();         break;
                    case "StdHW":               _stdHW              = Reader.ReadInteger();         break;
                    case "StdVW":               _stdVW              = Reader.ReadInteger();         break;

                    case "VVector":
                    case "StartDirection":
                        if (Reader.ReadInteger() != 0)
                            throw new PdfException("Only right to left font are supported.");
                        break;

                    case "StartCharMetrics":
                        fCharMetrics = true;
                        break;

                    case "EndCharMetrics":
                        fCharMetrics = false;
                        break;

                    case "C":
                    case "CH":
                        if (!fCharMetrics)
                            throw new PdfException("CharMetrics not active.");

                        _ReadAfmFile_ReadCharMetric(Reader, (Key == "CH") ? (int)Reader.ReadHex() : Reader.ReadInteger());
                        break;

                    case "Notice":
                    case "EncodingScheme":
                    case "StartFontMetrics":
                    case "EndFontMetrics":
                    case "StartKernData":
                    case "EndKernData":
                    case "StartKernPairs":
                    case "EndKernPairs":
                    case "KPX":
                        break;

                    default:
//                      System.Diagnostics.Debug.WriteLine("Warning: Unknown AFM key '"+Key+"'.");
                        break;
                    }
                }
            }
            catch(Exception Err) {
                throw new PdfException("Error parsing Afm file at line " + Reader.LineNo.ToString() + ".", Err);
            }
        }
        public                  void                WriteTo(System.IO.BinaryWriter writer)
        {
            writer.Write(_fontName);
            writer.Write(_fullName);
            writer.Write(_familyName);
            writer.Write(_weight);
            writer.Write(_isCIDFont);
            writer.Write(_italicAngle);
            writer.Write(_isFixedPitch);
            writer.Write(_characterSet);
            _fontBBox.WriteTo(writer);
            writer.Write(Convert.ToInt16(_underlinePosition));
            writer.Write(Convert.ToInt16(_underlineThickness));
            writer.Write(Convert.ToInt16(_capHeight));
            writer.Write(Convert.ToInt16(_xheight));
            writer.Write(Convert.ToInt16(_ascender));
            writer.Write(Convert.ToInt16(_descender));
            writer.Write(Convert.ToInt16(_stdHW));
            writer.Write(Convert.ToInt16(_stdVW));

            for (int i = 1 ; i < _charMetric.Length ; ++i) {
                if (_charMetric[i] != null) {
                    writer.Write(Convert.ToUInt16(i));
                    _charMetric[i].WriteTo(writer);
                }
            }

            writer.Write((UInt16)0);
        }
        private                 void                _ReadAfmFile_ReadCharMetric(AfmReader reader, int charCode)
        {
            string          subKey;
            int             unicode = -1;
            int             height  = 0;
            int             width   = 0;
            AfmRectangle    box     = new AfmRectangle(0, 0, 0, 0);

            while ((subKey = reader.ReadKey(true)) != null) {

                switch(subKey) {
                case "WX":
                case "W0X":
                    width = reader.ReadInteger();
                    break;

                case "WY":
                case "W0Y":
                    height = reader.ReadInteger();
                    break;

                case "B":
                    box    = reader.ReadRectangle();
                    break;

                case "L":
                    break;

                case "VV":
                case "W1X":
                case "W1Y":
                    throw new PdfException("Only right to left font are supported.");

                case "N":
                    {
                        string  GlyphName = reader.ReadString(false);

                        unicode = AfmReader.GlyphNameToUnicode(GlyphName);

                        if (unicode < 0 && charCode < 0)
                            throw new PdfException("Unknown GlyphName '" + GlyphName + "'.");
                    }
                    break;

                default:
//                  System.Diagnostics.Debug.WriteLine("Warning: Unknown AFM sub-key '"+subKey+"'.");
                    break;
                }
            }

            if (unicode < 0) {
                if (charCode < 0)
                    throw new PdfException("Unknown charcode and unicode.");

                if (_charMetric[charCode] == null)
                    unicode = charCode;
            }

            if (unicode > 0) {
                _charMetric[unicode] = new AfmCharMetric(height, width, box);
            }
        }
#endif
    }
}
