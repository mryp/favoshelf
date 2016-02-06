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
    public class JumpPageViewModel : INotifyPropertyChanged
    {
        private int m_index = -1;
        private ObservableCollection<ImageFlipItem> m_itemList = new ObservableCollection<ImageFlipItem>();

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
                    OnPropertyChanged();
                }
            }
        }

        internal void JumpPageFist()
        {
            this.SelectedIndex = 0;
        }

        internal void JumpPageLast()
        {
            //TODO: まだ未実装
            this.SelectedIndex = 0;
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
