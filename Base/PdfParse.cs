/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfParse
    {
        public  static  PdfColor            Color(string str)
        {
            if (string.Compare(str, 0, "rgb ", 0, 4, true) == 0)
                return ColorRGB(str.Substring(4));

            if (string.Compare(str, 0, "cmyk ", 0, 4, true) == 0)
                return ColorCMYK(str.Substring(5));

            throw new PdfException("Unknown colorspace in color '"+str+"'.");
        }
        public  static  PdfColorCMYK        ColorCMYK(string str)
        {
            str = str.Trim();

            switch(str)
            {
            case "black":   return Jannesen.FileFormat.Pdf.PdfColorCMYK.ncBlack;

            default:
                {
                    string[]    Parts = str.Split(',');
                    double[]    Values = new double[4];

                    if (Parts.Length != 4)
                        throw new PdfException("Invalid cmyk-color '" + str + "'.");

                    for (int i = 0 ; i < 4 ; ++i) {
                        try {
                            Values[i] = double.Parse(Parts[i]);

                            if (Values[i]< 0 || Values[i]>100)
                                throw new ArgumentOutOfRangeException();
                        }
                        catch(Exception) {
                            throw new PdfException("Invalid cmyk-color '" + str + "'.");
                        }
                    }

                    return new PdfColorCMYK(Values[0], Values[1], Values[2], Values[3]);
                }
            }
        }
        public  static  PdfColorRGB         ColorRGB(string str)
        {
            str = str.Trim();

            switch(str)
            {
            case "black":   return PdfColorRGB.ncBlack;
            case "red":     return PdfColorRGB.ncRed;
            case "green":   return PdfColorRGB.ncGreen;
            case "blue":    return PdfColorRGB.ncBlue;
            case "white":   return PdfColorRGB.ncWhite;

            default:
                {
                    string[]    Parts = str.Split(',');
                    double[]    Values = new double[4];

                    if (Parts.Length != 3)
                        throw new PdfException("Invalid cmyk-color '" + str + "'.");

                    for (int i = 0 ; i < 3 ; ++i) {
                        try {
                            Values[i] = double.Parse(Parts[i]);

                            if (Values[i]< 0 || Values[i]>100)
                                throw new ArgumentOutOfRangeException();
                        }
                        catch(Exception) {
                            throw new PdfException("Invalid cmyk-color '" + str + "'.");
                        }
                    }

                    return new PdfColorRGB(Values[0], Values[1], Values[2]);
                }
            }
        }
        public  static  PdfDistance         Distance(string str)
        {
            double  factor;

            str = str.Trim();

            if (str.EndsWith("mm")) {
                factor = 2.8125;
                str    = str.Substring(0, str.Length - 2);
            }
            else
            if (str.EndsWith("cm")) {
                factor = 28.125;
                str    = str.Substring(0, str.Length - 2);
            }
            else
            if (str.EndsWith("pt")) {
                factor = 1;
                str    = str.Substring(0, str.Length - 2);
            }
            else
            if (str.EndsWith("inch")) {
                factor = 72;
                str    = str.Substring(0, str.Length - 4);
            }
            else
                throw new PdfException("Invalid distance '" + str + "'.");

            try {
                return new PdfDistance(factor * double.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            catch(Exception) {
                throw new PdfException("Invalid distance '" + str + "'.");
            }
        }
        public  static  PdfLineCapStyle     LineCapStyle(string str)
        {
            try {
                return (PdfLineCapStyle)System.Enum.Parse(typeof(PdfLineCapStyle), str, true);
            }
            catch(Exception) {
                throw new PdfException("invalid LineCapStyle '" + str + "'");
            }
        }
        public  static  PdfLineJoinStyle    LineJoinStyle(string str)
        {
            try {
                return (PdfLineJoinStyle)System.Enum.Parse(typeof(PdfLineJoinStyle), str, true);
            }
            catch(Exception) {
                throw new PdfException("invalid LineCapStyle '" + str + "'");
            }
        }
        public  static  PdfSize             PageSize(string str)
        {
            str = str.Trim();

            switch(str)
            {
            case "A0":  return PdfPageSize.A0;
            case "A1":  return PdfPageSize.A1;
            case "A2":  return PdfPageSize.A2;
            case "A3":  return PdfPageSize.A3;
            case "A4":  return PdfPageSize.A4;
            case "A5":  return PdfPageSize.A5;
            case "A6":  return PdfPageSize.A6;

            default:
                {
                    string[] StrParts = str.Split(',');

                    if (StrParts.Length != 2)
                        throw new PdfException("invalid page-size '" + str + "'.");

                    return new PdfSize(Distance(StrParts[0]),
                                       Distance(StrParts[1]));
                }
            }
        }
        public  static  PdfPoint            Point(string str)
        {
            string[] StrParts = str.Split(',');

            if (StrParts.Length != 2)
                throw new PdfException("invalid point '" + str + "'.");

            return new PdfPoint(Distance(StrParts[0]),
                                Distance(StrParts[1]));
        }
        public  static  PdfRectangle        Rectangle(string str)
        {
            string[] StrParts = str.Split(',');

            if (StrParts.Length != 4)
                throw new PdfException("invalid rectangle '" + str + "'.");

            return new PdfRectangle(Distance(StrParts[0]),
                                    Distance(StrParts[1]),
                                    Distance(StrParts[2]),
                                    Distance(StrParts[3]));
        }
        public  static  PdfSize             Size(string str)
        {
            string[] StrParts = str.Split(',');

            if (StrParts.Length != 2)
                throw new PdfException("invalid size '" + str + "'.");

            return new PdfSize(Distance(StrParts[0]),
                               Distance(StrParts[1]));
        }
        public  static  PdfTextAlign        TextAlign(string str)
        {
            try {
                return (PdfTextAlign)System.Enum.Parse(typeof(PdfTextAlign), str, true);
            }
            catch(Exception) {
                throw new PdfException("invalid TextAlign '" + str + "'");
            }
        }
        public  static  double              Double(string str)
        {
            try {
                return double.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            catch(Exception) {
                throw new PdfException("Invalid number '" + str + "'.");
            }
        }
    }
}
