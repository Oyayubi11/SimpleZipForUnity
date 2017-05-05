using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZip
{
    /// <summary>
    /// SZIP情報
    /// </summary>
    public struct SZipHeader
    {
        #region public

        /// <summary>
        /// バイナリよりSZipヘッダ情報を読み取ります
        /// read szip header infomation from binary
        /// </summary>
        public void Read(System.IO.BinaryReader reader)
        {
            _magic = new System.Char[4];
            _magic[0] = reader.ReadChar();
            _magic[1] = reader.ReadChar();
            _magic[2] = reader.ReadChar();
            _magic[3] = reader.ReadChar();
            _version = reader.ReadUInt16();
            _contentCount = reader.ReadInt16();
            _contentNameTableCrc = reader.ReadUInt32();
            _contentHeaderTableCrc = reader.ReadUInt32();
            _compressedContentHeaderTableSize = reader.ReadInt32();
            _originalContentNameTableSize = reader.ReadInt32();
            _compressedContentNameTableSize = reader.ReadInt32();
        }

        /// <summary>
        /// SZipヘッダ情報を設定します。
        /// set szip header information
        /// </summary>
        public void Set( System.Int16 contentCount,                    //要素数
                         System.UInt32 contentNameTableCrc,             //要素名テーブル圧縮後Hash値
                         System.UInt32 contentHeaderTableCrc,           //要素情報テーブル圧縮後Hash値
                         System.Int32 compressedContentHeaderTableSize, //要素情報テーブル圧縮後サイズ
                         System.Int32 originalContentNameTableSize,     //要素名テーブル圧縮前サイズ
                         System.Int32 compressedContentNameTableSize    //要素情報テーブル圧縮後サイズ
            )
        {
            _magic = new System.Char[4] { 'S', 'Z', 'I', 'P' };
            _version = SZipConstant.VERSION;

            _contentCount = contentCount;
            _contentNameTableCrc = contentNameTableCrc;
            _contentHeaderTableCrc = contentHeaderTableCrc;
            _compressedContentHeaderTableSize = compressedContentHeaderTableSize;
            _originalContentNameTableSize = originalContentNameTableSize;
            _compressedContentNameTableSize = compressedContentNameTableSize;
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(_magic[0]);
            writer.Write(_magic[1]);
            writer.Write(_magic[2]);
            writer.Write(_magic[3]);
            writer.Write(_version);
            writer.Write(_contentCount);
            writer.Write(_contentNameTableCrc);
            writer.Write(_contentHeaderTableCrc);
            writer.Write(_compressedContentHeaderTableSize);
            writer.Write(_originalContentNameTableSize);
            writer.Write(_compressedContentNameTableSize);
        }

        public void Dump()
        {
            Debug.Log("Magic[0]                         : " + Magic[0]);
            Debug.Log("Magic[1]                         : " + Magic[1]);
            Debug.Log("Magic[2]                         : " + Magic[2]);
            Debug.Log("Magic[3]                         : " + Magic[3]);
            Debug.Log("Version                          : " + Version);
            Debug.Log("ContentCount                     : " + ContentCount);
            Debug.Log("ContentNameTableCrc              : " + ContentNameTableCrc);
            Debug.Log("ContentHeaderTableCrc            : " + ContentHeaderTableCrc);
            Debug.Log("CompressedContentHeaderTableSize : " + CompressedContentHeaderTableSize);
            Debug.Log("OriginalContentNameTableSize     : " + OriginalContentNameTableSize);
            Debug.Log("CompressedContentNameTableSize   : " + CompressedContentNameTableSize);
        }

        public static int Size
        {
            get
            {
                return 1 * 4      //magic
                     + 2          //version
                     + 2          //content count
                     + 4          //compressed content name table crc
                     + 4          //compressed content table crc
                     + 4          //compressec content table size
                     + 4          //original content name table size
                     + 4;         //compressed content name table size
            }
        }

        /// <summary>
        /// SZIPマジック値
        /// </summary>
        public System.Char[] Magic { get { return _magic; } }

        /// <summary>
        /// SZIPバージョン
        /// </summary>
        public System.UInt16 Version { get { return _version;  } }
        
        /// <summary>
        /// SZIP要素数
        /// </summary>
        public System.Int16  ContentCount { get { return _contentCount; } }
        
        /// <summary>
        /// SZIP要素名テーブル圧縮後Hash値
        /// </summary>
        public System.UInt32 ContentNameTableCrc { get { return _contentNameTableCrc; } }

        /// <summary>
        /// SZIP要素情報テーブル圧縮後Hash値
        /// </summary>
        public System.UInt32 ContentHeaderTableCrc { get { return _contentHeaderTableCrc; } }
        
        /// <summary>
        /// SZIP要素情報圧縮後サイズ
        /// </summary>
        public System.Int32  CompressedContentHeaderTableSize { get { return _compressedContentHeaderTableSize; } }
        
        /// <summary>
        /// SZIP要素名前テーブル圧縮前サイズ
        /// </summary>
        public System.Int32  OriginalContentNameTableSize { get { return _originalContentNameTableSize; } }
        
        /// <summary>
        /// SZIP要素名前テーブル圧縮後サイズ
        /// </summary>
        public System.Int32  CompressedContentNameTableSize { get { return _compressedContentNameTableSize; } }

        #endregion

        #region private
        private System.Char[] _magic;
        private System.UInt16 _version;
        private System.Int16  _contentCount;
        private System.UInt32 _contentNameTableCrc;
        private System.UInt32 _contentHeaderTableCrc;
        private System.Int32  _compressedContentHeaderTableSize;
        private System.Int32  _originalContentNameTableSize;
        private System.Int32  _compressedContentNameTableSize;
        #endregion
    }
}
