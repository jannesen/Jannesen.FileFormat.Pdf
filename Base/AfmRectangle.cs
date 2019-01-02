/*@
    Copyright � Jannesen Holding B.V. 2006-2010.
    Unautorised reproduction, distribution or reverse eniginering is prohibited.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Jannesen.FileFormat.Pdf
{
    public struct AfmRectangle
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

#if DEBUG
        public                      void                    WriteTo(System.IO.BinaryWriter stream)
        {
            stream.Write(Convert.ToInt16(llX));
            stream.Write(Convert.ToInt16(llY));
            stream.Write(Convert.ToInt16(urX));
            stream.Write(Convert.ToInt16(urY));
        }
#endif
    }
}
