using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace favoshelf
{
    /// <summary>
    /// ファイル・フォルダ利用履歴管理クラス
    /// </summary>
    public class StorageHistoryManager
    {
        #region 定数/定義
        public enum DataType
        {
            Folder,
            Bookshelf,
            Latest,
        }
        #endregion

        #region メソッド
        public static void AddStorage(IStorageItem item, DataType type)
        {
            if (type == DataType.Latest)
            {
                StorageApplicationPermissions.MostRecentlyUsedList.Add(item, type.ToString());
            }
            else
            {
                StorageApplicationPermissions.FutureAccessList.Add(item, type.ToString());
            }
        }

        public static void RemoveStorage(string token)
        {
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
            {
                StorageApplicationPermissions.FutureAccessList.Remove(token);
            }
        }

        public static List<string> GetTokenList(DataType type)
        {
            AccessListEntryView entries = getEntryFromType(type);
            List<string> resultList = new List<string>();

            foreach (AccessListEntry entry in entries)
            {
                if (entry.Metadata == type.ToString())
                {
                    resultList.Add(entry.Token);
                }
            }

            return resultList;
        }

        private static AccessListEntryView getEntryFromType(DataType type)
        {
            AccessListEntryView entries;
            if (type == DataType.Latest)
            {
                entries = StorageApplicationPermissions.MostRecentlyUsedList.Entries;
            }
            else
            {
                entries = StorageApplicationPermissions.FutureAccessList.Entries;
            }

            return entries;
        }

        public static void RemoveAll(DataType type)
        {
            AccessListEntryView entries = getEntryFromType(type);
            foreach (AccessListEntry entry in entries)
            {
                if (entry.Metadata == type.ToString())
                {
                    if (type == DataType.Latest)
                    {
                        StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.Token);
                    }
                    else
                    {
                        StorageApplicationPermissions.FutureAccessList.Remove(entry.Token);
                    }
                }
            }
        }
        #endregion
    }
}
