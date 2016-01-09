using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace favoshelf.Views
{
    /// <summary>
    /// フォルダページ共通化管理クラス
    /// </summary>
    public class CommonPageManager
    {
        public static bool OnGridViewItemClick(Frame frame, FolderListItem item)
        {
            if (item == null)
            {
                return false;
            }

            bool result = false;
            switch (item.Type)
            {
                case FolderListItem.FileType.Folder:
                    result = frame.Navigate(typeof(FolderSelectPage), new FolderPathNavigateParameter(item.Path));
                    break;
                case FolderListItem.FileType.Archive:
                    result = frame.Navigate(typeof(ImageMainPage), 
                        new ImageNavigateParameter(ImageNavigateParameter.DataType.Archive, item.Path));
                    break;
                case FolderListItem.FileType.ImageFile:
                    result = frame.Navigate(typeof(ImageMainPage), 
                        new ImageNavigateParameter(ImageNavigateParameter.DataType.ImageFile, item.Path));
                    break;
                default:
                    //何もしない
                    break;
            }

            return result;
        }
        
        public static void OnGridContentChanging(FolderListItem listItem, bool inRecycleQueue)
        {
            if (listItem != null)
            {
                //画像を遅延読み込みする
                if (inRecycleQueue)
                {
                    listItem.ReleaseThumImage();
                }
                else
                {
                    listItem.UpdateThumImage();
                }
            }
        }

        public static void OnGridPointerReleased(Frame frame, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(frame);
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                if (point.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
                {
                    //戻る
                    if (frame.CanGoBack)
                    {
                        frame.GoBack();
                    }
                }
            }
        }
    }
}
