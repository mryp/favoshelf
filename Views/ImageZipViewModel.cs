using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// ZIPファイルを表示するビューモデル
    /// </summary>
    public class ImageZipViewModel : ImageViewModelBase
    {
        private ZipArchive m_zipArchive;
        private static AsyncLock m_asyncLock = new AsyncLock();

        /// <summary>
        /// フィールドを初期化する
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override async Task initField(ImageNavigateParameter param)
        {
            if (m_zipArchive != null)
            {
                m_zipArchive.Dispose();
            }
            this.DataList.Clear();

            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(param.Path);
            StorageHistoryManager.AddStorage(zipFile, StorageHistoryManager.DataType.Latest);
            this.ImageStorage = zipFile;

            IRandomAccessStream randomStream = await zipFile.OpenReadAsync();
            Stream stream = randomStream.AsStreamForRead();
            m_zipArchive = new ZipArchive(stream);
            foreach (ZipArchiveEntry entry in m_zipArchive.Entries)
            {
                if (FileKind.IsImageFile(entry.FullName))
                {
                    this.DataList.Add(entry);
                }
            }

            this.CommandTitle = Path.GetFileNameWithoutExtension(param.Path);
            this.Index = 0;
        }

        /// <summary>
        /// Bitmapを生成する
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override async Task<BitmapImage> createBitmap(object data)
        {
            ZipArchiveEntry entry = data as ZipArchiveEntry;
            if (entry == null)
            {
                Debug.WriteLine("対象外オブジェクト data=" + data.ToString());
                return null;
            }
            
            BitmapImage bitmap = await BitmapUtils.CreateBitmap(entry);
            return bitmap;
        }

        /// <summary>
        /// 現在開いている画像を指定した場所にコピーする
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override async Task<StorageFile> CopyFileAsync(StorageFolder folder, string fileName)
        {
            ZipArchiveEntry entry = this.DataList[this.Index] as ZipArchiveEntry;
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
        /// オブジェクトの解放
        /// </summary>
        public override void Dispose()
        {
            if (m_zipArchive != null)
            {
                m_zipArchive.Dispose();
            }
        }
    }
}
