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
    public class BookItemNavigateParameter : INavigateParameter
    {
        private BookshelfDatabase m_db;
        private string m_label;

        public BookItemNavigateParameter(string label)
        {
            m_label = label;
        }

        public BookItemNavigateParameter(BookshelfDatabase db, string label)
        {
            m_db = db;
            m_label = label;
        }

        public async Task<IReadOnlyList<FolderListItem>> GetItemList()
        {
            if (m_db == null)
            {
                m_db = new BookshelfDatabase();
            }

            List<FolderListItem> itemList = new List<FolderListItem>();
            Size thumSize = await FolderListItem.GetThumSizeFromWindow();
            Bookshelf bookshelf = m_db.SelectBookshelf(m_label);
            foreach (BookItem bookItem in m_db.SelectBookList(bookshelf))
            {
                if (!StorageApplicationPermissions.FutureAccessList.ContainsItem(bookItem.Token))
                {
                    continue;
                }

                IStorageItem storageItem = await StorageApplicationPermissions.FutureAccessList.GetItemAsync(bookItem.Token);
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
                    });
                }
            }
            
            return itemList;
        }
    }
}
