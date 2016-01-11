using favoshelf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace favoshelf.Views
{
    /// <summary>
    /// 画像表示用ページ
    /// </summary>
    public sealed partial class ImageMainPage : Page
    {
        /// <summary>
        /// タッチ場所定義
        /// </summary>
        private enum TouchPosition
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }

        /// <summary>
        /// ビューモデル
        /// </summary>
        private ImageViewModelBase m_viewModel;

        /// <summary>
        /// ローカルDB
        /// </summary>
        private LocalDatabase m_db;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageMainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            m_db = new LocalDatabase();
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

            if (param.Type == ImageNavigateParameter.DataType.ImageFile
            ||  param.Type == ImageNavigateParameter.DataType.Folder)
            {
                m_viewModel = new ImageFolderViewModel();
            }
            else if (param.Type == ImageNavigateParameter.DataType.Archive)
            {
                m_viewModel = new ImageZipViewModel();
            }

            await m_viewModel.Init(param);
            this.DataContext = m_viewModel;
            setFirstImage();
            initBookCategory();
            initScrapbookCategory();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            
            //ナビメニューの表示状態を元に戻す
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                shell.IsNaviMenuPaneOpen = m_isOpenNaviDefault;
            }
        }

        /// <summary>
        /// 本棚リストを初期化する
        /// </summary>
        private void initBookCategory()
        {
            foreach (BookCategory category in m_db.QueryBookCategoryAll())
            {
                addBookCategoryMenuItem(category);
            }
        }

        /// <summary>
        /// 本棚アイテムを作成しメニューに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addBookCategoryMenuItem(BookCategory category)
        {
            ToggleMenuFlyoutItem buttonItem = new ToggleMenuFlyoutItem()
            {
                Text = category.Label,
                Tag = category,
            };
            buttonItem.Click += BookshelfButtonItem_Click;
            if (m_db.QueryBookItemFromPath(category.Id, m_viewModel.ImageStorage.Path) == null)
            {
                buttonItem.IsChecked = false;
            }
            else
            {
                buttonItem.IsChecked = true;
            }
            bookshelfMenu.Items.Add(buttonItem);
        }

        /// <summary>
        /// スクラップブックカテゴリ一覧を作成する
        /// </summary>
        private void initScrapbookCategory()
        {
            foreach (ScrapbookCategory category in m_db.QueryScrapbookCategoryAll())
            {
                addScrapbookCategoryMenuItem(category);
            }
        }

        /// <summary>
        /// スクラップブックカテゴリをメニューに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addScrapbookCategoryMenuItem(ScrapbookCategory category)
        {
            MenuFlyoutItem buttonItem = new MenuFlyoutItem()
            {
                Text = category.FolderName,
                Tag = category,
            };
            buttonItem.Click += ScrapbookButtonItem_Click;
            scrapbookMenu.Items.Add(buttonItem);
        }

        /// <summary>
        /// ウィンドウでキャッチするキー入力イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.Down:
                    setNextImage();
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.Up:
                    setPrevImage();
                    break;
            }
        }

        private bool m_isOpenNaviDefault = false;
        /// <summary>
        /// 最初の画面を表示する
        /// </summary>
        private void setFirstImage()
        {
            AppShell shell = Window.Current.Content as AppShell;
            if (shell != null)
            {
                m_isOpenNaviDefault = shell.IsNaviMenuPaneOpen;
                shell.IsNaviMenuPaneOpen = false;
            }
            m_viewModel.GetImage();
        }

        /// <summary>
        /// 次の画像を表示する
        /// </summary>
        private void setNextImage()
        {
            m_viewModel.GetNextImage();
        }

        /// <summary>
        /// 前の画像を表示する
        /// </summary>
        private void setPrevImage()
        {
            m_viewModel.GetPrevImage();
        }
        
        /// <summary>
        /// タッチ・クリック検知イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void touchPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            PointerPoint point = e.GetCurrentPoint(this.mainGrid);
            TouchPosition touchPos = getTouchPosition(sender as UIElement);
            
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.XButton1Released)
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            else if (touchPos == TouchPosition.TopLeft || touchPos == TouchPosition.BottomLeft)
            {
                setNextImage();
            }
            else if (touchPos == TouchPosition.TopRight || touchPos == TouchPosition.BottomRight)
            {
                setPrevImage();
            }
        }

        /// <summary>
        /// タッチされたパネルからタッチ位置を取得する
        /// </summary>
        /// <param name="touchControl"></param>
        /// <returns></returns>
        private TouchPosition getTouchPosition(UIElement touchControl)
        {
            if (touchControl == this.touchPanelTopLeft)
            {
                return TouchPosition.TopLeft;
            }
            if (touchControl == this.touchPanelTopRight)
            {
                return TouchPosition.TopRight;
            }
            if (touchControl == this.touchPanelBottomLeft)
            {
                return TouchPosition.BottomLeft;
            }
            if (touchControl == this.touchPanelBottomRight)
            {
                return TouchPosition.BottomRight;
            }

            return TouchPosition.None;
        }

        /// <summary>
        /// マウスホイールイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void touchPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            Pointer pointer = e.Pointer;
            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                PointerPoint point = e.GetCurrentPoint(this.imageView);
                int mouseWheelDelta = point.Properties.MouseWheelDelta;
                if (mouseWheelDelta > 0)    //上
                {
                    setPrevImage();
                }
                else
                {
                    setNextImage();
                }
            }
        }

        /// <summary>
        /// カテゴリに登録・解除を行う
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookshelfButtonItem_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item == null)
            {
                return;
            }
            BookCategory category = item.Tag as BookCategory;
            if (category == null)
            {
                return;
            }

            Debug.WriteLine("label=" + category.Label + " toggle=" + item.IsChecked.ToString());
            if (item.IsChecked)
            {
                addBookItem(category);
            }
            else
            {
                removeBookItem(category);
            }
        }

        /// <summary>
        /// 新しい本棚を作成して追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewBookShelfMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            InsertBookshelfDialog dialog = new InsertBookshelfDialog();
            await dialog.ShowAsync();
            if (!string.IsNullOrEmpty(dialog.Label))
            {
                BookCategory category = new BookCategory()
                {
                    Label = dialog.Label,
                };
                if (m_db.InsertBookCategory(category))
                {
                    addBookItem(category);
                    addBookCategoryMenuItem(category);
                }
            }
        }

        /// <summary>
        /// 指定した本棚カテゴリに追加する
        /// </summary>
        /// <param name="category"></param>
        private void addBookItem(BookCategory category)
        {
            string token = StorageHistoryManager.AddStorage(m_viewModel.ImageStorage, StorageHistoryManager.DataType.Bookshelf);
            m_db.InsertBookItem(new BookItem()
            {
                BookCategoryId = category.Id,
                Token = token,
                Path = m_viewModel.ImageStorage.Path,
                Uptime = DateTime.Now
            });
        }

        /// <summary>
        /// 指定した本棚カテゴリから削除する
        /// </summary>
        /// <param name="category"></param>
        private void removeBookItem(BookCategory category)
        {
            BookItem bookItem = m_db.QueryBookItemFromPath(category.Id, m_viewModel.ImageStorage.Path);
            if (bookItem != null)
            {
                StorageHistoryManager.RemoveStorage(bookItem.Token);
                m_db.DeleteBookItem(bookItem);
            }
        }

        /// <summary>
        /// スクラップ登録ボタンを押下した時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrapbookButtonItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item == null)
            {
                return;
            }
            ScrapbookCategory category = item.Tag as ScrapbookCategory;
            if (category == null)
            {
                return;
            }

            Debug.WriteLine("FolderName=" + category.FolderName);
            addScrapbookItem(category);
        }

        /// <summary>
        /// 新しいスクラップカテゴリを作成し登録する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewScrapbookMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
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
                    if (m_db.InsertScrapbookCategory(category))
                    {
                        addScrapbookItem(category);
                        addScrapbookCategoryMenuItem(category);
                    }
                }
            }
        }

        /// <summary>
        /// スクラップカテゴリに現在表示している画像を登録する
        /// </summary>
        /// <param name="category"></param>
        private async void addScrapbookItem(ScrapbookCategory category)
        {
            string fileName = EnvPath.CreateScrapbookFileName();
            StorageFolder folder = await EnvPath.GetScrapbookSubFolder(category.FolderName);
            StorageFile copyFile = await m_viewModel.CopyFileAsync(folder, fileName);
            if (copyFile == null)
            {
                Debug.WriteLine("ファイルコピー失敗");
                return;
            }

            m_db.InsertScrapbookItem(new ScrapbookItem()
            {
                ScrapbookCategoryId = category.Id,
                FileName = Path.GetFileName(copyFile.Path),
                Uptime = DateTime.Now
            });
            Debug.WriteLine("保存成功 path=" + copyFile.Path);
        }
    }

}
