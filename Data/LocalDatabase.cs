using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf.Data
{
    /// <summary>
    /// 本棚カテゴリテーブル
    /// </summary>
    public class BookCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string Label { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }

    /// <summary>
    /// 本棚に格納するための本情報テーブル
    /// </summary>
    public class BookItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int BookCategoryId { get; set; }
        public string Token { get; set; }
        public string Path { get; set; }
        public DateTime Uptime { get; set; }

        public override string ToString()
        {
            return String.Format("Id={0}, BookCategoryId={1}, Token={2}, Path={3}, Uptime={4}"
                , Id, BookCategoryId, Token, Path, Uptime.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }

    /// <summary>
    /// スクラップブックカテゴリ
    /// </summary>
    public class ScrapbookCategory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique]
        public string FolderName { get; set; }
        
        public override string ToString()
        {
            return FolderName;
        }
    }

    public class ScrapbookItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int ScrapbookCategoryId { get; set; }

        public string FileName { get; set; }
        public DateTime Uptime { get; set; }

        public override string ToString()
        {
            return String.Format("Id={0}, ScrapbookCategoryId={1}, FileName={2}, Uptime={3}"
                , Id, ScrapbookCategoryId, FileName, Uptime.ToString("yyyy/MM/dd HH:mm:ss"));
        }
    }

    /// <summary>
    /// 内部管理用データベース
    /// 参考：http://igrali.com/2015/05/01/using-sqlite-in-windows-10-universal-apps/
    /// </summary>
    public class LocalDatabase : SQLiteConnection
    {
        public LocalDatabase()
            : base(new SQLitePlatformWinRT(), EnvPath.GetDatabaseFilePath())
        {
            //DropTable<BookCategory>();
            //DropTable<BookItem>();
            CreateTable<BookCategory>();
            CreateTable<BookItem>();

            CreateTable<ScrapbookCategory>();
            CreateTable<ScrapbookItem>();
        }

        #region 本棚関連
        public BookCategory QueryBookCategory(string label)
        {
            return (from s in Table<BookCategory>()
                    where s.Label == label
                    select s).FirstOrDefault();
        }

        public IEnumerable<BookCategory> QueryBookCategoryAll()
        {
            return from shelf in Table<BookCategory>()
                   orderby shelf.Label
                   select shelf;
        }

        public bool InsertBookCategory(BookCategory category)
        {
            if (QueryBookCategory(category.Label) != null)
            {
                return false;
            }

            if (Insert(category) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteBookCategoryAll()
        {
            if (DeleteAll<BookCategory>() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 本関連
        public IEnumerable<BookItem> QueryBookItemList(BookCategory category)
        {
            return Table<BookItem>().Where(x => x.BookCategoryId == category.Id);
        }

        public BookItem QueryBookItemFromToken(int categoryId, string token)
        {
            return (from s in Table<BookItem>()
                    where (s.BookCategoryId == categoryId && s.Token == token)
                    select s).FirstOrDefault();
        }

        public BookItem QueryBookItemFromPath(int categoryId, string path)
        {
            return (from s in Table<BookItem>()
                    where (s.BookCategoryId == categoryId && s.Path == path)
                    select s).FirstOrDefault();
        }

        public bool InsertBookItem(BookItem bookItem)
        {
            if (QueryBookItemFromToken(bookItem.BookCategoryId, bookItem.Token) != null)
            {
                return false;
            }

            if (Insert(bookItem) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteBookItem(BookItem item)
        {
            if (Delete<BookItem>(item.Id) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region スクラップブック関連
        public ScrapbookCategory QueryScrapbookCategory(string name)
        {
            return (from s in Table<ScrapbookCategory>()
                    where s.FolderName == name
                    select s).FirstOrDefault();
        }

        public IEnumerable<ScrapbookCategory> QueryScrapbookCategoryAll()
        {
            return from shelf in Table<ScrapbookCategory>()
                   orderby shelf.FolderName
                   select shelf;
        }

        public bool InsertScrapbookCategory(ScrapbookCategory category)
        {
            if (QueryScrapbookCategory(category.FolderName) != null)
            {
                return false;
            }

            if (Insert(category) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteScrapbookCategoryAll()
        {
            if (DeleteAll<ScrapbookCategory>() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region スクラップブックアイテム関連
        public IEnumerable<ScrapbookItem> QueryScrapbookItemList(ScrapbookCategory category)
        {
            return Table<ScrapbookItem>().Where(x => x.ScrapbookCategoryId == category.Id);
        }

        public ScrapbookItem QueryScrapbookItem(int categoryId, string name)
        {
            return (from s in Table<ScrapbookItem>()
                    where (s.ScrapbookCategoryId == categoryId && s.FileName == name)
                    select s).FirstOrDefault();
        }

        public bool InsertScrapbookItem(ScrapbookItem item)
        {
            if (QueryScrapbookItem(item.ScrapbookCategoryId, item.FileName) != null)
            {
                return false;
            }

            if (Insert(item) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteScrapbookItem(ScrapbookItem item)
        {
            if (Delete<ScrapbookItem>(item.Id) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
