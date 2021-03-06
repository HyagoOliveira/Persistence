using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Compressor for the GZip data format, which uses an industry-standard algorithm for lossless 
    /// file compression and decompression.
    /// The format includes a cyclic redundancy check value for detecting data corruption.
    /// </summary>
    public sealed class GZipCompressor : ICompressor
    {
        public string Compress(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using var stream = new MemoryStream();
            var compressor = new GZipStream(stream, CompressionMode.Compress);
            compressor.Write(bytes, 0, bytes.Length);
            compressor.Dispose();

            var output = stream.ToArray();
            return Convert.ToBase64String(output); ;
        }

        public string Decompress(string value)
        {
            var bytes = Convert.FromBase64String(value);
            using var stream = new MemoryStream(bytes);
            using var decompressor = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new StreamReader(decompressor);
            return reader.ReadToEnd();
        }
    }
}