using favoshelf.Data;
using System;
using System.Collections.Generic;
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
        private int m_index = 0;
        private string m_title;

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

        public ImageFlipViewModel()
        {

        }

        public void Init(IImageFileReader reader, LocalDatabase db)
        {
            m_reader = reader;
            m_db = db;
            this.Title = m_reader.ParentStorage.Name;
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
