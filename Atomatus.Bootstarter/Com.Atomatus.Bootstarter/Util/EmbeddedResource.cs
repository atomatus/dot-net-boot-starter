using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// <para>
    /// Embedded Resources from current Assembly or other accessible assembly.
    /// </para>
    /// <para>
    /// For better usage, have sure that the resource is set to Embedded on Build Action property.
    /// </para>
    /// <para>
    /// <strong> For example: </strong><br/>
    /// 1 - Create or paste resource on project;<br/>
    /// 2 - Click over resource with right mouse button;<br/>
    /// 3 - Go to Properties and change "Build Action" to "Embedded Resource" (https://i.stack.imgur.com/qimwi.png)
    /// </para>
    /// </summary>
    /// <author>Carlos Matos</author>
    /// <date>2021-01-19</date>
    internal sealed class EmbeddedResource : IDisposable
    {
        private readonly WeakReference<string> contentRef;

        private Stream stream;
        private Assembly assembly;
        private string resourcePath;
        private bool disposed;

        /// <summary>
        /// <para>
        /// Load an embedded resource from target assembly or whether assembly not set load from <see cref="Assembly.GetEntryAssembly"/>.
        /// </para>
        /// <para>
        /// Resource path is defined by following format:<br/>
        /// <strong>Format: </strong> [AssemblyNameSpace|OrEmptyToUseFromInputAssembly].[FoldersSeparatedByDot].[filename].[Extension]<br/>
        /// <strong>Example:</strong> <i>Cogtive.Core.Domain.Resources.Template.html</i>
        /// </para>
        /// </summary>
        /// <param name="resourcePath">resource relative or absolute path, see above the example</param>
        /// <param name="assembly">assembly where resource is keeped it, if null, load from <see cref="Assembly.GetEntryAssembly"/></param>
        /// <returns>loaded embedded resource</returns>
        public static EmbeddedResource Load(string resourcePath, Assembly assembly = null)
        {
            return TryLoad(resourcePath, assembly, out EmbeddedResource resource) ?
                resource : throw new FileNotFoundException($"Resource ({resourcePath}) don't exists or " +
                    $"is not accessible by Assembly ({(assembly ?? Assembly.GetEntryAssembly()).GetName().Name})!");
        }

        /// <summary>
        /// <para>
        /// Try load an embedded resource from target assembly or whether assembly not set load from <see cref="Assembly.GetEntryAssembly"/>.
        /// </para>
        /// <para>
        /// Resource path is defined by following format:<br/>
        /// <strong>Format: </strong> [AssemblyNameSpace|OrEmptyToUseFromInputAssembly].[FoldersSeparatedByDot].[filename].[Extension]<br/>
        /// <strong>Example:</strong> <i>Cogtive.Core.Domain.Resources.Template.html</i>
        /// </para>
        /// </summary>
        /// <param name="resourcePath">resource relative or absolute path, see above the example</param>
        /// <param name="resource">embedded resource found</param>
        /// <returns>true if resource is found, otherwhise false</returns>
        public static bool TryLoad(string resourcePath, out EmbeddedResource resource)
        {
            return TryLoad(resourcePath, null, out resource);
        }

        /// <summary>
        /// <para>
        /// Try load an embedded resource from target assembly or whether assembly not set load from <see cref="Assembly.GetEntryAssembly"/>.
        /// </para>
        /// <para>
        /// Resource path is defined by following format:<br/>
        /// <strong>Format: </strong> [AssemblyNameSpace|OrEmptyToUseFromInputAssembly].[FoldersSeparatedByDot].[filename].[Extension]<br/>
        /// <strong>Example:</strong> <i>Cogtive.Core.Domain.Resources.Template.html</i>
        /// </para>
        /// </summary>
        /// <param name="resourcePath">resource relative or absolute path, see above the example</param>
        /// <param name="assembly">target assembly</param>
        /// <param name="resource">embedded resource found</param>
        /// <returns>true if resource is found, otherwhise false</returns>
        public static bool TryLoad(string resourcePath, Assembly assembly, out EmbeddedResource resource)
        {
            return TryLoad(resourcePath, assembly, true, out resource);
        }

        private static bool TryLoad(string resourcePath, Assembly assembly, bool useReferencedAssemblies, out EmbeddedResource resource)
        {
            if (resourcePath is null)
            {
                throw new ArgumentNullException(nameof(resourcePath));
            }
            else if (string.IsNullOrWhiteSpace(resourcePath))
            {
                throw new ArgumentException("Resource path can not be empty!");
            }

            resource = default;
            assembly ??= Assembly.GetEntryAssembly();

            string found = assembly.GetManifestResourceNames()
               .SingleOrDefault(res => res.EndsWith(resourcePath, StringComparison.CurrentCultureIgnoreCase));

            if(found != default)
            {
                resource = new EmbeddedResource(assembly, found);
            }
            else if(useReferencedAssemblies)
            {
                AssemblyName[] names = assembly.GetReferencedAssemblies();
                foreach (AssemblyName name in names)
                {
                    if(TryLoad(resourcePath, Assembly.Load(name), false, out resource))
                    {
                        break;
                    }
                }
            }

            return resource != default(EmbeddedResource);
        }

        ~EmbeddedResource()
        {
            this.OnDispose(false);
        }

        private EmbeddedResource(Assembly assembly, string resourcePath)
        {
            this.assembly = assembly;
            this.resourcePath = resourcePath;
            this.contentRef = new WeakReference<string>(null);
        }

        /// <summary>
        /// Open stream reader to get embedded resource content.
        /// </summary>
        /// <returns>embedded resource stream</returns>
        /// <exception cref="ObjectDisposedException">throws when attempt get stream from disposed objected</exception>
        public Stream GetStream()
        {
            this.RequireNonDisposed();
            this.stream?.Dispose();
            return (stream = this.assembly.GetManifestResourceStream(this.resourcePath));
        }

        /// <summary>
        /// Get content from embedded resource.
        /// </summary>
        /// <returns>embedded resource content</returns>
        /// <exception cref="ObjectDisposedException">throws when attempt get data from disposed objected</exception>
        public string GetContent()
        {
            this.RequireNonDisposed();
            lock (contentRef)
            {
                if (!this.contentRef.TryGetTarget(out string content))
                {
                    #pragma warning disable IDE0063
                    using (StreamReader reader = new StreamReader(this.assembly.GetManifestResourceStream(this.resourcePath)))                    
                    {
                        this.contentRef.SetTarget(content = reader.ReadToEnd());
                    }
                    #pragma warning restore IDE0063
                }
                return content;
            }
        }

        /// <summary>
        /// Get content from embedded resource.
        /// </summary>
        /// <returns>embedded resource content</returns>
        /// <exception cref="ObjectDisposedException">throws when attempt get data from disposed objected</exception>
        public Task<string> GetContentAsync()
        {
            this.RequireNonDisposed();
            return Task.Factory.StartNew(GetContent);
        }

        #region IDisposable
        private void RequireNonDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(EmbeddedResource));
            }
        }

        private void OnDispose(bool isDisposing)
        {
            if(isDisposing)
            {
                this.stream?.Dispose();

                lock (contentRef)
                {
                    this.contentRef?.SetTarget(null);
                }
            }

            this.stream = null;
            this.assembly = null;
            this.resourcePath = null;
            this.disposed = true;
        }

        public void Dispose()
        {
            this.OnDispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
