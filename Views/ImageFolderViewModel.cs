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
    public class ImageFolderViewModel : ImageViewModelBase<StorageFile>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageFolderViewModel()
        {
        }
        
        /// <summary>
        /// フィールドを初期化する
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override async Task initField(FolderListItem item)
        {
            StorageFile imageFile = await StorageFile.GetFileFromPathAsync(item.Path);
            StorageFolder folder = await imageFile.GetParentAsync();

            IReadOnlyList<StorageFile> fileList = await getImageFilesAsync(folder);
            this.DataList.Clear();
            foreach (StorageFile file in fileList)
            {
                this.DataList.Add(file);
            }

            //選択した画像位置をデフォルトとしてセットする
            for (int i = 0; i < this.DataList.Count; i++)
            {
                if (this.DataList[i].Name == imageFile.Name)
                {
                    this.Index = i;
                    break;
                }
            }
        }

        /// <summary>
        /// 指定したデータからビットマップデータを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override async Task<BitmapImage> createBitmap(StorageFile data)
        {
            //StorageFile file = await StorageFile.GetFileFromPathAsync(m_filePathList[index]);
            IRandomAccessStream stream = await data.OpenReadAsync();
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);

            return bitmap;
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

        public override void Dispose()
        {
            //なし
        }
    }
}
