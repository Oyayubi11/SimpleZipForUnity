using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SZip
{
    /// <summary>
    /// SZip使用例/テスト
    /// </summary>
    public class SZipExample : MonoBehaviour
    {
        void Start()
        {
            //SZipソースを圧縮
            string SZipDir = Path.Combine(Application.dataPath, "Plugins/SZip/Scripts");
            DirectoryInfo dirInfo = new DirectoryInfo(SZipDir);
            if (!dirInfo.Exists)
            {
                Debug.LogError("not found SZip scripts dir : " + SZipDir);
                return;
            }

            FileInfo[] fileInfos = dirInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly);
            if (fileInfos == null || fileInfos.Length <= 0)
            {
                Debug.LogError("not found SZip scripts");
                return;
            }

            Debug.Log("SZip is going to containt " + fileInfos.Length + " files");

            SZip.SZipFileDeflate szipDeflate = new SZip.SZipFileDeflate();
            foreach (var info in fileInfos)
            {
                var data = File.ReadAllBytes(info.FullName);
                szipDeflate.EntryContent("SZip/Scripts/" + info.Name, info);
            }

            var compressed = szipDeflate.Deflate();

            //圧縮したデータを解凍して比較
            SZip.SZipFileInflate szipInflate = new SZip.SZipFileInflate();
            szipInflate.Prepare(compressed);

            //要素数チェック
            if (szipInflate.ContentCount != fileInfos.Length)
            {
                Debug.LogError("Content Count does not match");
                return;
            }

            for (int contentIndex = 0; contentIndex < szipInflate.ContentCount; contentIndex++)
            {
                var originalData = File.ReadAllBytes(fileInfos[contentIndex].FullName);
                var originalDataHash = SZip.SZipUtility.FNVHash(originalData);

                //圧縮前データサイズチェック
                if (originalData.Length != szipInflate.GetContent(contentIndex).OriginalDataSize)
                {
                    Debug.LogError("Original Data Size does not match" + fileInfos[contentIndex].Name);
                }

                //解凍後ハッシュ値チェック
                var uncompressData = szipInflate.Inflate(contentIndex);
                var uncompressDataHash = SZip.SZipUtility.FNVHash(uncompressData);
                if (originalDataHash != uncompressDataHash)
                {
                    Debug.LogError("Uncompress Data does not match : " + fileInfos[contentIndex].Name);
                }

                Debug.Log("Uncompressed " + szipInflate.GetContentName(contentIndex));
            }
            Debug.Log("Compress/Uncompress success all");
        }
    }
}
