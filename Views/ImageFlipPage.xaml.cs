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
    /// 画像めくり対応ページ
    /// </summary>
    public sealed partial class ImageFlipPage : Page
    {
        /// <summary>
        /// ローカルDB
        /// </summary>
        private LocalDatabase m_db;

        /// <summary>
        /// ビューモデル
        /// </summary>
        public ImageFlipViewModel ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageFlipPage()
        {
            this.InitializeComponent();

            m_db = new LocalDatabase();
            this.ViewModel = new ImageFlipViewModel();
        }
        
        /// <summary>
        /// 画面遷移してきたときの処理
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ImageNavigateParameter param = e.Parameter as ImageNavigateParameter;
            if (param == null)
            {
                return;
            }

            IImageFileReader reader = null;
            if (param.Type == ImageNavigateParameter.DataType.ImageFile
            || param.Type == ImageNavigateParameter.DataType.Folder)
            {
                reader = new FolderImageFileReader(param);
            }
            else if (param.Type == ImageNavigateParameter.DataType.Archive)
            {
                reader = new ZipImageFileReader(param);
            }
            else
            {
                return;
            }

            await reader.LoadDataAsync();
            this.ViewModel.Init(reader, m_db);
            setFirstImage();
        }

        /// <summary>
        /// 画面から離れるとき
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //ナビメニューの表示状態を元に戻す
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                shell.ShowMenuPane();
            }
        }
        
        /// <summary>
        /// 最初の画面を表示する
        /// </summary>
        private void setFirstImage()
        {
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                shell.HideMenu();
            }
        }
    }
}
