using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace favoshelf.Views
{
    interface IImageAccess
    {
        /// <summary>
        /// 現在位置の画像を取得する
        /// </summary>
        /// <returns></returns>
        Task<BitmapImage> GetImage();

        /// <summary>
        /// 次の画像を取得する
        /// </summary>
        /// <returns></returns>
        Task<BitmapImage> GetNextImage();

        /// <summary>
        /// 前の画像を取得する
        /// </summary>
        /// <returns></returns>
        Task<BitmapImage> GetPrevImage();
    }
}
