using favoshelf.Data;
using favoshelf.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    /// 本棚ページ
    /// </summary>
    public sealed partial class BookshelfPage : LayoutAwarePage
    {
        /// <summary>
        /// データベース
        /// </summary>
        private LocalDatabase m_db;

        /// <summary>
        /// 画面遷移パラメーター
        /// </summary>
        private INavigateParameter m_naviParam;

        /// <summary>
        /// ビューモデル
        /// </summary>
        public FolderSelectViewModel ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookshelfPage()
        {
            this.InitializeComponent();

            this.ViewModel = new FolderSelectViewModel();
            m_db = new LocalDatabase();
        }

        /// <summary>
        /// 画面ステータスを読み込む
        /// </summary>
        /// <param name="navigationParameter"></param>
        /// <param name="pageState"></param>
        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            base.LoadState(navigationParameter, pageState);

            m_naviParam = navigationParameter as INavigateParameter;
            if (m_naviParam == null)
            {
                m_naviParam = new BookshelfNavigateParameter(m_db);
            }
            ViewModel.Init(m_naviParam);
        }

        /// <summary>
        /// 画面ステータスを保存する
        /// </summary>
        /// <param name="pageState"></param>
        protected override void SaveState(Dictionary<string, object> pageState)
        {
            base.SaveState(pageState);
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

            if (item.Type != FolderListItem.FileType.Bookshelf)
            {
                return;
            }
            
            BookCategory category = m_db.QueryBookCategory(item.Name);
            IEnumerable<BookItem> bookItemList = m_db.QueryBookItemList(category);
            if (bookItemList.Count() == 0)
            {
                return;
            }

            this.Frame.Navigate(typeof(FolderSelectPage), new BookItemNavigateParameter(m_db, category.Label));
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
        /// カテゴリの追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            InsertBookshelfDialog dialog = new InsertBookshelfDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.Label))
            {
                BookCategory category = new BookCategory()
                {
                    Label = dialog.Label,
                };
                m_db.InsertBookCategory(category);

                this.ViewModel.Init(m_naviParam);
            }
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
        }
        
        /// <summary>
        /// リスト項目を右クリックした時（マウス用）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void girdViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                return;
            }

            showContextMenu(sender as FrameworkElement);
            e.Handled = true;
        }

        /// <summary>
        /// リスト項目を長押しした時（タッチ用）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridViewItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != Windows.UI.Input.HoldingState.Started)
            {
                return;
            }

            showContextMenu(sender as FrameworkElement);
            e.Handled = true;
        }

        /// <summary>
        /// リストアイテム用コンテキストを表示する
        /// </summary>
        /// <param name="senderElement"></param>
        private void showContextMenu(FrameworkElement senderElement)
        {
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        /// <summary>
        /// 指定したアイテムを削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            FolderListItem item = (sender as MenuFlyoutItem).DataContext as FolderListItem;
            if (item == null)
            {
                return;
            }

            MessageDialog dialog = new MessageDialog(item.Name + "を本棚から削除しますか？", "確認");
            dialog.Commands.Add(new UICommand("OK", (command) => {
                //TODO:ここに削除処理
                BookCategory category = m_db.QueryBookCategory(item.Name);
                foreach (BookItem bookItem in m_db.QueryBookItemList(category))
                {
                    StorageHistoryManager.RemoveStorage(bookItem.Token);
                }
                m_db.DeleteBookCategory(category);

                this.ViewModel.Init(m_naviParam);
            }));
            dialog.Commands.Add(new UICommand("キャンセル"));
            dialog.DefaultCommandIndex = 1;
            await dialog.ShowAsync();
        }
    }
}
