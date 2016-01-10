using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace favoshelf.Data
{
    class ScrapbookNavigateParameter : INavigateParameter
    {
        private LocalDatabase m_db;

        public ScrapbookNavigateParameter()
        {
            m_db = null;
        }

        public ScrapbookNavigateParameter(LocalDatabase db)
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
            IEnumerable<ScrapbookCategory> categoryList = m_db.QueryScrapbookCategoryAll();
            if (categoryList == null)
            {
                return itemList;
            }

            foreach (ScrapbookCategory category in categoryList)
            {
                itemList.Add(new FolderListItem()
                {
                    Name = category.FolderName,
                    Path = category.Id.ToString(),
                    Token = "",
                    Type = FolderListItem.FileType.Scrapbook,
                    ThumWidth = (int)thumSize.Width,
                    ThumHeight = (int)thumSize.Height
                });
            }

            return itemList;
        }
    }
}
