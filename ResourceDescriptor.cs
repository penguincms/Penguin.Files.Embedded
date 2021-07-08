using System;
using System.Reflection;

namespace Penguin.Files.Embedded
{
    public class ResourceDescriptor
    {
        /// <summary>
        /// The assembly to search for the resource in. If defaults to executing
        /// </summary>
        public Assembly SourceAssembly { get; set; } = Assembly.GetExecutingAssembly();


        /// <summary>
        /// Full path to the resource, overrides any other values
        /// </summary>
        public string ResourceFullPath { get; set; }


        /// <summary>
        /// The namespace the resource is rooted in
        /// </summary>
        public string Namespace { get; set; }
    }

    public class ResourceEnumerationDescriptor : ResourceDescriptor
    {
        public string SearchDirectory { get; set; }

        public string GetSearchRoot() => $"{this.Namespace ?? this.SourceAssembly.GetName().Name}.{this.SearchDirectory.Replace("\\", ".")}.";
    }

    /// <summary>
    /// Defines how a resource should be extracted from an assembly, where it should be extracted to, and what assembly is being targeted
    /// </summary>
    public class ResourceExtractionDescriptor : ResourceDescriptor
    {

        public static implicit operator ResourceExtractionDescriptor(string path) => new ResourceExtractionDescriptor()
        {
            ResourceLocalPath = path,
            OutputPath = path
        };

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

        /// <summary>
        /// The path to the resource inside of the containing assembly
        /// </summary>
        public string ResourceLocalPath { get; set; }

        /// <summary>
        /// Gets the full path including namespace, using the provided information
        /// </summary>
        /// <returns></returns>
        public string GetResourcePath()
        {
            if (this.ResourceFullPath != null)
            {
                return this.ResourceFullPath;
            }

            string targetNs = this.Namespace ?? this.SourceAssembly.GetName().Name;

            return $"{targetNs}.{this.ResourceLocalPath.Replace("\\", ".")}";
        }

        /// <summary>
        /// The file path that the embedded resource should be extracted to on disk
        /// </summary>
        public string OutputPath { get; set; }
    }
}
