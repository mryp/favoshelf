using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;

namespace favoshelf.Views
{
    /// <summary>
    /// フォルダ一覧画面のビューモデル
    /// </summary>
    public class FolderSelectViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// フォルダ・ファイルリスト
        /// </summary>
        private ObservableCollection<FolderListItem> m_itemList = new ObservableCollection<FolderListItem>();

        /// <summary>
        /// タイトル
        /// </summary>
        private string m_titleName = "";

        /// <summary>
        /// フォルダ・ファイルリスト
        /// </summary>
        public ObservableCollection<FolderListItem> ItemList
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
        /// タイトル
        /// </summary>
        public string TitleName
        {
            get
            {
                return m_titleName;
            }
            set
            {
                if (value != m_titleName)
                {
                    m_titleName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderSelectViewModel()
        {
        }

        /// <summary>
        /// リストの初期化を行う
        /// </summary>
        /// <param name="naviParam"></param>
        public async void Init(INavigateParameter naviParam)
        {
            this.TitleName = naviParam.GetFolderName();
            IReadOnlyList<FolderListItem> itemList = await naviParam.GetItemList();
            this.ItemList.Clear();
            foreach (FolderListItem item in itemList)
            {
                this.ItemList.Add(item);
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
