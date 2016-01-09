using System;
using System.Collections.Generic;
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
            List<FolderListItem> itemList = new List<FolderListItem>();
            foreach (string token in tokenList)
            {
                if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    continue;
                }
                
                IStorageItem storageItem = await StorageApplicationPermissions.FutureAccessList.GetItemAsync(token);
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
                    });
                }
            }

            return itemList;
        }
    }
}
