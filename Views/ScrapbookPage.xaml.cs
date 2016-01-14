using favoshelf.Data;
using favoshelf.Util;
using System;
using System.Collections.Generic;
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
    /// スクラップブックカテゴリ表示ページ
    /// </summary>
    public sealed partial class ScrapbookPage : LayoutAwarePage
    {
        /// <summary>
        /// ビューモデル
        /// </summary>
        public FolderSelectViewModel ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// データベース
        /// </summary>
        private LocalDatabase m_db;

        public ScrapbookPage()
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

            INavigateParameter param = navigationParameter as INavigateParameter;
            if (param == null)
            {
                param = new ScrapbookNavigateParameter(m_db);
            }
            ViewModel.Init(param);
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

            if (item.Type != FolderListItem.FileType.Scrapbook)
            {
                return;
            }

            ScrapbookCategory category = m_db.QueryScrapbookCategory(item.Name);
            IEnumerable<ScrapbookItem> scrapItemLit = m_db.QueryScrapbookItemList(category);
            if (scrapItemLit.Count() == 0)
            {
                return;
            }
            
            this.Frame.Navigate(typeof(FolderSelectPage), new ScrapbookItemNavigateParameter(m_db, category.FolderName));
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
            InsertScrapbookDialog dialog = new InsertScrapbookDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.FolderName))
            {
                ScrapbookCategory category = new ScrapbookCategory()
                {
                    FolderName = dialog.FolderName,
                };
                if (EnvPath.GetScrapbookSubFolder(category.FolderName) != null)
                {
                    m_db.InsertScrapbookCategory(category);
                }
            }
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
