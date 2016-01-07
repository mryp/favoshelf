using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// クイックアクセス（履歴）ページ
    /// </summary>
    public sealed partial class QuickAccessPage : Page
    {
        /// <summary>
        /// データモデル
        /// </summary>
        private FolderSelectViewModel m_viewModel;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public QuickAccessPage()
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
            
            m_viewModel.InitFromToken(StorageHistoryManager.GetTokenList(StorageHistoryManager.DataType.Latest));
            this.gridView.DataContext = m_viewModel;
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

        /// <summary>
        /// ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == deleteAllButton)
            {
                StorageHistoryManager.RemoveAll(StorageHistoryManager.DataType.Latest);
            }
        }
    }
}
