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
using System.Threading;
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
    public class ImageFolderViewModel : ImageViewModelBase
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
        protected override async Task initField(ImageNavigateParameter param)
        {
            StorageFolder folder;
            if (param.Type == ImageNavigateParameter.DataType.ImageFile)
            {
                StorageFile imageFile = await StorageFile.GetFileFromPathAsync(param.Path);
                folder = await imageFile.GetParentAsync();
            }
            else
            {
                folder = await StorageFolder.GetFolderFromPathAsync(param.Path);
            }
            StorageHistoryManager.AddStorage(folder, StorageHistoryManager.DataType.Latest);

            IReadOnlyList<StorageFile> fileList = await getImageFilesAsync(folder);
            this.DataList.Clear();
            foreach (StorageFile file in fileList)
            {
                this.DataList.Add(file);
            }

            if (param.Type == ImageNavigateParameter.DataType.ImageFile)
            {
                //選択した画像位置をデフォルトとしてセットする
                string imageFileName = Path.GetFileName(param.Path).ToLower();
                for (int i = 0; i < fileList.Count; i++)
                {
                    if (fileList[i].Name.ToLower() == imageFileName)
                    {
                        this.Index = i;
                        break;
                    }
                }
            }
            else
            {
                this.Index = 0;
            }
            this.CommandTitle = folder.DisplayName;
        }

        /// <summary>
        /// 指定したデータからビットマップデータを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override async Task<BitmapImage> createBitmap(object data)
        {
            StorageFile storage = data as StorageFile;
            if (storage == null)
            {
                Debug.WriteLine("対象外のオブジェクト data=" + data);
                return null;
            }
            
            BitmapImage bitmap = await BitmapUtils.CreateBitmap(storage);
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
