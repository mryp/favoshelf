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

    public class BookItem
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
    /// 本棚データベース
    /// 参考：http://igrali.com/2015/05/01/using-sqlite-in-windows-10-universal-apps/
    /// </summary>
    public class BookshelfDatabase : SQLiteConnection
    {
        public BookshelfDatabase()
            : base(new SQLitePlatformWinRT(), EnvPath.GetDatabaseFilePath())
        {
            CreateTable<Bookshelf>();
            CreateTable<BookItem>();
        }

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

            int id = Insert(bookshelf);
            bookshelf.Id = id;
            return true;
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

        public IEnumerable<BookItem> SelectBookList(Bookshelf boolshelf)
        {
            return Table<BookItem>().Where(x => x.BookshelfId == boolshelf.Id);
        }

        public BookItem SelectBookItemFromToken(int bookshelfId, string token)
        {
            return (from s in Table<BookItem>()
                    where (s.BookshelfId == bookshelfId && s.Token == token)
                    select s).FirstOrDefault();
        }

        public BookItem SelectBookItemFromPath(int bookshelfId, string path)
        {
            return (from s in Table<BookItem>()
                    where (s.BookshelfId == bookshelfId && s.Path == path)
                    select s).FirstOrDefault();
        }

        public bool InsertBookItem(BookItem bookItem)
        {
            if (SelectBookItemFromToken(bookItem.BookshelfId, bookItem.Token) != null)
            {
                return false;
            }

            int id = Insert(bookItem);
            bookItem.Id = id;
            return true;
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

    }
}
