using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ionic.Zlib;

namespace SZip
{
    /// <summary>
    /// SZip展開器
    /// </summary>
    public class SZipFileInflate
    {
        /// <summary>
        /// 展開準備
        /// prepare to inflate
        /// </summary>
        public void Prepare( byte[] data )
        {
            _stream = new MemoryStream(data);

                //ヘッダ読み取り
                //read szip header
                {
                    var binStream = new BinaryReader(_stream);
                    _header = new SZipHeader();
                    _header.Read(binStream);
                }

                //各コンテンツ情報読み取り
                //read each content information
                {
                    //要素情報テーブル解凍
                    _stream.Seek(SZipHeader.Size, SeekOrigin.Begin);
                    var decompressed = SZipUtility.ZlibDecompress(_stream,
                                                                  SZipContent.Size * _header.ContentCount,
                                                                  _header.CompressedContentHeaderTableSize);

                    _contents = new SZipContent[_header.ContentCount];
                    using (MemoryStream stream = new MemoryStream(decompressed))
                    {
                        for (int contentIndex = 0; contentIndex < _header.ContentCount; contentIndex++)
                        {
                            var content = new SZipContent();
                            content.Read(new BinaryReader(stream));
                            _contents[contentIndex] = content;
                        }
                    }
                }

            //要素名読み取り
            //read each content name
            {
                //要素名前テーブル解凍
                _stream.Seek(SZipHeader.Size + _header.CompressedContentHeaderTableSize, SeekOrigin.Begin);
                var decompressed = SZipUtility.ZlibDecompress(_stream, (int)_header.OriginalContentNameTableSize, (int)_header.CompressedContentNameTableSize);

                _fileNameTable = new string[_header.ContentCount];
                _contentNameHashSet = new HashSet<string>();
                using (MemoryStream fnameStream = new MemoryStream(decompressed))
                {
                    using (BinaryReader binReader = new BinaryReader(fnameStream))
                    {
                        for (int contentIndex = 0; contentIndex < _header.ContentCount; contentIndex++)
                        {
                            int length = (int)_contents[contentIndex].NameByteSize;
                            var from = binReader.ReadBytes(length);
                            var contentName = System.Text.Encoding.UTF8.GetString(from);
                            _fileNameTable[contentIndex] = contentName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 要素情報取得
        /// </summary>
        public SZipContent GetContent( string contentName )
        {
            for (int i = 0; i < _contents.Length; i++)
            {
                if (_fileNameTable[i] == contentName)
                {
                    return _contents[i];
                }
            }

            Debug.LogError("Not found SZip Content : " + contentName);
            return default(SZipContent);
        }

        /// <summary>
        /// 要素情報取得
        /// </summary>
        public SZipContent GetContent( int index )
        {
            return _contents[index];
        }

        /// <summary>
        /// 要素名前取得
        /// </summary>
        public string GetContentName( int index )
        {
            return _fileNameTable[index];
        }

        /// <summary>
        /// 要素が含まれるか？
        /// </summary>
        public bool HasContent(string contentName)
        {
            foreach( var n in _fileNameTable )
            {
                if (n == contentName) return true;
            }

            return false;
        }


        /// <summary>
        /// SZIP情報取得
        /// </summary>
        public SZipHeader Header { get { return _header;  } }

        /// <summary>
        /// 要素数取得
        /// </summary>
        public int ContentCount { get { return _contents.Length;  } }

        /// <summary>
        /// 解凍
        /// </summary>
        public byte[] Inflate(string contentName)
        {
            var contentIndex = GetContentIndex(contentName);
            if( contentIndex < 0 )
            {
                Debug.LogError("Not Found Content : " + contentName);
                return null;
            }

            return Inflate(contentIndex);
        }

        /// <summary>
        /// 解凍
        /// </summary>
        public byte[] Inflate(int contentIndex)
        {
            int offset = SZipHeader.Size
                       + _header.CompressedContentHeaderTableSize
                       + _header.CompressedContentNameTableSize;
            for (int i = 0; i < contentIndex; i++)
            {
                offset += _contents[i].CompressedDataSize;
            }

            _stream.Seek(offset, SeekOrigin.Begin);
            byte[] buf = new byte[4 * 1024];
            byte[] uncompressed = new byte[_contents[contentIndex].OriginalDataSize];

            int remain = _contents[contentIndex].CompressedDataSize;
            using (MemoryStream toStram = new MemoryStream(uncompressed))
            {
                using (ZlibStream zlibStream = new ZlibStream(toStram, CompressionMode.Decompress))
                {
                    while (remain > 0)
                    {
                        int count = (remain > buf.Length) ? buf.Length : remain;
                        _stream.Read(buf, 0, count);
                        zlibStream.Write(buf, 0, count);
                        remain -= count;
                    }
                }
            }
            return uncompressed;
        }

        /// <summary>
        /// 要素名->インデックス
        /// </summary>
        private int GetContentIndex(string contentName)
        {
            for(int i = 0; i < _fileNameTable.Length; i++)
            {
                if(_fileNameTable[i] == contentName)
                {
                    return i;
                }
            }
            return -1;
        }

        private SZipHeader  _header;
        private SZipContent[] _contents;
        private string[] _fileNameTable;
        private HashSet<string> _contentNameHashSet;

        private MemoryStream _stream;
    }
}
