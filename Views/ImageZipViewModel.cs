using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public class ImageZipViewModel : ImageViewModelBase<ZipArchiveEntry>
    {
        private ZipArchive m_zipArchive;

        /// <summary>
        /// Bitmapを生成する
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected override async Task<BitmapImage> createBitmap(ZipArchiveEntry entry)
        {
            var bitmap = new BitmapImage();
            using (Stream entryStream = entry.Open())
            {
                using (IInputStream inputStream = entryStream.AsInputStream())
                {
                    byte[] buffBytes = new byte[entry.Length];
                    await inputStream.ReadAsync(buffBytes.AsBuffer(), (uint)buffBytes.Length, InputStreamOptions.None);
                    using (MemoryStream memStream = new MemoryStream(buffBytes))
                    {
                        await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// フィールドを初期化する
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override async Task initField(FolderListItem item)
        {
            if (m_zipArchive != null)
            {
                m_zipArchive.Dispose();
            }
            this.DataList.Clear();

            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(item.Path);
            IRandomAccessStream randomStream = await zipFile.OpenReadAsync();
            Stream stream = randomStream.AsStreamForRead();
            m_zipArchive = new ZipArchive(stream);
            foreach (ZipArchiveEntry entry in m_zipArchive.Entries)
            {
                this.DataList.Add(entry);
            }

            this.Index = 0;
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
