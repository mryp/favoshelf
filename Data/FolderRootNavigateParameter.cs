using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace favoshelf.Data
{
    public class FolderRootNavigateParameter : INavigateParameter
    {
        public async Task<IReadOnlyList<FolderListItem>> GetItemList()
        {
            List<string> tokenList = StorageHistoryManager.GetTokenList(StorageHistoryManager.DataType.Folder);

            Size thumSize = await FolderListItem.GetThumSizeFromWindow();
            int fontSize = FolderListItem.GetFontSizeFromThumImage((int)thumSize.Width);
            List<FolderListItem> itemList = new List<FolderListItem>();
            foreach (string token in tokenList)
            {
                if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    continue;
                }
                IStorageItem storageItem = await getItemAsync(token);
                if (storageItem == null)
                {
                    continue;
                }

                if (storageItem.IsOfType(StorageItemTypes.Folder))
                {
                    itemList.Add(new FolderListItem()
                    {
                        Name = storageItem.Name,
                        Path = storageItem.Path,
                        Token = token,
                        Type = FolderListItem.FileType.Folder,
                        ThumWidth = (int)thumSize.Width,
                        ThumHeight = (int)thumSize.Height,
                        TextSize = fontSize,
                    });
                }
                else
                {
                    itemList.Add(new FolderListItem()
                    {
                        Name = storageItem.Name,
                        Path = storageItem.Path,
                        Token = token,
                        Type = FolderListItem.GetFileTypeFromStorage((StorageFile)storageItem),
                        ThumWidth = (int)thumSize.Width,
                        ThumHeight = (int)thumSize.Height,
                        TextSize = fontSize,
                    });
                }
            }

            return itemList;
        }

        private async Task<IStorageItem> getItemAsync(string token)
        {
            IStorageItem storageItem = null;
            try
            {
                storageItem = await StorageApplicationPermissions.FutureAccessList.GetItemAsync(token);
            }
            catch (Exception e)
            {
                StorageHistoryManager.RemoveStorage(token, StorageHistoryManager.DataType.Folder);
                Debug.WriteLine("BookItemNavigateParameter#getItemAsync 読み込み失敗 e=" + e.Message);
                return null;
            }

            return storageItem;
        }

        public string GetFolderName()
        {
            return "フォルダー";
        }
    }
}
