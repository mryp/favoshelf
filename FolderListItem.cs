using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
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
            OtherFile,
            Bookshelf,
        }

        private const int DESKTOP_THUM_IMAGE_WIDTH = 200;
        private const int DESKTOP_THUM_IMAGE_HEIGHT = 200;
        private const int NORMAL_TEXT_SIZE = 16;
        private const int SMALL_TEXT_SIZE = 12;
        #endregion

        #region フィールド
        private string m_name = "";
        private string m_path = "";
        private string m_token = "";
        private FileType m_type = FileType.OtherFile;
        private int m_thumWidth = 100;
        private int m_thumHeight = 100;
        private int m_textSize = 16;
        private BitmapImage m_prevImage;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderListItem()
        {
            m_textSize = NORMAL_TEXT_SIZE;
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
        /// ファイル名表示サイズ
        /// </summary>
        public int TextSize
        {
            get { return m_textSize; }
            set
            {
                if (value != m_textSize)
                {
                    m_textSize = value;
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
        /// サムネイル画像を取得する
        /// </summary>
        public async void UpdateThumImage()
        {
            if (this.Type == FileType.ImageFile)
            {
                StorageFile storage = await StorageFile.GetFileFromPathAsync(Path);
                PreviewImage = await BitmapUtils.CreateThumbnailBitmap(storage, (uint)ThumWidth);
            }
            else if (this.Type == FileType.Archive)
            {
                //PreviewImage = await BitmapUtils.GetFirstImageFromArchive(Path, ThumWidth);
                PreviewImage = await BitmapUtils.CreateBitmapFromArchiveCover(Path, ThumWidth);
            }
            else if (this.Type == FileType.Folder)
            {
                StorageFolder storage = await StorageFolder.GetFolderFromPathAsync(Path);
                PreviewImage = await BitmapUtils.CreateThumbnailBitmap(storage, (uint)ThumWidth);
            }
        }

        /// <summary>
        /// サムネイル画像を削除する
        /// </summary>
        public void ReleaseThumImage()
        {
            if (PreviewImage != null)
            {
                PreviewImage = null;
            }
        }
        
        public async static Task<Size> GetThumSizeFromWindow()
        {
            Rect size;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                size = Window.Current.Bounds;
            });

            int thumWidth = (int)(size.Width / 3.0) - (4 * 4);
            int thumHeight = thumWidth;
            if (thumWidth > DESKTOP_THUM_IMAGE_WIDTH)
            {
                thumWidth = DESKTOP_THUM_IMAGE_WIDTH;
                thumHeight = DESKTOP_THUM_IMAGE_HEIGHT;
            }

            return new Size(thumWidth, thumHeight);
        }

        public static FileType GetFileTypeFromStorage(StorageFile file)
        {
            FileType resultType = FileType.OtherFile;
            if (FileKind.IsImageFile(file.Path))
            {
                resultType = FolderListItem.FileType.ImageFile;
            }
            else if (FileKind.IsArchiveFile(file.Path))
            {
                resultType = FolderListItem.FileType.Archive;
            }

            return resultType;
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
