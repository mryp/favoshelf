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
    /// 本棚テーブル
    /// </summary>
    public class Bookshelf
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
    public class BookshelfItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int BookshelfId { get; set; }
        public string Token { get; set; }
        public string Path { get; set; }
        public DateTime Uptime { get; set; }

        public override string ToString()
        {
            return String.Format("Id={0}, BookshelfId={1}, Token={2}, Path={3}, Uptime={4}"
                , Id, BookshelfId, Token, Path, Uptime.ToString("yyyy/MM/dd HH:mm:ss"));
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
            //DropTable<Bookshelf>();
            //DropTable<BookshelfItem>();
            CreateTable<Bookshelf>();
            CreateTable<BookshelfItem>();
        }

        #region 本棚関連
        public Bookshelf SelectBookshelf(string label)
        {
            return (from s in Table<Bookshelf>()
                    where s.Label == label
                    select s).FirstOrDefault();
        }

        public IEnumerable<Bookshelf> SelectBookshelfAll()
        {
            return from shelf in Table<Bookshelf>()
                   orderby shelf.Label
                   select shelf;
        }

        public bool InsertBoolshelf(Bookshelf bookshelf)
        {
            if (SelectBookshelf(bookshelf.Label) != null)
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

        public bool DeleteBookshelfAll()
        {
            if (DeleteAll<Bookshelf>() > 0)
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
        public IEnumerable<BookshelfItem> SelectBookList(Bookshelf boolshelf)
        {
            return Table<BookshelfItem>().Where(x => x.BookshelfId == boolshelf.Id);
        }

        public BookshelfItem SelectBookItemFromToken(int bookshelfId, string token)
        {
            return (from s in Table<BookshelfItem>()
                    where (s.BookshelfId == bookshelfId && s.Token == token)
                    select s).FirstOrDefault();
        }

        public BookshelfItem SelectBookItemFromPath(int bookshelfId, string path)
        {
            return (from s in Table<BookshelfItem>()
                    where (s.BookshelfId == bookshelfId && s.Path == path)
                    select s).FirstOrDefault();
        }

        public bool InsertBookItem(BookshelfItem bookItem)
        {
            if (SelectBookItemFromToken(bookItem.BookshelfId, bookItem.Token) != null)
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

        public bool DeleteBookItem(BookshelfItem item)
        {
            if (Delete<BookshelfItem>(item.Id) > 0)
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
