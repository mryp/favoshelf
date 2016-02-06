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
    /// ページ選択ダイアログ
    /// </summary>
    public sealed partial class JumpPageDialog : ContentDialog
    {
        public JumpPageViewModel ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JumpPageDialog()
        {
            this.InitializeComponent();
            this.ViewModel = new JumpPageViewModel();
        }

        /// <summary>
        /// 移動するボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ContentDialogButtonClickDeferral deferral = args.GetDeferral();
            deferral.Complete();
        }

        /// <summary>
        /// キャンセルボタン桜花処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.ViewModel.SelectedIndex = -1;
        }

        /// <summary>
        /// 先頭へ移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void firstPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.JumpPageFist();
        }

        /// <summary>
        /// 最後へ移動ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lastPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.JumpPageLast();
        }
    }
}
