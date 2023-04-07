using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using ZipInterfaces;

namespace ZipLibrary
{
    public class ZipHelper : IZipHelper
    {
        public void ZipFiles(IEnumerable<FileInfo> files, string sourceFolder, string fileName)
        {
            try
            {
                using (var stream = File.OpenWrite(fileName))
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Create))
                    {
                        foreach (var item in files)
                        {
                            archive.CreateEntryFromFile(
                                item.FullName,
                                item.FullName.Substring(sourceFolder.Length + 1),
                                CompressionLevel.Optimal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while zipping files: {ex.Message}");
            }
        }

        public void ZipByteArrays(Dictionary<string, byte[]> sources, string fileName, ZipArchiveMode mode)
        {
            try
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (var archive = new ZipArchive(stream, mode))
                    {
                        foreach (var key in sources.Keys)
                        {
                            var entry = archive.CreateEntry(Path.ChangeExtension(key, "pdf"), CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                entryStream.Write(sources[key],0, sources[key].Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while zipping byte arrays: {ex.Message}");
            }
        }
    }
}
