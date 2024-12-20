using System;
using System.IO;
using System.Text;
using System.Reflection;
using Jannesen.FileFormat.Pdf.Internal;

namespace Jannesen.FileFormat.Pdf
{
    public class PdfImage : PdfObject
    {
        private                 byte[]          _imageData;
        private                 int             _imageDataLength;
        private                 int             _height;
        private                 int             _width;
        private                 int             _bitsPerComponent;

        public                  int             Height
        {
            get {
                return _height;
            }
        }
        public                  int             Width
        {
            get {
                return _width;
            }
        }

        public  override        string          NamedType           { get => "XObject";     }
        public  override        bool            hasStream           { get => true;          }

        public                                  PdfImage(Stream imageStream)
        {
            ArgumentNullException.ThrowIfNull(imageStream);

            var header = new byte[16];

            if (imageStream.Read(header, 0, 16) == 16 &&
                header[0] == 0xFF
             && header[1] == 0xD8
             && header[2] == 0xFF
             && header[3] == 0xE0
             && header[6] == (byte)'J'
             && header[7] == (byte)'F'
             && header[8] == (byte)'I'
             && header[9] == (byte)'F')
            {
                _LoadJPG(imageStream);
            }
            else
                throw new PdfExceptionWriter("Unsupported image format.");
        }
        public  static          PdfImage        LoadFromFile(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);

            try {
                using (FileStream imageStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    return new PdfImage(imageStream);
                }
            }
            catch(Exception Err) {
                throw new PdfExceptionWriter("Error reading '"+fileName+"': "+Err.Message);
            }
        }
        public  static          PdfImage        LoadFromAssambly(Assembly assembly, string name)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            ArgumentNullException.ThrowIfNull(name);

            Stream imageStream = null;

            try {
                imageStream = assembly.GetManifestResourceStream(name);

                if (imageStream == null)
                    throw new InvalidOperationException("Can't get resource '" + name + "'.");

                return new PdfImage(imageStream);
            }
            catch(Exception Err) {
                throw new PdfExceptionWriter("Error reading '"+name+"' from assembly '" + assembly.FullName + "': "+Err.Message);
            }
            finally {
                imageStream?.Close();
            }
        }

        internal override       void            pdfWriteToDocument(PdfDocumentWriter document, PdfStreamWriter writer)
        {
            writer.WriteDictionaryBegin();
            {
                writer.WriteName("Type");
                writer.WriteName("XObject");

                writer.WriteName("Subtype");
                writer.WriteName("Image");

                writer.WriteName("Width");
                writer.WriteInteger(_width);

                writer.WriteName("Height");
                writer.WriteInteger(_height);

                writer.WriteName("ColorSpace");
                writer.WriteName("DeviceRGB");

                writer.WriteName("BitsPerComponent");
                writer.WriteInteger(_bitsPerComponent);

                writer.WriteName("Filter");
                writer.WriteName("DCTDecode");

                writer.WriteName("Interpolate");
                writer.WriteBoolean(true);

                writer.WriteName("Length");
                writer.WriteInteger(_imageDataLength);
            }
            writer.WriteDictionaryEnd();
            writer.WriteStream(_imageData, _imageDataLength);
        }

        private                 void            _LoadJPG(Stream imageStream)
        {
            imageStream.Position = 0;

            _imageDataLength = (int)imageStream.Length;
            _imageData       = new byte[_imageDataLength];

            if (imageStream.Read(_imageData, 0, _imageDataLength) != _imageDataLength)
                throw new PdfExceptionWriter("Read error.");

            int     pos = 2;

            for (;;) {
                if (pos > _imageDataLength - 2 || _imageData[pos] != 0xFF)
                    throw new PdfExceptionWriter("Corrupt JPEG image.");

                switch(_imageData[pos + 1]) {
                case 0xE0:  // APP0
                    break;

                case 0xC0: // SOF
                case 0xC1:
                case 0xC2:
                    if (_imageData[pos + 4] != 0x08)
                        throw new PdfExceptionWriter("Unsupported JPEG image (BitsPerSample <> 8).");

                    _height           = _imageData[pos + 5] << 8 | _imageData[pos + 6];
                    _width            = _imageData[pos + 7] << 8 | _imageData[pos + 8];
                    _bitsPerComponent = 8;
                    return ;

//              case 0xDA:  // Start Of Scan
//              case 0xD9:  // End Of Image
//                  return ;
                }

                pos += 2 + (_imageData[pos + 2] << 8 | _imageData[pos + 3]);
            }
        }
    }
}
