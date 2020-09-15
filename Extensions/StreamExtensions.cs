using System.IO;

namespace Penguin.Files.Embedded.Extensions
{
    internal static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            // Pre .Net version 4.0
            const int bufferSize = 4096;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, count);
                }

                return ms.ToArray();
            }
        }
    }
}