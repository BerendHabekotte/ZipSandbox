using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ZipInterfaces
{
    public interface IZipHelper
    {
        void ZipFiles(IEnumerable<FileInfo> files, string sourceFolder, string fileName);
        void ZipByteArrays(Dictionary<string, byte[]> sources, string fileName, ZipArchiveMode mode);
    }
}
