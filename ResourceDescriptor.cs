using System;
using System.Reflection;

namespace Penguin.Files.Embedded
{
    public class ResourceDescriptor
    {
        public Assembly SourceAssembly { get; set; } = Assembly.GetExecutingAssembly();

        public string ResourceFullPath { get; set; }

        public string Namespace { get; set; }
    }

    public class ResourceEnumerationDescriptor : ResourceDescriptor
    {
        public string SearchDirectory { get; set; }

        public string GetSearchRoot()
        {
            return $"{(this.Namespace ?? this.SourceAssembly.GetName().Name)}.{this.SearchDirectory.Replace("\\", ".")}.";
        }
    }

    public class ResourceExtractionDescriptor : ResourceDescriptor
    {
        public ResourceExtractionDescriptor()
        {
        }

        public ResourceExtractionDescriptor(ResourceDescriptor descriptor)
        {
            if (descriptor is null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            this.Namespace = descriptor.Namespace;
            this.SourceAssembly = descriptor.SourceAssembly;
            this.ResourceFullPath = descriptor.ResourceFullPath;
        }

        public string ResourcePath { get; set; }

        public string ResourceLocalPath { get; set; }

        public string GetResourcePath()
        {
            if (this.ResourceFullPath != null)
            {
                return this.ResourceFullPath;
            }

            string targetNs = this.Namespace ?? this.SourceAssembly.GetName().Name;

            return $"{targetNs}.{this.ResourceLocalPath.Replace("\\", ".")}";
        }

        public string OutputPath { get; set; }
    }
}
