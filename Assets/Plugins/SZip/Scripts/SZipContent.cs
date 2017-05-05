using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZip
{
    /// <summary>
    /// SZIP 1要素情報
    /// SZIP Contents Infomation
    /// </summary>
    public struct SZipContent
    {
        #region public
        /// <summary>
        /// バイナリより要素情報読み取り
        /// read content information from binary
        /// </summary>
        public void Read(System.IO.BinaryReader reader)
        {
            _nameByteSize         = reader.ReadInt32();
            _originalDataSize   = reader.ReadInt32();
            _compressedDataSize = reader.ReadInt32();
            _compressedDataCrc  = reader.ReadUInt32();
        }

        /// <summary>
        /// 要素情報設定
        /// set content information
        /// </summary>
        public void Set( System.Int32 nameByteSize,
                         System.Int32 originalDataSize,
                         System.Int32 compressedDataSize,
                         System.UInt32 compressedDataCrc )
        {
            _nameByteSize       = nameByteSize;
            _originalDataSize   = originalDataSize;
            _compressedDataSize = compressedDataSize;
            _compressedDataCrc  = compressedDataCrc;
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(_nameByteSize);
            writer.Write(_originalDataSize);
            writer.Write(_compressedDataSize);
            writer.Write(_compressedDataCrc);
        }


        public void Dump()
        {
            Debug.Log("NameByteSize        : " + NameByteSize);
            Debug.Log("OriginalDataSize    : " + OriginalDataSize);
            Debug.Log("CompressedDataSize  : " + CompressedDataSize);
            Debug.Log("CompressedDataCrc   : " + CompressedDataCrc);
        }

        public static System.Int32 Size
        {
            get
            {
                return 4     //name byte size
                     + 4     //original data size
                     + 4     //compressed data size
                     + 4     //compressed data crc
                     ;
            }
        }

        /// <summary>
        /// 要素名 サイズ(Byte)
        /// </summary>
        public System.Int32  NameByteSize { get { return _nameByteSize; } }

        /// <summary>
        /// 圧縮前データサイズ
        /// </summary>
        public System.Int32  OriginalDataSize { get { return _originalDataSize;  } }

        /// <summary>
        /// 圧縮後データサイズ
        /// </summary>
        public System.Int32  CompressedDataSize { get { return _compressedDataSize; } }

        /// <summary>
        /// 圧縮後データハッシュ値
        /// </summary>
        public System.UInt32 CompressedDataCrc { get { return _compressedDataCrc; } }
        #endregion

        #region private
        private System.Int32  _nameByteSize;
        private System.Int32  _originalDataSize;
        private System.Int32  _compressedDataSize;
        private System.UInt32 _compressedDataCrc;
        #endregion
    }
}
