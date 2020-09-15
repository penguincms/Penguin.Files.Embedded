using Penguin.Files.Embedded.Extensions;
using System;
using System.IO;
using System.Reflection;

namespace Penguin.Files.Embedded
{
    public static class EmbeddedFileService
    {
        public static byte[] ReadAllBytes(string path, Assembly source) {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty", nameof(path));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return TryExtract(path, source, File.ReadAllBytes); 
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return ReadAllBytes(path, Assembly.GetCallingAssembly());
        }

        public static string ReadAllText(string path, Assembly source)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return TryExtract(path, source, File.ReadAllText);
        }

        public static string ReadAllText(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return ReadAllText(path, Assembly.GetCallingAssembly());
        }

        private static byte[] ReadResource(Assembly assembly, string resourceName)
        {
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return reader.ReadAllBytes();
            }
        }

        private static T TryExtract<T>(string path, Assembly assembly, Func<string, T> toCall)
        {
            string resourceName = path.Replace("/", ".").Replace("\\", ".");

            resourceName = $"{assembly.GetName().Name}.{resourceName}";

            FileInfo resourceFile = new FileInfo(path);
      
            if (!resourceFile.Exists)
            {
                if(!resourceFile.Directory.Exists)
                {
                    resourceFile.Directory.Create();
                }

                File.WriteAllBytes(path, ReadResource(assembly, resourceName));
            }

            return toCall.Invoke(path);
        }
    }
}