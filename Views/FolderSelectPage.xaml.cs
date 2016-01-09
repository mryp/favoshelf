using favoshelf.Data;
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

            INavigateParameter param = e.Parameter as INavigateParameter;
            if (param == null)
            {
                param = new FolderRootNavigateParameter();
            }
            m_viewModel.Init(param);

            /*
            FolderListItem item = e.Parameter as FolderListItem;
            if (item == null)
            {
                m_viewModel.InitFromToken(StorageHistoryManager.GetTokenList(StorageHistoryManager.DataType.Folder));
            }
            else
            {
                m_viewModel.InitFromFolderPath(item.Path);
            }
            */

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
        /// アイテムを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            CommonPageManager.OnGridViewItemClick(this.Frame, e.ClickedItem as FolderListItem);
        }

        /// <summary>
        /// グリッドビューのアイテム変更状態変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void gridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            CommonPageManager.OnGridContentChanging(args.Item as FolderListItem, args.InRecycleQueue);
        }

        /// <summary>
        /// 全画面グリッドでタッチ・マウスボタンを離したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CommonPageManager.OnGridPointerReleased(this.Frame, e);
        }
    }
}
