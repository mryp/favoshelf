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
    public class BookItemNavigateParameter : INavigateParameter
    {
        private LocalDatabase m_db;
        private string m_label;

        public BookItemNavigateParameter(string label)
        {
            m_label = label;
        }

        public BookItemNavigateParameter(LocalDatabase db, string label)
        {
            m_db = db;
            m_label = label;
        }

        public async Task<IReadOnlyList<FolderListItem>> GetItemList()
        {
            if (m_db == null)
            {
                m_db = new LocalDatabase();
            }

            List<FolderListItem> itemList = new List<FolderListItem>();
            Size thumSize = await FolderListItem.GetThumSizeFromWindow();
            int fontSize = FolderListItem.GetFontSizeFromThumImage((int)thumSize.Width);
            BookCategory category = m_db.QueryBookCategory(m_label);
            foreach (BookItem bookItem in m_db.QueryBookItemList(category))
            {
                if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(bookItem.Token))
                {
                    continue;
                }
                IStorageItem storageItem = await getItemAsync(bookItem.Token);
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
                        Token = bookItem.Token,
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
                        Token = bookItem.Token,
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
                StorageHistoryManager.RemoveStorage(token, StorageHistoryManager.DataType.Bookshelf);
                Debug.WriteLine("BookItemNavigateParameter#getItemAsync 読み込み失敗 e=" + e.Message);
                return null;
            }

            return storageItem;
        }

        public string GetFolderName()
        {
            return m_label;
        }
    }
}
