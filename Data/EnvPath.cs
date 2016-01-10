using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace favoshelf.Data
{
    /// <summary>
    /// 環境パス情報取得クラス
    /// </summary>
    public class EnvPath
    {
        private const string FOLDER_ARCHIVE_COVER = "arccover";
        private const string DB_FILE_NAME = "bookshelf.db";
        private const string FOLDER_SCRAP_ROOT = "scrapbook";

        public static StorageFolder GetLocalFolder()
        {
            return ApplicationData.Current.LocalFolder;
        }

        public static async Task<StorageFolder> GetArchiveCoverFolder()
        {
            StorageFolder local = GetLocalFolder();
            try
            {
                return await local.CreateFolderAsync(FOLDER_ARCHIVE_COVER, CreationCollisionOption.OpenIfExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetArchiveCoverFolder フォルダ取得失敗 e=" + e.ToString());
                return null;
            }
        }

        public static string GetArchiveCoverFileName(string zipFilePath)
        {
            string hash = StringUtils.GetHashString(zipFilePath);
            return hash + ".jpg";
        }

        public static string GetDatabaseFilePath()
        {
            StorageFolder local = GetLocalFolder();
            return Path.Combine(local.Path, DB_FILE_NAME);
        }

        public async static Task<StorageFolder> GetScrapbookRootFolder()
        {
            StorageFolder local = GetLocalFolder();
            try
            {
                return await local.CreateFolderAsync(FOLDER_SCRAP_ROOT, CreationCollisionOption.OpenIfExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetScrapbookRootFolder フォルダ取得失敗 e=" + e.ToString());
                return null;
            }
        }

        public async static Task<StorageFolder> GetScrapbookSubFolder(string folderName)
        {
            StorageFolder baseFolder = await GetScrapbookRootFolder();
            try
            {
                return await baseFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetScrapbookSubFolder フォルダ取得失敗 e=" + e.ToString());
                return null;
            }
        }
        
        /// <summary>
        /// スクラップファイル用ファイル名（拡張子なし）
        /// </summary>
        /// <returns></returns>
        public static string CreateScrapbookFileName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + StringUtils.GeneratePassword(4);
        }
    }
}
