using favoshelf.Util;
using favoshelf.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Data
{
    public class ImageFlipItem : INotifyPropertyChanged
    {
        private static AsyncLock m_asyncLock = new AsyncLock();
        private bool m_isLoading = true;
        public BitmapImage m_image;
        
        public ImageFlipItem()
        {
        }

        /// <summary>
        /// 画像データ
        /// </summary>
        public BitmapImage ImageData
        {
            get
            {
                return m_image;
            }
            set
            {
                if (value != m_image)
                {
                    m_image = value;
                    if (m_image == null)
                    {
                        IsLoading = true;
                    }
                    else
                    {
                        IsLoading = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 読込中かどうか
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return m_isLoading;
            }
            set
            {
                if (value != m_isLoading)
                {
                    m_isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 画像読み込みImageDataにセットする
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        public async void SetImage(IImageFileReader reader, int index)
        {
            using (await m_asyncLock.LockAsync())
            {
                if (this.ImageData != null)
                {
                    return;
                }
                Debug.WriteLine("SetImage index=" + index.ToString());
                this.ImageData = await reader.CreateBitmapAsync(index);
            }
        }
        
        /// <summary>
        /// 画像をクリアーする
        /// </summary>
        /// <param name="index"></param>
        public async void ClearImage(int index)
        {
            using (await m_asyncLock.LockAsync())
            {
                if (this.ImageData == null)
                {
                    return;
                }

                Debug.WriteLine("ClearImage index=" + index.ToString());
                this.ImageData = null;
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
