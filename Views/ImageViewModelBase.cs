using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像表示ページのデータビューの共通部分
    /// </summary>
    /// <typeparam name="T">データリスト（画像読み込み元情報）の型</typeparam>
    public abstract class ImageViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region 定数
        private const string NOT_FOUND_IMAGE_URI = "ms-appx:///Assets/NotFoundImage.png";
        #endregion

        #region フィールド
        private ImageNavigateParameter m_viewParam = null;
        private int m_index = 0;
        private ObservableCollection<object> m_dataList = new ObservableCollection<object>();
        private BitmapImage m_indexImage;
        private string m_comanndTitle;
        private IStorageItem m_storage = null;
        private bool m_isBusy = false;
        #endregion

        #region プロパティ
        /// <summary>
        /// 画像表示時に選択したファイル情報
        /// </summary>
        public ImageNavigateParameter ViewParam
        {
            get
            {
                return m_viewParam;
            }
            set
            {
                if (value != m_viewParam)
                {
                    m_viewParam = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 初期化時に読み込んだファイルまたはフォルダ情報
        /// </summary>
        public IStorageItem ImageStorage
        {
            get
            {
                return m_storage;
            }
            set
            {
                if (value != m_storage)
                {
                    m_storage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 現在位置インデックス
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                if (value != m_index)
                {
                    m_index = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// データリスト（画像読み込み元情報保持）
        /// </summary>
        public ObservableCollection<object> DataList
        {
            get
            {
                return m_dataList;
            }
            set
            {
                if (value != m_dataList)
                {
                    m_dataList = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 表示する画像データ
        /// </summary>
        public BitmapImage IndexImage
        {
            get
            {
                return m_indexImage;
            }
            set
            {
                if (value != m_indexImage)
                {
                    m_indexImage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// コマンドバータイトル
        /// </summary>
        public string CommandTitle
        {
            get
            {
                return m_comanndTitle;
            }
            set
            {
                if (value != m_comanndTitle)
                {
                    m_comanndTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 画像読み込み処理中かどうか
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return m_isBusy;
            }
            set
            {
                if (value != m_isBusy)
                {
                    m_isBusy = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageViewModelBase()
        {
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 指定ファイルにある画像一覧をリストにセットする
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task Init(ImageNavigateParameter item)
        {
            this.ViewParam = item;
            await initField(item);
        }

        /// <summary>
        /// 指定したファイル情報からフィールド（リストなど）の初期化を行う
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Task initField(ImageNavigateParameter item);

        /// <summary>
        /// 指定した位置の画像を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<BitmapImage> GetImage(int index)
        {
            if (m_dataList == null)
            {
                return null;
            }
            if (index < 0 || index >= m_dataList.Count)
            {
                return null;
            }

            BitmapImage bitmap = await createBitmap(m_dataList[index]);
            if (bitmap == null)
            {
                Debug.WriteLine("Bitmap null index=" + index);
                bitmap = new BitmapImage(new Uri(NOT_FOUND_IMAGE_URI));
            }
            return bitmap;
        }

        /// <summary>
        /// 指定したリスたアイテムからビットマップデータを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task<BitmapImage> createBitmap(object data);

        /// <summary>
        /// 現在位置の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async void GetImage()
        {
            this.IsBusy = true;
            this.IndexImage = await GetImage(this.Index);
            this.IsBusy = false;
        }

        /// <summary>
        /// 次の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async void GetNextImage()
        {
            if (this.Index >= m_dataList.Count - 1)
            {
                return;
            }
            if (this.IsBusy)
            {
                return;
            }
            this.IsBusy = true;
            this.Index++;
            this.IndexImage = await GetImage(this.Index);
            this.IsBusy = false;
        }

        /// <summary>
        /// 前の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async void GetPrevImage()
        {
            if (this.Index == 0)
            {
                return;
            }
            if (this.IsBusy)
            {
                return;
            }
            this.IsBusy = true;
            this.Index--;
            this.IndexImage = await GetImage(this.Index);
            this.IsBusy = false;
        }

        /// <summary>
        /// 指定したフォルダに現在のファイルをコピーする
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract Task<StorageFile> CopyFileAsync(StorageFolder folder, string fileName);
        #region INotifyPropertyChanged member

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
