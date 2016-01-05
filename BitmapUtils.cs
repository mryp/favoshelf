using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf
{
    /// <summary>
    /// Bitmap処理クラス
    /// </summary>
    public class BitmapUtils
    {
        private static AsyncLock m_asyncLock = new AsyncLock();
        
        /// <summary>
        /// Bitmap画像を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(string filePath)
        {
            return await CreateBitmap(filePath, 0);
        }

        /// <summary>
        /// Bitmap画像を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="decodePixelWidth"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(string filePath, int decodePixelWidth)
        {
            StorageFile storage = await StorageFile.GetFileFromPathAsync(filePath);
            return await CreateBitmap(storage, decodePixelWidth);
        }

        /// <summary>
        /// Bitmap画像を取得する
        /// </summary>
        /// <param name="storage"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(StorageFile storage)
        {
            return await CreateBitmap(storage, 0);
        }

        /// <summary>
        /// Bitmap画像を取得する
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="decodePixelWidth"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(StorageFile storage, int decodePixelWidth)
        {
            BitmapImage bitmap = null;
            using (await m_asyncLock.LockAsync())
            {
                IRandomAccessStream stream = await storage.OpenReadAsync();
                bitmap = new BitmapImage();
                if (decodePixelWidth > 0)
                {
                    bitmap.DecodePixelWidth = decodePixelWidth;
                }
                await bitmap.SetSourceAsync(stream);
            }

            return bitmap;
        }

        /// <summary>
        /// Bitmap画像を取得する
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(ZipArchiveEntry entry)
        {
            return await CreateBitmap(entry, 0);
        }

        /// <summary>
        /// Bitmap画像を取得する（Zipファイル用）
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="decodePixelWidth"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> CreateBitmap(ZipArchiveEntry entry, int decodePixelWidth)
        {
            BitmapImage bitmap = null;
            using (await m_asyncLock.LockAsync())
            {
                using (Stream entryStream = entry.Open())
                {
                    using (IInputStream inputStream = entryStream.AsInputStream())
                    {
                        byte[] buffBytes = new byte[entry.Length];
                        await inputStream.ReadAsync(buffBytes.AsBuffer(), (uint)buffBytes.Length, InputStreamOptions.None);
                        using (MemoryStream memStream = new MemoryStream(buffBytes))
                        {
                            bitmap = new BitmapImage();
                            if (decodePixelWidth > 0)
                            {
                                bitmap.DecodePixelWidth = decodePixelWidth;
                            }
                            await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                        }
                    }
                }
            }

            return bitmap;
        }

    }
}
