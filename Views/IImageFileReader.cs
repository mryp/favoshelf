using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像ビューワー画面使用する画像リーダーインターフェース
    /// </summary>
    public interface IImageFileReader
    {
        /// <summary>
        /// リストの個数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 画像読み込み親ファイル・フォルダ情報
        /// </summary>
        IStorageItem ParentStorage { get; }

        /// <summary>
        /// 画像表示の最初の画像（指定されていない時は-1）
        /// </summary>
        int FirstIndex { get; }

        /// <summary>
        /// データの初期化を行う
        /// </summary>
        /// <returns></returns>
        Task LoadDataAsync();

        /// <summary>
        /// 視程した位置の画像を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<BitmapImage> CreateBitmapAsync(int index);

        /// <summary>
        /// 視程した位置の画像をファイルコピーする
        /// </summary>
        /// <param name="index"></param>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<StorageFile> CopyFileAsync(int index, StorageFolder folder, string fileName);
    }
}
