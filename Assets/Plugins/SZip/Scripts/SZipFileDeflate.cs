using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ionic.Zlib;

namespace SZip
{
    /// <summary>
    /// SZip圧縮器
    /// </summary>
    public class SZipFileDeflate
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SZipFileDeflate()
        {
            _contentFileInfoTable = new List<FileInfo>();
            _contentNameTable = new List<string>();
        }

        /// <summary>
        /// 圧縮対象ファイル追加
        /// </summary>
        public void EntryContent(string contentName, System.IO.FileInfo fileInfo )
        {
            _contentFileInfoTable.Add(fileInfo);
            _contentNameTable.Add(contentName);
        }

        /// <summary>
        /// 追加済み圧縮対象データ
        /// </summary>
        public System.IO.FileInfo GetFileInfo(int index)
        {
            return _contentFileInfoTable[index];
        }

        /// <summary>
        /// 追加済み圧縮対象名前
        /// </summary>
        public string GetContentName(int index)
        {
            return _contentNameTable[index];
        }

        /// <summary>
        /// 圧縮
        /// </summary>
        public byte[] Deflate()
        {
            //データ部圧縮
            //compress datas
            List<int> originalContentDataSizeTable = new List<int>();
            List<byte[]> compressedContentDataTable = new List<byte[]>(_contentFileInfoTable.Count);
            int compressedContentDataTableSize = 0;
            for (int contentIndex = 0; contentIndex < _contentFileInfoTable.Count; contentIndex++)
            {
                var fileInfo = _contentFileInfoTable[contentIndex];
                var data = System.IO.File.ReadAllBytes(fileInfo.FullName);
                originalContentDataSizeTable.Add(data.Length);
                var compressed = ZlibStream.CompressBuffer(data);
                compressedContentDataTableSize += compressed.Length;
                compressedContentDataTable.Add(compressed);
            }

            //ファイル名部圧縮
            //compress name table
            byte[] compressedContentNameTable = null;
            int compressedContentNameTableSize = 0;
            {
                //全ファイル名結合
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (var s in _contentNameTable)
                {
                    builder.Append(s);
                    compressedContentNameTableSize += System.Text.Encoding.UTF8.GetByteCount(s);
                }

                compressedContentNameTable = ZlibStream.CompressString(builder.ToString());
            }

            //コンテンツ情報部圧縮
            //compress contents
            byte[] compressedContentTable = null;
            {
                byte[] headerTable = new byte[SZipContent.Size * _contentFileInfoTable.Count];
                using (MemoryStream stream = new MemoryStream(headerTable))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        for (int contentIndex = 0; contentIndex < _contentFileInfoTable.Count; contentIndex++)
                        {
                            var content = new SZipContent();
                            System.UInt32 crc = SZipUtility.FNVHash(compressedContentDataTable[contentIndex]);
                            content.Set(_contentNameTable[contentIndex].Length,
                                        originalContentDataSizeTable[contentIndex],
                                        compressedContentDataTable[contentIndex].Length,
                                        crc);
                            content.Write(writer);
                        }
                    }
                }
                compressedContentTable = ZlibStream.CompressBuffer(headerTable);
            }

            //ヘッダ生成
            //create szip header
            var header = new SZipHeader();
            header.Set((System.Int16)_contentFileInfoTable.Count,
                       SZipUtility.FNVHash(compressedContentNameTable),
                       SZipUtility.FNVHash(compressedContentTable),
                       compressedContentTable.Length,
                       compressedContentNameTableSize,
                       compressedContentNameTable.Length);

            //書き込み
            //write to byte[]
            {
                int totalSize = SZipHeader.Size
                              + compressedContentTable.Length
                              + compressedContentNameTable.Length
                              + compressedContentDataTableSize;

                byte[] output = new byte[totalSize];

                using (MemoryStream stream = new MemoryStream(output))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        //ヘッダ
                        header.Write(writer);

                        //コンテンツ
                        writer.Write(compressedContentTable);

                        //コンテンツ名
                        writer.Write(compressedContentNameTable);

                        //データ
                        foreach (var data in compressedContentDataTable)
                        {
                            writer.Write(data);
                        }
                    }
                }

                return output;
            }
        }

        private List<System.IO.FileInfo> _contentFileInfoTable;
        private List<string> _contentNameTable;
    }
}