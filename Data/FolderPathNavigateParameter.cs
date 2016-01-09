using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace favoshelf.Data
{
    public class FolderPathNavigateParameter : INavigateParameter
    {
        private string m_folderPath;

        public FolderPathNavigateParameter(string folderPath)
        {
            m_folderPath = folderPath;
        }

        public async Task<IReadOnlyList<FolderListItem>> GetItemList()
        {
            List<FolderListItem> itemList = new List<FolderListItem>();
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(m_folderPath);
            if (folder == null)
            {
                return itemList;
            }

            Size thumSize = await FolderListItem.GetThumSizeFromWindow();
            IReadOnlyList<StorageFolder> subFolderList = await folder.GetFoldersAsync();
            foreach (StorageFolder subFolder in subFolderList)
            {
                itemList.Add(new FolderListItem()
                {
                    Name = subFolder.DisplayName,
                    Path = subFolder.Path,
                    Token = "",
                    Type = FolderListItem.FileType.Folder,
                    ThumWidth = (int)thumSize.Width,
                    ThumHeight = (int)thumSize.Height,
                });
            }

            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                FolderListItem item = new FolderListItem()
                {
                    Name = file.DisplayName,
                    Path = file.Path,
                    Token = "",
                    Type = FolderListItem.GetFileTypeFromStorage(file),
                    ThumWidth = (int)thumSize.Width,
                    ThumHeight = (int)thumSize.Height,
                };
                itemList.Add(item);
            }

            return itemList;
        }
    }
}
