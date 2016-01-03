using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像表示ページのビューモデル
    /// </summary>
    public class ImageFolderViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 画像ファイルリスト
        /// </summary>
        private IReadOnlyList<StorageFile> m_fileList = null;

        /// <summary>
        /// 現在表示している画像の位置
        /// </summary>
        private int m_index = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageFolderViewModel()
        {
        }

        /// <summary>
        /// 指定ファイルにある画像一覧をリストにセットする
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task Init(FolderListItem item)
        {
            StorageFile imageFile = await StorageFile.GetFileFromPathAsync(item.Path);
            StorageFolder folder = await imageFile.GetParentAsync();
            m_fileList = await getImageFilesAsync(folder);

            //選択した画像位置をデフォルトとしてセットする
            for (int i = 0; i < m_fileList.Count; i++)
            {
                if (m_fileList[i].Name == imageFile.Name)
                {
                    m_index = i;
                    break;
                }
            }
        }
        
        /// <summary>
        /// 指定フォルダ以下にある画像ファイルのみを抽出して返す
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private async Task<IReadOnlyList<StorageFile>> getImageFilesAsync(StorageFolder folder)
        {
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, FileKind.GetImageFilterList());
            StorageFileQueryResult queryResult = folder.CreateFileQueryWithOptions(queryOptions);
            return await queryResult.GetFilesAsync();
        }

        /// <summary>
        /// 指定した位置の画像を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
            await bitmap.SetSourceAsync(stream);

            return bitmap;
        }

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
            if (m_index < m_fileList.Count-1)
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
