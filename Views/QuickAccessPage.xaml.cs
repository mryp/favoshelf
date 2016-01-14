using favoshelf.Data;
using favoshelf.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
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
    /// クイックアクセス（履歴）ページ
    /// </summary>
    public sealed partial class QuickAccessPage : LayoutAwarePage
    {
        /// <summary>
        /// 画面繊維パラメーター
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
        public QuickAccessPage()
        {
            this.InitializeComponent();
            this.ViewModel = new FolderSelectViewModel();
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
                m_naviParam = new QuickAccessNavigateParameter();
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
        /// 全削除ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void deleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog("全履歴の削除を実行しますか？", "確認");
            dialog.Commands.Add(new UICommand("OK", (command) => {
                StorageHistoryManager.RemoveAll(StorageHistoryManager.DataType.Latest);
                ViewModel.Init(m_naviParam);
            }));
            dialog.Commands.Add(new UICommand("キャンセル"));
            dialog.DefaultCommandIndex = 1;
            await dialog.ShowAsync();
        }
    }
}
