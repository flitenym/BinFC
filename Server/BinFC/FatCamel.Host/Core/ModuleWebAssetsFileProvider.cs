using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Runtime.InteropServices;

namespace FatCamel.Host.Core
{
    class ModuleWebAssetsFileProvider : IFileProvider
    {
        private static readonly StringComparison FilePathComparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            StringComparison.OrdinalIgnoreCase :
            StringComparison.Ordinal;

        private PathString _basePath;
        private PhysicalFileProvider _innerProvider;

        public ModuleWebAssetsFileProvider(string prefix, string root)
        {
            _basePath = new PathString(prefix.StartsWith("/") ? prefix : "/" + prefix);
            _innerProvider = new PhysicalFileProvider(root);
        }

        /// <inheritdoc />
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!StartsWithBasePath(subpath, out var physicalPath))
                return NotFoundDirectoryContents.Singleton;
            else
                return _innerProvider.GetDirectoryContents(physicalPath);
        }

        /// <inheritdoc />
        public IFileInfo GetFileInfo(string subpath)
        {
            if (!StartsWithBasePath(subpath, out var physicalPath))
                return new NotFoundFileInfo(subpath);
            else
                return _innerProvider.GetFileInfo(physicalPath);
        }

        /// <inheritdoc />
        public IChangeToken Watch(string filter) => _innerProvider.Watch(filter);

        private bool StartsWithBasePath(string subpath,
                                        out PathString rest) => new PathString(subpath).StartsWithSegments(_basePath,
                                                                                                           FilePathComparison,
                                                                                                           out rest);
    }
}