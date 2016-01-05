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

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace favoshelf.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class QuickAccessPage : Page
    {
        /// <summary>
        /// データモデル
        /// </summary>
        private FolderSelectViewModel m_viewModel;


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
            FolderListItem item = e.ClickedItem as FolderListItem;
            if (item == null)
            {
                return;
            }

            switch (item.Type)
            {
                case FolderListItem.FileType.Archive:
                case FolderListItem.FileType.ImageFile:
                    this.Frame.Navigate(typeof(ImageMainPage), item);
                    break;
                default:
                    //何もしない
                    break;
            }
        }

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
