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
                    result = frame.Navigate(typeof(FolderSelectPage), item);
                    break;
                case FolderListItem.FileType.Archive:
                case FolderListItem.FileType.ImageFile:
                    result = frame.Navigate(typeof(ImageMainPage), item);
                    break;
                case FolderListItem.FileType.OtherFile:
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
