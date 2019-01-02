/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public class AfmCharMetric
    {
        private                 int             _height;
        private                 int             _width;
        private                 AfmRectangle    _box;

        public                  int             Height          { get { return _height; } }
        public                  int             Width           { get { return _width;  } }
        public                  AfmRectangle    Box             { get { return _box;    } }

        public                                  AfmCharMetric(int Height, int Width, AfmRectangle Box)
        {
            _height = Height;
            _width  = Width;
            _box    = Box;
        }
        internal                                AfmCharMetric(System.IO.BinaryReader reader)
        {
            _height = reader.ReadInt16();
            _width  = reader.ReadInt16();
            _box    = new AfmRectangle(reader);
        }
#if DEBUG
        public                  void            WriteTo(System.IO.BinaryWriter stream)
        {
            stream.Write(Convert.ToInt16(_height));
            stream.Write(Convert.ToInt16(_width));
            _box.WriteTo(stream);
        }
#endif
    }
}
