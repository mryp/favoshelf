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
            Archive,
            ImageFile,
            OtherFile
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
