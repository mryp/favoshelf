using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace favoshelf.Views
{
    /// <summary>
    /// フォルダ一覧表示用ページ
    /// </summary>
    public sealed partial class FolderSelectPage : Page
    {
        /// <summary>
        /// データモデル
        /// </summary>
        private FolderSelectViewModel m_viewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderSelectPage()
        {
            this.InitializeComponent();
            m_viewModel = new FolderSelectViewModel();
        }

        /// <summary>
        /// 画面遷移されてきたときの処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            FolderListItem item = e.Parameter as FolderListItem;
            if (item == null)
            {
                m_viewModel.InitFromToken(StorageHistoryManager.GetTokenList(StorageHistoryManager.DataType.Folder));
            }
            else
            {
                m_viewModel.InitFromFolderPath(item.Path);
            }

            this.gridView.DataContext = m_viewModel;
        }

        /// <summary>
        /// フォルダを追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await selectFolder();
            if (folder == null)
            {
                return; //未選択
            }

            StorageHistoryManager.AddStorage(folder, StorageHistoryManager.DataType.Folder);
        }

        /// <summary>
        /// フォルダ選択ダイアログからフォルダを選択して返す
        /// </summary>
        /// <returns>選択フォルダ（未選択時はnull）</returns>
        private async Task<StorageFolder> selectFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                Debug.WriteLine("選択フォルダ：" + folder.Path);
            }

            return folder;
        }

        /// <summary>
        /// グリッドビューのアイテム変更状態変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void gridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            FolderListItem listItem = args.Item as FolderListItem;
            if (listItem != null)
            {
                if (args.InRecycleQueue)
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
        /// アイテムを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FolderListItem item = e.ClickedItem as FolderListItem;
            if (item == null)
            {
                return;
            }

            switch (item.Type)
            {
                case FolderListItem.FileType.Folder:
                    this.Frame.Navigate(typeof(FolderSelectPage), item);
                    break;
                case FolderListItem.FileType.Archive:
                case FolderListItem.FileType.ImageFile:
                    this.Frame.Navigate(typeof(ImageMainPage), item);
                    break;
                case FolderListItem.FileType.OtherFile:
                default:
                    //何もしない
                    break;
            }
        }

        /// <summary>
        /// 全画面グリッドでタッチ・マウスボタンを離したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(this.Frame);
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                if (point.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
                {
                    //戻る
                    if (this.Frame.CanGoBack)
                    {
                        this.Frame.GoBack();
                    }
                }
            }
        }
    }
}
