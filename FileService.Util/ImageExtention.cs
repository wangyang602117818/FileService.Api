using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileService.Util
{
    public static class ImageExtention
    {
        private static string GetImageType(Stream stream)
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, 0, 10);
            if (buffer[0] == 'G' && buffer[1] == 'I' && buffer[2] == 'F') return ".gif";
            if (buffer[1] == 'P' && buffer[2] == 'N' && buffer[3] == 'G') return ".png";
            if (buffer[6] == 'J' && buffer[7] == 'F' && buffer[8] == 'I' && buffer[9] == 'F') return ".jpg";
            if (buffer[0] == 'B' && buffer[1] == 'M') return ".bmp";
            return null;
        }
        public static string GetImageType2(Stream stream)
        {
            string headerCode = GetHeaderInfo(stream).ToUpper();
            if (headerCode.StartsWith("FFD8FF"))
            {
                return "JPG";
            }
            else if (headerCode.StartsWith("49492A"))
            {
                return "TIFF";
            }
            else if (headerCode.StartsWith("424D"))
            {
                return "BMP";
            }
            else if (headerCode.StartsWith("474946"))
            {
                return "GIF";
            }
            else if (headerCode.StartsWith("89504E470D0A1A0A"))
            {
                return "PNG";
            }
            else if (headerCode.StartsWith("3C3F786D6C"))
            {
                return "XML";
            }
            else
            {
                return "";
            }
        }
        public static string GetHeaderInfo(Stream stream)
        {
            byte[] buffer = new byte[8];
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true);
            reader.Read(buffer, 0, buffer.Length);
            reader.Close();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in buffer)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
