using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf
{
    /// <summary>
    /// フォルダ選択画面で表示する項目アイテム
    /// </summary>
    public class FolderListItem : INotifyPropertyChanged
    {
        #region 定義・定数
        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public enum FileType
        {
            Folder,
            Archive,
            ImageFile,
            OtherFile
        }
        #endregion

        #region フィールド
        private static AsyncLock m_asyncLock = new AsyncLock();
        private string m_name = "";
        private string m_path = "";
        private string m_token = "";
        private FileType m_type = FileType.OtherFile;
        private int m_thumWidth = 230;
        private int m_thumHeight = 250;
        private BitmapImage m_prevImage;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderListItem()
        {
        }

        /// <summary>
        /// 表示名
        /// </summary>
        public string Name
        {
            get { return m_name; }
            set
            {
                if (value != m_name)
                {
                    m_name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// フルパス（ある場合のみ
        /// </summary>
        public string Path
        {
            get { return m_path; }
            set
            {
                if (value != m_path)
                {
                    m_path = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 権限ありトークン（ある場合のみ）
        /// </summary>
        public string Token
        {
            get { return m_token; }
            set
            {
                if (value != m_token)
                {
                    m_token = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// ファイルタイプ
        /// </summary>
        public FileType Type
        {
            get { return m_type; }
            set
            {
                if (value != m_type)
                {
                    m_type = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// サムネイル画像幅
        /// </summary>
        public int ThumWidth
        {
            get { return m_thumWidth; }
            set
            {
                if (value != m_thumWidth)
                {
                    m_thumWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// サムネイル画像高さ
        /// </summary>
        public int ThumHeight
        {
            get { return m_thumHeight; }
            set
            {
                if (value != m_thumHeight)
                {
                    m_thumHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// サムネイル画像
        /// </summary>
        public BitmapImage PreviewImage
        {
            get { return m_prevImage; }
            set
            {
                if (value != m_prevImage)
                {
                    m_prevImage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// タイプによる背景色
        /// </summary>
        public string BackgroundColor
        {
            get { return convertFileTypeToBackColor(this.Type); }
        }

        private string convertFileTypeToBackColor(FileType type)
        {
            string color = "#111111";
            switch (type)
            {
                case FileType.Archive:
                case FileType.ImageFile:
                    color = "#0D47A1";
                    break;
                case FileType.Folder:
                    color = "#827717";
                    break;
                case FileType.OtherFile:
                default:
                    color = "#212121";
                    break;
            }
            return color;
        }

        public async void UpdateThumImage()
        {
            if (this.Type == FileType.ImageFile)
            {
                //PreviewImage = await createBitmapFromImageFile(Path);
            }
        }

        private async Task<BitmapImage> createBitmapFromImageFile(string filePath)
        {
            StorageFile storage = await StorageFile.GetFileFromPathAsync(filePath);
            BitmapImage bitmap = null;
            using (await m_asyncLock.LockAsync())
            {
                IRandomAccessStream stream = await storage.OpenReadAsync();
                bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream);
            }

            return bitmap;
        }

        #region INotifyPropertyChanged member

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
