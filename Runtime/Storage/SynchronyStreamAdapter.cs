#if !UNITY_WEBGL || UNITY_EDITOR
#define ASYNCHRONOUS_PLATFORM
#endif
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ActionCode.Persistence
{
    /// <summary>
    /// Static class to adapter write and read operations asynchronously or synchronously 
    /// depending on the current platform.
    /// <para><b>WebGL</b> does not support to read/write files asynchronously.</para>
    /// </summary>
    public static class SynchronyStreamAdapter
    {
#if ASYNCHRONOUS_PLATFORM
        public static async Task Write(string path, string content)
        {
            await using var writer = new StreamWriter(path);
            await writer.WriteAsync(content);
        }

        public static async Task Write(Stream stream, string content)
        {
            await using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteAsync(content);
        }

        public static async Task Write(GZipStream compressor, byte[] bytes) =>
            await compressor.WriteAsync(bytes, 0, bytes.Length);
#else
        public static async Task Write(string path, string content)
        {
            await Task.Yield();
            using var writer = new StreamWriter(path);
            writer.Write(content);
        }

        public static async Task Write(Stream stream, string content)
        {
            await Task.Yield();
            using var streamWriter = new StreamWriter(stream);
            streamWriter.Write(content);
        }

        public static async Task Write(GZipStream compressor, byte[] bytes)
        {
            await Task.Yield();
            compressor.Write(bytes, 0, bytes.Length);
        }
#endif
    }
}