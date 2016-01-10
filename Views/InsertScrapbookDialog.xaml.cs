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

//コンテンツ ダイアログ項目テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください。

namespace favoshelf.Views
{
    public sealed partial class InsertScrapbookDialog : ContentDialog
    {
        public string FolderName
        {
            get;
            private set;
        }

        public InsertScrapbookDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrEmpty(folderNameTextBox.Text))
            {
                this.FolderName = "";
                args.Cancel = true;
            }
            else
            {
                this.FolderName = folderNameTextBox.Text;
            }

            ContentDialogButtonClickDeferral deferral = args.GetDeferral();
            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.FolderName = "";
        }
    }
}
