using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace favoshelf.Views
{
    /// <summary>
    /// フォルダページ共通化管理クラス
    /// </summary>
    public class CommonPageManager
    {
        /// <summary>
        /// フォルダ・ファイルアイテムをクリックした時の遷移処理
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="item"></param>
        /// <returns></returns>
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
                    navigateFolder(frame, item);
                    result = true;
                    break;
                case FolderListItem.FileType.Archive:
                    result = frame.Navigate(typeof(ImageFlipPage), 
                        new ImageNavigateParameter(ImageNavigateParameter.DataType.Archive, item.Path));
                    break;
                case FolderListItem.FileType.ImageFile:
                    result = frame.Navigate(typeof(ImageFlipPage),
                        new ImageNavigateParameter(ImageNavigateParameter.DataType.ImageFile, item.Path));
                    break;
                default:
                    //何もしない
                    break;
            }

            return result;
        }
        
        /// <summary>
        /// フォルダー用遷移処理
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="item"></param>
        private static async void navigateFolder(Frame frame, FolderListItem item)
        {
            if (item.Type != FolderListItem.FileType.Folder)
            {
                return;
            }

            LocalDatabase db = new LocalDatabase();
            if (db.QueryBookmark(item.Path) == null)
            {
                frame.Navigate(typeof(FolderSelectPage), new FolderPathNavigateParameter(item.Path));
                return;
            }

            MessageDialog dialog = new MessageDialog("しおりが見つかりました。続きから表示しますか？", "確認");
            dialog.Commands.Add(new UICommand("続きから画像を表示", (command) => {
                frame.Navigate(typeof(ImageFlipPage),
                    new ImageNavigateParameter(ImageNavigateParameter.DataType.Folder, item.Path));
            }));
            dialog.Commands.Add(new UICommand("ファイル一覧", (command) => {
                frame.Navigate(typeof(FolderSelectPage), new FolderPathNavigateParameter(item.Path));
            }));
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }
        
        /// <summary>
        /// グリッド更新処理
        /// </summary>
        /// <param name="listItem"></param>
        /// <param name="inRecycleQueue"></param>
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

        /// <summary>
        /// グリッド内のボタン処理
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="e"></param>
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
