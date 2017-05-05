using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ionic.Zlib;

namespace SZip
{
    public static class SZipUtility
    {
        //streamは読み取り開始位置へSeek済みであること
        public static byte[] ZlibDecompress(MemoryStream stream, int originalSize, int compressedSize)
        {
#if false
            byte[] data = new byte[compressedSize];
            stream.Read(data, 0, data.Length);
            return ZlibStream.UncompressBuffer(data);
#else
            byte[] decompressed = new byte[originalSize];
            byte[] buf = new byte[4096];
            int remain = compressedSize;
            using (MemoryStream toStream = new MemoryStream(decompressed))
            {
                using (ZlibStream zlibStream = new ZlibStream(toStream, CompressionMode.Decompress))
                {
                    while (remain > 0)
                    {
                        int count = (remain > buf.Length) ? buf.Length : remain;
                        stream.Read(buf, 0, count);
                        zlibStream.Write(buf, 0, count);
                        remain -= count;
                    }
                }
            }
            return decompressed;
#endif
        }

        /// <summary>
        /// 32bit Hash値計算
        /// http://isthe.com/chongo/tech/comp/fnv/
        /// </summary>
        public static System.UInt32 FNVHash(byte[] data)
        {
            System.UInt32 hash = 2166136261;

            int remain = data.Length;
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (remain >= sizeof(System.UInt32))
                    {
                        hash = hash ^ reader.ReadUInt32();
                        hash = hash * 16777619;
                        remain -= sizeof(System.UInt32);
                    }
                }
            }

            return hash;
        }
    }
}

