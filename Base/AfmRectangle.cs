using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public readonly struct AfmRectangle: IEquatable<AfmRectangle>
    {
        public  readonly    int     llX;
        public  readonly    int     llY;
        public  readonly    int     urX;
        public  readonly    int     urY;

        public                      AfmRectangle(int llX, int llY, int urX, int urY)
        {
            this.llX = llX;
            this.llY = llY;
            this.urX = urX;
            this.urY = urY;
        }
        internal                    AfmRectangle(System.IO.BinaryReader reader)
        {
            this.llX = reader.ReadInt16();
            this.llY = reader.ReadInt16();
            this.urX = reader.ReadInt16();
            this.urY = reader.ReadInt16();
        }

        public  static      bool    operator == (AfmRectangle p1, AfmRectangle p2)
        {
            return p1.llX == p2.llX &&
                   p1.llY == p2.llY &&
                   p1.urX == p2.urX &&
                   p1.urY == p2.urY;
        }
        public  static      bool    operator != (AfmRectangle p1, AfmRectangle p2)
        {
            return !(p1 == p2);
        }
        public  override    bool    Equals(object obj)
        {
            if (obj is AfmRectangle)
                return this == (AfmRectangle)obj;

            return false;
        }
        public              bool    Equals(AfmRectangle o)
        {
            return this == o;
        }
        public  override    int     GetHashCode()
        {
            return llX.GetHashCode() ^ llY.GetHashCode() ^ urX.GetHashCode() ^ urY.GetHashCode();
        }

#if DEBUG
        public              void    WriteTo(System.IO.BinaryWriter stream)
        {
            if (stream is null) throw new ArgumentNullException(nameof(stream));

            stream.Write(Convert.ToInt16(llX));
            stream.Write(Convert.ToInt16(llY));
            stream.Write(Convert.ToInt16(urX));
            stream.Write(Convert.ToInt16(urY));
        }
#endif
    }
}
