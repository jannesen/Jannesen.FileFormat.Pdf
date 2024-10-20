using System;
using System.IO;
using Jannesen.FileFormat.Pdf;

namespace Serialize
{
    sealed class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            try {
//              PdfData data = PdfStandard.Standard;

                using (BinaryWriter writer = new BinaryWriter(new FileStream(@"..\Data.bin", FileMode.Create, FileAccess.Write, FileShare.None))) {
                    PdfData.ReadFromFiles().WriteTo(writer);
                }
            }
            catch(Exception err) {
                while (err != null) {
                    Console.WriteLine("ERROR: " + err.Message);
                    err = err.InnerException;
                }
            }
#endif
        }
    }
}
