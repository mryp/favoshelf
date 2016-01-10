using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace favoshelf.Data
{
    public class BookshelfNavigateParameter : INavigateParameter
    {
        private LocalDatabase m_db;

        public BookshelfNavigateParameter()
        {
            m_db = null;
        }

        public BookshelfNavigateParameter(LocalDatabase db)
        {
            m_db = db;
        }

        public async Task<IReadOnlyList<FolderListItem>> GetItemList()
        {
            if (m_db == null)
            {
                m_db = new LocalDatabase();
            }
            
            Size thumSize = await FolderListItem.GetThumSizeFromWindow();
            List<FolderListItem> itemList = new List<FolderListItem>();
            IEnumerable<BookCategory> bookshelfList = m_db.QueryBookCategoryAll();
            if (bookshelfList == null)
            {
                return itemList;
            }

            foreach (BookCategory bookshelf in bookshelfList)
            {
                itemList.Add(new FolderListItem()
                {
                    Name = bookshelf.Label,
                    Path = bookshelf.Id.ToString(),
                    Token = "",
                    Type = FolderListItem.FileType.Bookshelf,
                    ThumWidth = (int)thumSize.Width,
                    ThumHeight = (int)thumSize.Height
                });
            }

            return itemList;
        }
    }
}
