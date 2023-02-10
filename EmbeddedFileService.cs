using Penguin.Files.Embedded.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Penguin.Files.Embedded
{
    public static class EmbeddedFileService
    {
        public static byte[] ReadAllBytes(ResourceExtractionDescriptor descriptor)
        {
            return descriptor is null ? throw new ArgumentNullException(nameof(descriptor)) : TryExtract(descriptor, File.ReadAllBytes);
        }

        public static string ReadAllText(ResourceExtractionDescriptor descriptor)
        {
            return descriptor is null ? throw new ArgumentNullException(nameof(descriptor)) : TryExtract(descriptor, File.ReadAllText);
        }

        private static byte[] ReadResource(Assembly assembly, string resourceName)
        {
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new ArgumentException($"No resource was found in the assembly '{assembly.FullName}' with the name '{resourceName}'");
            }

            using BinaryReader reader = new(stream);
            return reader.ReadAllBytes();
        }

        public static IEnumerable<ResourceDescriptor> EnumerateFiles(ResourceEnumerationDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            IEnumerable<string> files = descriptor.SourceAssembly.GetManifestResourceNames();

            if (!string.IsNullOrWhiteSpace(descriptor.SearchDirectory))
            {
                files = files.Where(r => r.StartsWith(descriptor.GetSearchRoot()));
            }

            foreach (string f in files)
            {
                yield return new ResourceDescriptor()
                {
                    SourceAssembly = descriptor.SourceAssembly,
                    Namespace = descriptor.Namespace,
                    ResourceFullPath = f
                };
            }
        }

        public static bool TryExtract(ResourceExtractionDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            FileInfo resourceFile = new(descriptor.OutputPath);

            if (!resourceFile.Exists)
            {
                if (!resourceFile.Directory.Exists)
                {
                    resourceFile.Directory.Create();
                }

                File.WriteAllBytes(descriptor.OutputPath, ReadResource(descriptor.SourceAssembly, descriptor.GetResourcePath()));

                return true;
            }

            return false;
        }

        private static T TryExtract<T>(ResourceExtractionDescriptor descriptor, Func<string, T> toCall)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            if (toCall is null)
            {
                throw new ArgumentNullException(nameof(toCall));
            }

            _ = TryExtract(descriptor);

            return toCall.Invoke(descriptor.OutputPath);
        }
    }
}