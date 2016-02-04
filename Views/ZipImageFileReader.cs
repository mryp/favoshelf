using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using favoshelf.Data;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.IO.Compression;
using favoshelf.Util;
using Windows.Storage.Streams;
using System.IO;

namespace favoshelf.Views
{
    /// <summary>
    /// ZIPファイルを読み込む
    /// </summary>
    public class ZipImageFileReader : IImageFileReader, IDisposable
    {
        private ImageNavigateParameter m_imageParam;
        private ZipArchive m_zipArchive;
        private List<ZipArchiveEntry> m_dataList = new List<ZipArchiveEntry>();

        /// <summary>
        /// 読み込み可能なリスト個数
        /// </summary>
        public int Count
        {
            get
            {
                return m_dataList.Count;
            }
        }

        /// <summary>
        /// ZIPファイルのストレージオブジェクト
        /// </summary>
        public IStorageItem ParentStorage
        {
            get;
            private set;
        }

        /// <summary>
        /// 最初の表示位置
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
        public ZipImageFileReader(ImageNavigateParameter param)
        {
            m_imageParam = param;
            this.FirstIndex = -1;
        }

        /// <summary>
        /// ZIPデータを読み込みファイル一覧を作成する
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataAsync()
        {
            this.Dispose();
            this.m_dataList.Clear();

            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(m_imageParam.Path);
            StorageHistoryManager.AddStorage(zipFile, StorageHistoryManager.DataType.Latest);
            this.ParentStorage = zipFile;

            IRandomAccessStream randomStream = await zipFile.OpenReadAsync();
            Stream stream = randomStream.AsStreamForRead();
            m_zipArchive = new ZipArchive(stream);
            foreach (ZipArchiveEntry entry in m_zipArchive.Entries)
            {
                if (FileKind.IsImageFile(entry.FullName))
                {
                    m_dataList.Add(entry);
                }
            }            
        }

        /// <summary>
        /// 指定した位置のBitmapを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<BitmapImage> CreateBitmapAsync(int index)
        {
            ZipArchiveEntry entry = getEntryFromIndex(index);
            if (entry == null)
            {
                return null;
            }

            BitmapImage bitmap = await BitmapUtils.CreateBitmap(entry);
            return bitmap;
        }
        
        /// <summary>
        /// 視程した位置のファイルをファイルコピーする
        /// </summary>
        /// <param name="index"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<StorageFile> CopyFileAsync(int index, StorageFolder folder, string fileName)
        {
            ZipArchiveEntry entry = getEntryFromIndex(index);
            if (entry == null)
            {
                return null;
            }

            string ext = Path.GetExtension(entry.FullName);
            StorageFile saveFile = await folder.CreateFileAsync(fileName + ext, CreationCollisionOption.ReplaceExisting);
            StorageFile outputFile = await BitmapUtils.SaveToFileFromZipEntry(entry, saveFile);
            if (outputFile == null)
            {
                return null;
            }

            return saveFile;
        }
        
        /// <summary>
        /// 指定し位置のオブジェクトを取得する
        /// 存在しない場合はnullを返す
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ZipArchiveEntry getEntryFromIndex(int index)
        {
            if (m_dataList.Count <= index)
            {
                return null;
            }
            return m_dataList[index];
        }

        /// <summary>
        /// オブジェク開放
        /// </summary>
        public void Dispose()
        {
            if (m_zipArchive != null)
            {
                m_zipArchive.Dispose();
                m_zipArchive = null;
            }
        }
    }
}
