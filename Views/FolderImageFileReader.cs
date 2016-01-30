using favoshelf.Data;
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
        /// コンストラクタ
        /// </summary>
        /// <param name="param"></param>
        public FolderImageFileReader(ImageNavigateParameter param)
        {
            m_imageParam = param;
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
