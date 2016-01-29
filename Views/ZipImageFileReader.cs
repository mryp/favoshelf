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

        public int Count
        {
            get
            {
                return m_dataList.Count;
            }
        }

        public IStorageItem ParentStorage
        {
            get;
            private set;
        }

        public ZipImageFileReader(ImageNavigateParameter param)
        {
            m_imageParam = param;
        }

        public async Task LoadData()
        {
            this.Dispose();

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
        
        private ZipArchiveEntry getEntryFromIndex(int index)
        {
            if (m_dataList.Count <= index)
            {
                return null;
            }
            return m_dataList[index];
        }

        public void Dispose()
        {
            if (m_zipArchive != null)
            {
                m_zipArchive.Dispose();
                m_zipArchive = null;
            }
            if (m_dataList != null)
            {
                m_dataList.Clear();
                m_dataList = null;
            }
        }
    }
}
