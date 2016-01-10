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
            return String.Format("Id={0}, BookshelfId={1}, Token={2}, Path={3}, Uptime={4}"
                , Id, BookCategoryId, Token, Path, Uptime.ToString("yyyy/MM/dd HH:mm:ss"));
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

        public bool InsertBookCategory(BookCategory bookshelf)
        {
            if (QueryBookCategory(bookshelf.Label) != null)
            {
                return false;
            }

            if (Insert(bookshelf) > 0)
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
        public IEnumerable<BookItem> QueryBookItemList(BookCategory boolshelf)
        {
            return Table<BookItem>().Where(x => x.BookCategoryId == boolshelf.Id);
        }

        public BookItem QueryBookItemFromToken(int bookshelfId, string token)
        {
            return (from s in Table<BookItem>()
                    where (s.BookCategoryId == bookshelfId && s.Token == token)
                    select s).FirstOrDefault();
        }

        public BookItem QueryBookItemFromPath(int bookshelfId, string path)
        {
            return (from s in Table<BookItem>()
                    where (s.BookCategoryId == bookshelfId && s.Path == path)
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
    }
}
