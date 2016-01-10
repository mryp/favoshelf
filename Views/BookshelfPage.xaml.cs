using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class BookshelfPage : Page
    {
        /// <summary>
        /// データモデル
        /// </summary>
        private FolderSelectViewModel m_viewModel;

        /// <summary>
        /// データベース
        /// </summary>
        private LocalDatabase m_db;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BookshelfPage()
        {
            this.InitializeComponent();

            m_viewModel = new FolderSelectViewModel();
            m_db = new LocalDatabase();
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
                param = new BookshelfNavigateParameter(m_db);
            }
            m_viewModel.Init(param);
            this.gridView.DataContext = m_viewModel;
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
            }
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
