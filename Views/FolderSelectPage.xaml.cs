using favoshelf.Data;
using favoshelf.Util;
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
    public sealed partial class FolderSelectPage : LayoutAwarePage
    {
        public FolderSelectViewModel ViewModel
        {
            get;
            private set;
        }

        private double? m_scrollPosition;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderSelectPage()
        {
            this.InitializeComponent();
            this.ViewModel = new FolderSelectViewModel();
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
            ViewModel.Init(param);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            base.LoadState(navigationParameter, pageState);
            
            if (pageState != null && pageState.ContainsKey("ScrollPosition"))
            {
                m_scrollPosition = pageState["ScrollPosition"] as double?;
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            base.SaveState(pageState);

            pageState["ScrollPosition"] = gridView.VerticalOffset;
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
            Debug.WriteLine("");
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
            if (args.Phase == 0)
            {
                if (m_scrollPosition.HasValue)
                {
                    gridView.ScrollToVerticalOffset(m_scrollPosition.Value);
                    if (m_scrollPosition.Value == 0 || gridView.VerticalOffset != 0)
                    {
                        m_scrollPosition = null;
                        args.RegisterUpdateCallback(1, gridView_ContainerContentChanging);
                    }
                    else
                    {
                        //スクロール位置が設定できなかったのでもう一度試す
                        args.RegisterUpdateCallback(0, gridView_ContainerContentChanging);
                    }
                }
                else
                {
                    args.RegisterUpdateCallback(1, gridView_ContainerContentChanging);
                }
            }
            else if (args.Phase == 1)
            {
                CommonPageManager.OnGridContentChanging(args.Item as FolderListItem, args.InRecycleQueue);
            }

            args.Handled = true;
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
        
        private void gridView_ScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            if (m_scrollPosition.HasValue)
            {
                gridView.ScrollToVerticalOffset(m_scrollPosition.Value);
                m_scrollPosition = null;
            }
            */
        }
    }
}
