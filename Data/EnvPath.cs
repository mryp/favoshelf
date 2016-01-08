﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                Debug.WriteLine("error e="+e.ToString());
                return null;
            }
        }

        public static string GetArchiveCoverFileName(string zipFilePath)
        {
            string hash = StringUtils.GetHashString(zipFilePath);
            return hash + ".jpg";
        }
    }
}
