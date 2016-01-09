using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf
{
    /// <summary>
    /// ファイル種別判定クラス
    /// </summary>
    public class FileKind
    {
        /// <summary>
        /// 指定したファイルが画像ファイルかどうか
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsImageFile(string path)
        {
            bool result = false;
            switch (Path.GetExtension(path).ToLower())
            {
                case ".jpg":
                case ".jpe":
                case ".jpeg":
                case ".png":
                    result = true;
                    break;
            }

            return result;
        }

        public static List<string> GetImageFilterList()
        {
            List<string> fileTypeFilter = new List<string>();
            fileTypeFilter.Add(".png");
            fileTypeFilter.Add(".jepg");
            fileTypeFilter.Add(".jpg");

            return fileTypeFilter;
        }

        /// <summary>
        /// 指定したファイルがアーカイブファイルかどうか
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsArchiveFile(string path)
        {
            bool result = false;
            switch (Path.GetExtension(path).ToLower())
            {
                case ".zip":
                    result = true;
                    break;
            }

            return result;
        }
    }
}
