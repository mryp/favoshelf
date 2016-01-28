using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace favoshelf.Data
{
    /// <summary>
    /// ファイル・フォルダ利用履歴管理クラス
    /// </summary>
    public class StorageHistoryManager
    {
        #region 定数/定義
        /// <summary>
        /// 履歴データ種別
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// フォルダ一覧
            /// </summary>
            Folder,

            /// <summary>
            /// 本棚
            /// </summary>
            Bookshelf,

            /// <summary>
            /// クイックアクセス
            /// </summary>
            Latest,
        }
        #endregion

        #region メソッド
        public static string AddStorage(IStorageItem item, DataType type)
        {
            string token = "";
            if (type == DataType.Latest)
            {
                if (!EnvPath.IsLocalFolder(item.Path))
                {
                    token = StorageApplicationPermissions.MostRecentlyUsedList.Add(item, DateTime.Now.ToString("yyyyMMddHHmmss"));
                }
            }
            else
            {
                token = StorageApplicationPermissions.FutureAccessList.Add(item, type.ToString());
            }

            return token;
        }

        public static void RemoveStorage(string token, DataType type)
        {
            if (type == DataType.Latest)
            {
                if (StorageApplicationPermissions.MostRecentlyUsedList.ContainsItem(token))
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(token);
                }
            }
            else
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    StorageApplicationPermissions.FutureAccessList.Remove(token);
                }
            }
        }

        public static List<string> GetTokenList(DataType type)
        {
            AccessListEntryView entries = getEntryFromType(type);
            List<string> resultList;
            if (type == DataType.Latest)
            {
                resultList = entries.OrderByDescending(entry => entry.Metadata)
                    .Select(entry => entry.Token)
                    .ToList();
            }
            else
            {
                resultList = entries.Where(entry => entry.Metadata == type.ToString())
                    .Select(entry => entry.Token)
                    .ToList();
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
            if (type == DataType.Latest)
            {
                foreach (AccessListEntry entry in entries)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Remove(entry.Token);
                }
            }
            else
            {
                AccessListEntry[] filterList = entries.Where(entry => entry.Metadata == type.ToString()).ToArray();
                foreach (AccessListEntry entry in filterList)
                {
                    StorageApplicationPermissions.FutureAccessList.Remove(entry.Token);
                }
            }
        }
        #endregion
    }
}
