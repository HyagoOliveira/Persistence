using System;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;
using ActionCode.AsyncIO;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Compressor for the GZip data format, which uses an industry-standard algorithm for lossless 
    /// file compression and decompression.
    /// The format includes a cyclic redundancy check value for detecting data corruption.
    /// </summary>
    public sealed class GZipCompressor : ICompressor
    {
        private readonly IStream stream;

        public GZipCompressor(IStream stream)
        {
            this.stream = stream;
        }

        public async Task<string> Compress(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using var memoryStream = new MemoryStream();
            var compressor = new GZipStream(memoryStream, CompressionMode.Compress);

            await stream.Write(compressor, bytes);

            compressor.Dispose();
            var output = memoryStream.ToArray();
            return Convert.ToBase64String(output); ;
        }

        public async Task<string> Decompress(string value)
        {
            var bytes = Convert.FromBase64String(value);
            await using var memoryStream = new MemoryStream(bytes);
            await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress);
            return await stream.Read(decompressor);
        }
    }
}