using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace favoshelf.Views
{
    public class ImageFlipViewModel : INotifyPropertyChanged, IDisposable
    {
        private IImageFileReader m_reader;
        private LocalDatabase m_db;
        private Bookmark m_bookmark;

        private int m_index = -1;
        private string m_title;
        private ObservableCollection<ImageFlipItem> m_itemList = new ObservableCollection<ImageFlipItem>();

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                if (value != m_title)
                {
                    m_title = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 画像アイテムリスト
        /// </summary>
        public ObservableCollection<ImageFlipItem> ItemList
        {
            get
            {
                return m_itemList;
            }
            set
            {
                if (value != m_itemList)
                {
                    m_itemList = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// 現在選択しているインデックス
        /// </summary>
        public int SelectedIndex
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

                    //前後2つを読み込み、その外側はクリアーする
                    updateImage(m_index);
                    updateImage(m_index + 1);
                    updateImage(m_index - 1);
                    clearImage(m_index + 2);
                    clearImage(m_index - 2);

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageFlipViewModel()
        {
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="db"></param>
        public void Init(IImageFileReader reader, LocalDatabase db)
        {
            m_reader = reader;
            m_db = db;

            this.Title = m_reader.ParentStorage.Name;
            initDataList();
            initBookmark();
        }

        /// <summary>
        /// データリストを初期化する
        /// </summary>
        private void initDataList()
        {
            this.ItemList.Clear();
            for (int i = 0; i < m_reader.Count; i++)
            {
                this.ItemList.Add(new ImageFlipItem());
            }
        }

        /// <summary>
        /// ブックマークを読み込む
        /// </summary>
        private void initBookmark()
        {
            m_bookmark = m_db.QueryBookmark(m_reader.ParentStorage.Path);
            if (m_bookmark == null)
            {
                m_bookmark = new Bookmark()
                {
                    Path = m_reader.ParentStorage.Path,
                    PageIndex = 0,
                    MaxPageCount = m_reader.Count,
                    Uptime = DateTime.Now,
                };
                m_db.InsertBookmark(m_bookmark);
            }

            if (m_bookmark.PageIndex < m_reader.Count)
            {
                this.SelectedIndex = m_bookmark.PageIndex;
            }
        }

        /// <summary>
        /// 次の画像を表示する
        /// </summary>
        public void SelectNext()
        {
            int index = this.SelectedIndex;
            if (!isItemListRange(index + 1))
            {
                return;
            }

            this.SelectedIndex = index + 1;
        }

        /// <summary>
        /// 前の画像を表示する
        /// </summary>
        public void SelectPrev()
        {
            int index = this.SelectedIndex;
            if (!isItemListRange(index - 1))
            {
                return;
            }

            this.SelectedIndex = index - 1;
        }

        /// <summary>
        /// 画像を読み込みリストにセットする
        /// </summary>
        /// <param name="index"></param>
        private void updateImage(int index)
        {
            if (!isItemListRange(index))
            {
                return;
            }

            this.ItemList[index].SetImage(m_reader, index);
        }
        
        /// <summary>
        /// 画像をクリアーする
        /// </summary>
        /// <param name="index"></param>
        private void clearImage(int index)
        {
            if (!isItemListRange(index))
            {
                return;
            }

            this.ItemList[index].ClearImage(index);
        }

        /// <summary>
        /// 指定した位置のデータがリスト範囲に入っているかどうか
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool isItemListRange(int index)
        {
            if (index < 0 || index >= this.ItemList.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Dispose()
        {
            m_bookmark.PageIndex = this.SelectedIndex;
            m_db.InsertBookmark(m_bookmark);

            IDisposable disReader = m_reader as IDisposable;
            if (disReader != null)
            {
                disReader.Dispose();
            }
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
