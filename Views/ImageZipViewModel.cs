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

            this.CommandTitle = item.Name;
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
            BitmapImage bitmap = null;
            using (await m_asyncLock.LockAsync())
            {
                Debug.WriteLine("ImageZipViewModel#createBitmap call name=" + entry.Name);
                using (Stream entryStream = entry.Open())
                {
                    using (IInputStream inputStream = entryStream.AsInputStream())
                    {
                        byte[] buffBytes = new byte[entry.Length];
                        await inputStream.ReadAsync(buffBytes.AsBuffer(), (uint)buffBytes.Length, InputStreamOptions.None);
                        using (MemoryStream memStream = new MemoryStream(buffBytes))
                        {
                            bitmap = new BitmapImage();
                            await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                        }
                    }
                }

                Debug.WriteLine("ImageZipViewModel#createBitmap finish");
            }

            return bitmap;
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
