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
        public enum FileType
        {
            Folder,
            ImageFolder,
            Archive,
            File
        }

        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public FileType Type
        {
            get;
            set;
        }

        public string BackgroundColor
        {
            get
            {
                string color = "#FFFFFF";
                switch (this.Type)
                {
                    case FileType.Folder:
                        color = "#FFFACD";
                        break;
                    case FileType.ImageFolder:
                        color = "#E0FFFF";
                        break;
                    case FileType.Archive:
                        color = "#F0F8FF";
                        break;
                    case FileType.File:
                    default:
                        color = "#FFFFFF";
                        break;
                }
                return color;
            }
        }

    }
}
