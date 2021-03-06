﻿using favoshelf.Data;
using favoshelf.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// フォルダ内画像表示用リーダークラス
    /// </summary>
    public class FolderImageFileReader : IImageFileReader, IDisposable
    {
        private ImageNavigateParameter m_imageParam;
        private List<StorageFile> m_dataList = new List<StorageFile>();

        /// <summary>
        /// ファイル個数
        /// </summary>
        public int Count
        {
            get
            {
                return m_dataList.Count;
            }
        }

        /// <summary>
        /// 画像フォルダのストレージオブジェクト
        /// </summary>
        public IStorageItem ParentStorage
        {
            get;
            private set;
        }

        /// <summary>
        /// ファイル選択時の初期表示位置
        /// フォルダ選択時は常に-1
        /// </summary>
        public int FirstIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="param"></param>
        public FolderImageFileReader(ImageNavigateParameter param)
        {
            m_imageParam = param;
            this.FirstIndex = -1;
        }

        /// <summary>
        /// ファイル一覧を作成する
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync()
        {
            StorageFolder folder;
            if (m_imageParam.Type == ImageNavigateParameter.DataType.ImageFile)
            {
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(m_imageParam.Path);
                folder = await imageFile.GetParentAsync();
            }
            else
            {
                folder = await StorageFolder.GetFolderFromPathAsync(m_imageParam.Path);
            }
            StorageHistoryManager.AddStorage(folder, StorageHistoryManager.DataType.Latest);
            this.ParentStorage = folder;

            IReadOnlyList<StorageFile> fileList = await getImageFilesAsync(folder);
            m_dataList.Clear();
            foreach (StorageFile file in fileList)
            {
                m_dataList.Add(file);
            }

            //ファイル名昇順で並び替え
            sortList(m_dataList);

            //遷移時の選択ファイルを取得
            for (int i=0; i< m_dataList.Count; i++)
            {
                if (m_dataList[i].Path == m_imageParam.Path)
                {
                    this.FirstIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// ファイル名でソートを行う
        /// </summary>
        /// <param name="list"></param>
        private void sortList(List<StorageFile> list)
        {
            list.Sort((a, b) =>
            {
                string aName = Path.GetFileNameWithoutExtension(a.Name);
                string bName = Path.GetFileNameWithoutExtension(b.Name);
                int aValue = 0;
                int bValue = 0;
                if (int.TryParse(aName, out aValue) && int.TryParse(bName, out bValue))
                {
                    //ファイル名が数値の時は0詰めで行う
                    return string.Format("{0:D6}", aValue).CompareTo(string.Format("{0:D6}", bValue));
                }
                else
                {
                    return a.Name.CompareTo(b.Name);
                }
            });
        }

        /// <summary>
        /// 指定フォルダ以下にある画像ファイルのみを抽出して返す
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<StorageFile>> getImageFilesAsync(StorageFolder folder)
        {
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, FileKind.GetImageFilterList());
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(queryOptions);
            return await queryResult.GetFilesAsync();
        }

        /// <summary>
        /// 指定した位置のBitmapを作成して返す
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<BitmapImage> CreateBitmapAsync(int index)
        {
            StorageFile storage = getFileFromIndex(index);
            if (storage == null)
            {
                Debug.WriteLine("対象外のオブジェクト index=" + index);
                return null;
            }

            BitmapImage bitmap = await BitmapUtils.CreateBitmap(storage);
            return bitmap;
        }
        
        /// <summary>
        /// 指定した位置のファイル情報を取得する
        /// 存在しない場合はnullを返す
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private StorageFile getFileFromIndex(int index)
        {
            if (m_dataList.Count <= index)
            {
                return null;
            }
            return m_dataList[index];
        }

        /// <summary>
        /// 指定した位置のファイルをコピーする
        /// </summary>
        /// <param name="index"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<StorageFile> CopyFileAsync(int index, StorageFolder folder, string fileName)
        {
            StorageFile imageFile = getFileFromIndex(index);
            if (imageFile == null)
            {
                return null;
            }
            string ext = Path.GetExtension(imageFile.Path);

            StorageFile copyFile = await imageFile.CopyAsync(folder, fileName + ext, NameCollisionOption.ReplaceExisting);
            return copyFile;
        }

        /// <summary>
        /// オブジェクト開放
        /// </summary>
        public void Dispose()
        {
        }
    }
}
