using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像表示ページのデータビューの共通部分
    /// </summary>
    /// <typeparam name="T">データリスト（画像読み込み元情報）の型</typeparam>
    public abstract class ImageViewModelBase<T> : INotifyPropertyChanged, IImageAccess, IDisposable
    {
        private FolderListItem m_selectFileItem = null;

        /// <summary>
        /// 現在表示している画像の位置
        /// </summary>
        private int m_index = 0;

        /// <summary>
        /// データリスト
        /// </summary>
        private ObservableCollection<T> m_dataList = new ObservableCollection<T>();

        public FolderListItem SelectFileItem
        {
            get
            {
                return m_selectFileItem;
            }
            set
            {
                if (value != m_selectFileItem)
                {
                    m_selectFileItem = value;
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
        public ObservableCollection<T> DataList
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
        public async Task Init(FolderListItem item)
        {
            this.SelectFileItem = item;
            await initField(item);
        }

        /// <summary>
        /// 指定したファイル情報からフィールド（リストなど）の初期化を行う
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract Task initField(FolderListItem item);

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
            return bitmap;
        }

        /// <summary>
        /// 指定したリスたアイテムからビットマップデータを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task<BitmapImage> createBitmap(T data);

        /// <summary>
        /// 現在位置の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<BitmapImage> GetImage()
        {
            return await GetImage(m_index);
        }

        /// <summary>
        /// 次の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<BitmapImage> GetNextImage()
        {
            if (m_index < m_dataList.Count - 1)
            {
                m_index++;
            }
            return await GetImage(m_index);
        }

        /// <summary>
        /// 前の画像を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<BitmapImage> GetPrevImage()
        {
            if (m_index > 0)
            {
                m_index--;
            }
            return await GetImage(m_index);
        }

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
