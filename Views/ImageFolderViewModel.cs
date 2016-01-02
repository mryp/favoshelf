using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    public class ImageFolderViewModel : INotifyPropertyChanged
    {
        //private List<string> m_filePathList = new List<string>();
        private IReadOnlyList<StorageFile> m_fileList = null;
        private int m_index = 0;

        public ImageFolderViewModel()
        {
        }

        public async Task Init(FolderListItem item)
        {
            StorageFolder folder = null;
            if (item.Token == "")
            {
                folder = await StorageFolder.GetFolderFromPathAsync(item.Path);
            }
            else
            {
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem(item.Token))
                {
                    folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(item.Token);
                }
            }
            
            if (folder != null)
            {
                m_fileList = await folder.GetFilesAsync();
            }
        }

        public async Task<BitmapImage> GetImage(int index)
        {
            if (m_fileList == null)
            {
                return null;
            }
            if (index < 0 || index >= m_fileList.Count)
            {
                return null;
            }
            
            //StorageFile file = await StorageFile.GetFileFromPathAsync(m_filePathList[index]);
            IRandomAccessStream stream = await m_fileList[index].OpenReadAsync();
            var bitmap = new BitmapImage();
            bitmap.SetSource(stream);

            return bitmap;
        }

        public async Task<BitmapImage> GetImage()
        {
            return await GetImage(m_index);
        }

        public async Task<BitmapImage> GetNextImage()
        {
            if (m_index < m_fileList.Count-1)
            {
                m_index++;
            }
            return await GetImage(m_index);
        }

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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
