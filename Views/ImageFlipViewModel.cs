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
    public class ImageFlipViewModel : INotifyPropertyChanged
    {
        private IImageFileReader m_reader;
        private LocalDatabase m_db;
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
                    updateImage(m_index);
                    updateImage(m_index + 1);
                    updateImage(m_index - 1);
                    clearImage(m_index + 2);
                    clearImage(m_index - 2);
                    OnPropertyChanged();
                }
            }
        }

        public ImageFlipViewModel()
        {
        }

        public void Init(IImageFileReader reader, LocalDatabase db)
        {
            m_reader = reader;
            m_db = db;
            this.Title = m_reader.ParentStorage.Name;
            for (int i=0; i<m_reader.Count; i++)
            {
                this.ItemList.Add(new ImageFlipItem());
            }
        }

        private void updateImage(int index)
        {
            if (!isItemListRange(index))
            {
                return;
            }

            this.ItemList[index].SetImage(m_reader, index);
        }
        
        private void clearImage(int index)
        {
            if (!isItemListRange(index))
            {
                return;
            }

            this.ItemList[index].ClearImage(index);
        }

        private bool isItemListRange(int index)
        {
            if (index < 0 || index >= this.ItemList.Count)
            {
                return false;
            }

            return true;
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
