using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace favoshelf.Data
{
    class ScrapbookItemNavigateParameter : INavigateParameter
    {
        private LocalDatabase m_db;
        private string m_folderName;

        public ScrapbookItemNavigateParameter(string folderName)
        {
            m_folderName = folderName;
        }

        public ScrapbookItemNavigateParameter(LocalDatabase db, string folderName)
        {
            m_db = db;
            m_folderName = folderName;
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
            ScrapbookCategory category = m_db.QueryScrapbookCategory(m_folderName);
            StorageFolder folder = await EnvPath.GetScrapbookSubFolder(category.FolderName);
            foreach (ScrapbookItem scrapItem in m_db.QueryScrapbookItemList(category))
            {
                string filePath = Path.Combine(folder.Path, scrapItem.FileName);
                try
                {
                    StorageFile storage = await StorageFile.GetFileFromPathAsync(filePath);
                    itemList.Add(new FolderListItem()
                    {
                        Name = scrapItem.FileName,
                        Path = filePath,
                        Token = "",
                        Type = FolderListItem.GetFileTypeFromStorage((StorageFile)storage),
                        ThumWidth = (int)thumSize.Width,
                        ThumHeight = (int)thumSize.Height,
                        TextSize = fontSize,
                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ScrapbookItemNavigateParameter error=" + e.ToString() + " filePath=" + filePath);
                }
            }

            return itemList;
        }

        public string GetFolderName()
        {
            return m_folderName;
        }
    }
}
