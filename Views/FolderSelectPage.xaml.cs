using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
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
        private FolderSelectViewModel m_viewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderSelectPage()
        {
            this.InitializeComponent();
            m_viewModel = new FolderSelectViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Debug.WriteLine("FolderSelectPage#OnNavigatedFrom");
            FolderListItem item = e.Parameter as FolderListItem;
            if (item == null)
            {
                m_viewModel.Init(AppSettings.Current.FolderTokenList);
            }
            else
            {
                m_viewModel.InitFromFolderPath(item.Path);
            }

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

            string token = StorageApplicationPermissions.FutureAccessList.Add(folder);
            Debug.WriteLine("フォルダアクセストークン：" + token);
            List<string> folderTokenList = new List<string>(AppSettings.Current.FolderTokenList);
            folderTokenList.Add(token);
            AppSettings.Current.FolderTokenList = folderTokenList.ToArray();
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

        private void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FolderListItem item = e.ClickedItem as FolderListItem;
            if (item == null)
            {
                return;
            }

            if (item.Type == FolderListItem.FileType.Folder)
            {
                this.Frame.Navigate(typeof(FolderSelectPage), item);
            }
            else
            {
                Debug.WriteLine("ファイル選択 name=" + item.Name);
            }
        }
    }
}
