using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf
{
    /// <summary>
    /// フォルダ選択画面で表示する項目アイテム
    /// </summary>
    public class FolderListItem
    {
        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public enum FileType
        {
            Folder,
            Archive,
            ImageFile,
            OtherFile
        }

        /// <summary>
        /// 表示名
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// フルパス（ある場合のみ
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// 権限ありトークン（ある場合のみ）
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public FileType Type
        {
            get;
            set;
        }

        /// <summary>
        /// タイプによる背景色
        /// </summary>
        public string BackgroundColor
        {
            get
            {
                string color = "#FFFFFF";
                switch (this.Type)
                {
                    case FileType.Archive:
                    case FileType.ImageFile:
                        color = "#E0FFFF";
                        break;
                    case FileType.Folder:
                        color = "#FFFACD";
                        break;
                    case FileType.OtherFile:
                    default:
                        color = "#FFFFFF";
                        break;
                }
                return color;
            }
        }
    }
}
