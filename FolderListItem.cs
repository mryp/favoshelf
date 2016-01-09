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
