using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Data
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
                    bitmap.DecodePixelType = DecodePixelType.Logical;
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

        public static async Task<BitmapImage> CreateThumbnailBitmap(IStorageItem storage, uint size)
        {
            StorageItemThumbnail thumbnailFile = null;
            if (storage is StorageFolder)
            {
                thumbnailFile = await ((StorageFolder)storage).GetThumbnailAsync(ThumbnailMode.SingleItem, size);
            }
            else if (storage is StorageFile)
            {
                thumbnailFile = await ((StorageFile)storage).GetThumbnailAsync(ThumbnailMode.ListView, size);
            }

            if (thumbnailFile == null)
            {
                Debug.WriteLine("サムネイル画像取得失敗");
                return null;
            }

            BitmapImage bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(thumbnailFile);
            return bitmap;
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
                                bitmap.DecodePixelType = DecodePixelType.Logical;
                                bitmap.DecodePixelWidth = decodePixelWidth;
                            }
                            await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                        }
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// アーカイブファイルからサムネイル用画像データを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="thumWidth"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetFirstImageFromArchive(string path, int thumWidth)
        {
            BitmapImage bitmap = null;
            StorageFile zipFile = await StorageFile.GetFileFromPathAsync(path);
            using (IRandomAccessStream randomStream = await zipFile.OpenReadAsync())
            {
                using (Stream stream = randomStream.AsStreamForRead())
                {
                    using (ZipArchive zipArchive = new ZipArchive(stream))
                    {
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            if (FileKind.IsImageFile(entry.FullName))
                            {
                                bitmap = await BitmapUtils.CreateBitmap(entry, thumWidth);
                                break;
                            }
                        }
                    }
                }
            }

            return bitmap;
        }

        public static async Task<BitmapImage> CreateBitmapFromArchiveCover(string path, int thumWidth)
        {
            StorageFolder folder = await EnvPath.GetArchiveCoverFolder();
            string fileName = EnvPath.GetArchiveCoverFileName(path);

            IStorageItem imageFile = await folder.TryGetItemAsync(fileName);
            if (imageFile == null)
            {
                using (await m_asyncLock.LockAsync())
                {
                    StorageFile coverFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    StorageFile zipFile = await StorageFile.GetFileFromPathAsync(path);
                    using (IRandomAccessStream randomStream = await zipFile.OpenReadAsync())
                    {
                        using (Stream stream = randomStream.AsStreamForRead())
                        {
                            using (ZipArchive zipArchive = new ZipArchive(stream))
                            {
                                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                                {
                                    if (FileKind.IsImageFile(entry.FullName))
                                    {
                                        WriteableBitmap writeableBitmap = await CreateWriteableBitmap(entry);
                                        await SaveToJpegFile(writeableBitmap, coverFile, 1000);
                                        imageFile = await folder.GetFileAsync(fileName);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (imageFile != null)
            {
                return await CreateThumbnailBitmap(imageFile, (uint)thumWidth);
            }
            else
            {
                return null;
            }
        }

        public static async Task<WriteableBitmap> CreateWriteableBitmap(ZipArchiveEntry entry)
        {
            WriteableBitmap writableBitmap = null;
            using (Stream entryStream = entry.Open())
            {
                using (IInputStream inputStream = entryStream.AsInputStream())
                {
                    byte[] buffBytes = new byte[entry.Length];
                    await inputStream.ReadAsync(buffBytes.AsBuffer(), (uint)buffBytes.Length, InputStreamOptions.None);
                    using (MemoryStream memStream = new MemoryStream(buffBytes))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());

                        memStream.Seek(0, SeekOrigin.Begin);
                        writableBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
                        await writableBitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                    }
                }
            }

            return writableBitmap;
        }

        public static async Task SaveToJpegFile(WriteableBitmap writeableBitmap, StorageFile outputFile, uint size)
        {
            Guid encoderId = BitmapEncoder.JpegEncoderId;
            Stream stream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);
            
            using (IRandomAccessStream writeStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, writeStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)writeableBitmap.PixelWidth,
                    (uint)writeableBitmap.PixelHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                using (IOutputStream outputStream = writeStream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();
                }
            }
        }
    }
}
