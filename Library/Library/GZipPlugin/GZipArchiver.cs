using System;
using System.IO;
using System.ComponentModel;
using PluginInterface;
using System.IO.Compression;

namespace GZipPlugin
{
    [Description("GZip-архиватор")]
    public class GZipArchiver : IPlugin
    {
        public void Archive(string fileName, Stream sourceStream)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (GZipStream gZipStream = new GZipStream(fs, CompressionMode.Compress))
                {
                    sourceStream.Seek(0, SeekOrigin.Begin);
                    sourceStream.CopyTo(gZipStream);
                }
            }
        }

        public Stream Dearchive(string fileName)
        {
            Stream stream = new MemoryStream();
            FileStream fs = new FileStream(fileName, FileMode.Open);
            GZipStream gZipStream = new GZipStream(fs, CompressionMode.Decompress);
            gZipStream.CopyTo(stream);
            return stream;
        }

        public string GetExt()
        {
            return "gz";
        }
    }
}
