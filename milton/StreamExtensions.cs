using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Get the bytes from a stream (max 2GB)
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Byte[] GetBytes(this Stream stream)
        {
            Int32 length = (Int32)stream.Length;
            Byte[] bytes = new Byte[length];
            stream.Read(bytes, 0, length);
            return bytes;
        }
    }
}
