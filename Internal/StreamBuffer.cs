using System;
using System.IO;

namespace Jannesen.FileFormat.Pdf.Internal
{
    sealed class StreamBuffer: MemoryStream
    {
        public  override    void    Close()
        {
        }
    }
}
